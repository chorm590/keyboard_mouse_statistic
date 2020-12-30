using KMS.src.tool;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace KMS.src.core
{
    /// <summary>
    /// created at 2020-12-30 19:54
    /// </summary>
    static class StatisticThread
    {
        private const string TAG = "StatisticThread";

        internal static bool CanThreadRun
        {
            get;
            set;
        }

        internal static void ThreadProc()
        {
            Logger.v(TAG, "ThreadProc() run");
            CanThreadRun = true;
            while (CanThreadRun)
            {
                //Logger.v(TAG, "hello sub-thread," + Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(2000); //Check every 2s.
            }
            Logger.v(TAG, "ThreadProc() end");
        }

        
    }
}
