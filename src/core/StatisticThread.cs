using KMS.src.tool;
using System.Threading;
using System.ComponentModel;

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

        internal static StatisticResult statisticResult;

        internal static bool CanThreadRun
        {
            get;
            set;
        }

        internal static void ThreadProc()
        {
            Logger.v(TAG, "ThreadProc() run");
            CanThreadRun = true;
            statisticResult = new StatisticResult();
            while (CanThreadRun)
            {
                statistic();
                Thread.Sleep(2000); //Check every 2s.
            }
            Logger.v(TAG, "ThreadProc() end");
            statisticResult.EventAmount = 0;
            statisticResult = null;
        }

        private static void statistic()
        {
            eventAmount = 0;
            EventQueue.migrate(ref events, ref eventAmount);
            statisticResult.EventAmount = eventAmount;
            Logger.v(TAG, "event amount:" + eventAmount);

            for (idx = 0; idx < eventAmount; idx++)
            {
                
            }
        }

        internal class StatisticResult : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private PropertyChangedEventArgs pca;
            private int eventAmount;

            internal StatisticResult()
            {
                pca = new PropertyChangedEventArgs("EventAmount");
            }

            public int EventAmount
            {
                get
                {
                    return eventAmount;
                }

                set
                {
                    eventAmount = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged.Invoke(this, pca);
                    }
                }
            }
        }
    }
}
