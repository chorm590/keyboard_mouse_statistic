using KMS.src.tool;
using System.Threading;
using System.ComponentModel;
using System.Windows;
using System;

namespace KMS.src.core
{
    /// <summary>
    /// created at 2020-12-30 19:54
    /// </summary>
    static class StatisticThread
    {
        private const string TAG = "StatisticThread";

        private static EventQueue.KMEvent[] events = new EventQueue.KMEvent[EventQueue.MAX_EVENT_AMOUNT];
        private static int eventAmount;
        private static int idx;

        internal static bool CanThreadRun;

        internal static void ThreadProc()
        {
            Logger.v(TAG, "ThreadProc() run");
            CanThreadRun = true;
            while (CanThreadRun)
            {
                statistic();
                Thread.Sleep(2000); //Check every 2s.
            }
            Logger.v(TAG, "ThreadProc() end");
        }

        private static void statistic()
        {
            eventAmount = 0;
            EventQueue.migrate(ref events, ref eventAmount);
            Logger.v(TAG, "event amount:" + eventAmount);

            for (idx = 0; idx < eventAmount; idx++)
            {
                if (events[idx].type == EventQueue.EVENT_TYPE_KEYBOARD)
                {
                    parseKeyboardEvent(events[idx].eventCode, events[idx].keyCode);
                }
                else if (events[idx].type == EventQueue.EVENT_TYPE_MOUSE)
                {
                    parseMouseEvent(events[idx].eventCode, events[idx].keyCode, events[idx].x, events[idx].y);
                }
                else
                {
                    Logger.v(TAG, "Invalid event type");
                }
            }
        }

        private static void parseKeyboardEvent(short eventCode, short keyCode)
        {

        }

        private static void parseMouseEvent(short eventCode, short keyCode, short x, short y)
        { 
            
        }
    }
}
