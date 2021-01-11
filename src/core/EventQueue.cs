using KMS.src.tool;
using System;
using System.Threading;

namespace KMS.src.core
{
    /// <summary>
    /// Manage the keyboard and mouse event, storage and migrate it
    /// </summary>
    static class EventQueue
    {
        private const int EQ_STEP_IDLE = 0;
        private const int EQ_STEP_READY = 1;
        private const int EQ_STEP_ENQUEUING = 2;

        private const int SLEEP_DURATION = 5; //5ms
        private const int MAX_WAIT_TIMES = 4;

        private static int amount;
        private static int enqueueStep;
        private static bool isMigrating;

        internal const sbyte EVENT_TYPE_KEYBOARD = 5;
        internal const sbyte EVENT_TYPE_MOUSE = 6;

        internal struct KMEvent
        {
            internal sbyte type; //keyboard event or mouse event
            internal short eventCode; //down or up
            internal short keyCode; //which key
            internal short x; //for mouse event
            internal short y; //for mouse event
            internal DateTime time;
        }

        //storage the event.
        internal const int MAX_EVENT_AMOUNT = 500;
        private static KMEvent[] events = new KMEvent[MAX_EVENT_AMOUNT];

        /// <summary>
        /// Can only be call by KMEventHook to storage the original keyboard and mouse event.
        /// </summary>
        internal static void enqueue(sbyte type, short eventCode, short keyCode, short x, short y)
        {
            int loopCounter = 0;
        BEGIN:
            if (enqueueStep == EQ_STEP_IDLE)
            {
                if (isMigrating)
                {
                    if (loopCounter < MAX_WAIT_TIMES)
                    {
                        loopCounter++;
                        Thread.Sleep(SLEEP_DURATION);
                        goto BEGIN;
                    }
                    else
                    {
                        Logger.v("EventQueue", "wait for enqueue event timeout1");
                    }
                }
                else
                {
                    enqueueStep = EQ_STEP_READY;
                    if (isMigrating)
                    {
                        if (loopCounter < MAX_WAIT_TIMES)
                        {
                            loopCounter++;
                            Thread.Sleep(SLEEP_DURATION);
                            goto BEGIN;
                        }
                        else
                        {
                            Logger.v("EventQueue", "wait for enqueue event timeout2");
                        }
                    }
                    else
                    {
                        enqueueStep = EQ_STEP_ENQUEUING;

                        events[amount].type = type;
                        events[amount].eventCode = eventCode;
                        events[amount].keyCode = keyCode;
                        events[amount].x = x;
                        events[amount].y = y;
                        events[amount].time = DateTime.Now;

                        amount++;
                    }
                }
            }
            else
            {
                if (loopCounter < MAX_WAIT_TIMES)
                {
                    loopCounter++;
                    Thread.Sleep(SLEEP_DURATION);
                    goto BEGIN;
                }
                else
                {
                    Logger.v("EventQueue", "wait for enqueue event timeout3");
                }
            }

            enqueueStep = EQ_STEP_IDLE;
        }

        /// <summary>
        /// Can only be call by 'StatisticThread',
        /// copy all the event to another place to process.
        /// </summary>
        internal static void migrate(ref KMEvent[] e, ref int amount)
        {
            isMigrating = true;

            for (int i = 0; i < EventQueue.amount; i++)
            {
                e[i].type = events[i].type;
                e[i].eventCode = events[i].eventCode;
                e[i].keyCode = events[i].keyCode;
                e[i].x = events[i].x;
                e[i].y = events[i].y;
                e[i].time = events[i].time;
            }
            amount = EventQueue.amount;
            EventQueue.amount = 0;

            isMigrating = false;
        }
    }
}
