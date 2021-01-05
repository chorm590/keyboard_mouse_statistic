using System;
using System.Threading;
using System.Windows;
using KMS.src.core;
using KMS.src.tool;
using KMS.src.db;

namespace KMS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private const string TAG = "KMS";

        private Thread statisticThread;
        private SQLiteManager sqliteManager;

        public MainWindow()
        {
            InitializeComponent();
            Logger.v(TAG, "hello world");
            sqliteManager = SQLiteManager.getInstance();
            startWatching();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Logger.v(TAG, "Window closing");
            stopAll();
        }

        private void startWatching()
        {
            statisticThread = new Thread(StatisticThread.ThreadProc);
            statisticThread.Start();
            Logger.v(TAG, "Statistic thread started");

            KMEventHook.InsertHook();
            Logger.v(TAG, "KM-event listening");
        }

        private void stopAll()
        {
            KMEventHook.RemoveHook();
            if (statisticThread != null)
                StatisticThread.CanThreadRun = false;
            Logger.v(TAG, "Hook removed");
        }
    }
}
