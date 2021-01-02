using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

        private Thread statisticThread;

        public MainWindow()
        {
            InitializeComponent();
            Logger.v(TAG, "hello world");
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            statisticThread = new Thread(StatisticThread.ThreadProc);
            statisticThread.Start();
            Logger.v(TAG, "Statistic thread started");

            KMEventHook.InsertHook();
            Logger.v(TAG, "KM-event listening");

            Binding bding = new Binding();
            bding.Source = StatisticThread.statisticResult;
            bding.Path = new PropertyPath("EventAmount");
            BindingOperations.SetBinding(tbStr, TextBlock.TextProperty, bding);
        }

        private void Select_Click(object sender, RoutedEventArgs e)
        {
            if (statisticThread != null)
                StatisticThread.CanThreadRun = false;
            KMEventHook.RemoveHook();
            Logger.v(TAG, "Hook removed");
        }
    }
}
