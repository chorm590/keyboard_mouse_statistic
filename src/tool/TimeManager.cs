using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace KMS.src.tool
{
    /// <summary>
    /// 为避免跨日时间重置带来的影响，将整个程序的时间统一标准。
    /// 所有需要参照系统时间来访问数据库的情况都从这里取时间值。
    /// 2021-01-07 10:38
    /// </summary>
    static class TimeManager
    {
        internal delegate void TimerCallback(object obj);

        private static Timer timer;
        private static Dictionary<string, TimerCallback> timerCb;

        internal static void Init()
        {
            if (timer is null)
            {
                timer = new Timer(timerCallback, null, 60000, 60000);
            }

            if (timerCb is null)
            {
                timerCb = new Dictionary<string, TimerCallback>();
            }
            else
            {
                timerCb.Clear();
            }
        }

        internal static DateTime TimeUsing
        {
            get;
            set;
        }

        internal static void RegisterTimerCallback(string name, TimerCallback cb)
        {
            timerCb.TryAdd(name, cb);
        }

        private static void timerCallback(object state)
        {
            Dictionary<string, TimerCallback>.ValueCollection values = timerCb.Values;
            foreach (TimerCallback cb in values)
            {
                cb(state);
            }
        }

        internal static void DestroyTimer()
        {
            timer.Dispose();
            timerCb.Clear();
        }
    }
}
