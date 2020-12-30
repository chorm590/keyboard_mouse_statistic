using System;
using System.Threading;
using System.Windows;

using KMS.src.core;
using KMS.src.tool;

namespace KMS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private const string TAG = "KMS";

        public MainWindow()
        {
            InitializeComponent();
            Logger.v(TAG, "hello world");
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            KMEventHook.InsertHook();
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            //GlobalEventListener.RemoveHook();
        }

        private void ThreadTest_Click(object sender, RoutedEventArgs e)
        {
            //Thread t = new Thread(StatisticThread.ThreadProc);
            //t.Start();
        }

        private void ThreadTest_Click2(object sender, RoutedEventArgs e)
        {
            //mre.Set();
            StatisticThread.CanThreadRun = false;
        }

/*        private ManualResetEvent mre = new ManualResetEvent(false);
        private void ThreadProc()
        {
            string name = Thread.CurrentThread.Name;
            while (true)
            {
                Console.WriteLine(name + " starts and calls mre.WaitOne(), thread id:" + Thread.CurrentThread.ManagedThreadId);

                mre.WaitOne();
                mre.Reset();

                Console.WriteLine(name + " ends.");
                Thread.Sleep(500);
            }
        }*/
    }
}
