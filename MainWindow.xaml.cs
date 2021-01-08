using System;
using System.Threading;
using System.Windows;
using KMS.src.core;
using KMS.src.tool;
using KMS.src.db;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Data;
using System.Windows.Controls;

namespace KMS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private const string TAG = "KMS";

        private Thread countThread;
        private StatisticManager statisticManager;

        public MainWindow()
        {
            InitializeComponent();
            Logger.v(TAG, "hello world");
            TimeManager.TimeUsing = DateTime.Now;

            statisticManager = StatisticManager.GetInstance;
            startWatching();
            bindData();
        }

        private void bindData()
        {
            Binding binding = new Binding();
            binding.Source = statisticManager.SttKeyboardTotal;
            binding.Path = new PropertyPath("Value");
            BindingOperations.SetBinding(KbTotal, TextBlock.TextProperty, binding);

            binding = new Binding();
            binding.Source = statisticManager.SttComboKeyTotal;
            binding.Path = new PropertyPath("Value");
            BindingOperations.SetBinding(ComboTotal, TextBlock.TextProperty, binding);
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
            countThread = new Thread(CountThread.ThreadProc);
            countThread.Start();
            Logger.v(TAG, "Count thread started");

            KMEventHook.InsertHook();
            Logger.v(TAG, "KM-event listening");
        }

        private void stopAll()
        {
            KMEventHook.RemoveHook();
            if (countThread != null)
                CountThread.CanThreadRun = false;
            Logger.v(TAG, "Hook removed");
        }
    }
}
