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
            Logger.v(TAG, $"ThreadTest_Click, current thread id:{Thread.CurrentThread.ManagedThreadId}");
            Thread t = new Thread(ThreadProc);
            t.Name = "Thread_test";
            t.Start();
            Console.WriteLine($"thread {t.Name} started");
        }

        private void ThreadTest_Click2(object sender, RoutedEventArgs e)
        {
            mre.Set();
        }

        private ManualResetEvent mre = new ManualResetEvent(false);
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
        }
    }
}
