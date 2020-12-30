using KMS.src.tool;
using System;
using System.Collections.Generic;
using System.Text;

namespace KMS.src.core
{
    static class EventQueue
    {
        internal struct KMEvent
        {
            internal sbyte type; //keyboard event or mouse event
            internal short eventCode; //down or up
            internal short keyCode; //which key
            internal short x; //for mouse event
            internal short y; //for mouse event
        }

        //storage the event.
        private static KMEvent[] events = new KMEvent[1000];
        private static int amount;

        /// <summary>
        /// Can only be call by KMEventHook to storage the original keyboard and mouse event.
        /// </summary>
        internal static void enqueue()
        {
            
        }

        /// <summary>
        /// Can only be call by 'StatisticThread',
        /// copy all the event to another place to process.
        /// </summary>
        internal static void migrate(ref KMEvent[] e, ref int amount)
        {
            for(int i = 0; i < 1000; i++)
            {
                e[i].type = (sbyte)i;
                e[i].eventCode = events[i].eventCode;
                e[i].keyCode = events[i].keyCode;
                e[i].x = events[i].x;
                e[i].y = events[i].y;
            }
            amount = 1000;
        }
    }
}
