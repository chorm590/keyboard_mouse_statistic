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
            src.tool.Timer.StartTimer();
            TimeManager.TimeUsing = DateTime.Now;

            statisticManager = StatisticManager.GetInstance;
            startWatching();
            bindData();
        }

        private void bindData()
        {
            //全局键盘统计
            Binding binding = new Binding();
            binding.Source = statisticManager.GetRecord(Constants.TypeNumber.KEYBOARD_TOTAL);
            binding.Path = new PropertyPath("Desc");
            BindingOperations.SetBinding(KbTotal, TextBlock.TextProperty, binding);

            binding = new Binding();
            binding.Source = statisticManager.GetRecord(Constants.TypeNumber.KEYBOARD_COMBOL_TOTAL);
            binding.Path = new PropertyPath("Desc");
            BindingOperations.SetBinding(ComboTotal, TextBlock.TextProperty, binding);

            binding = new Binding();
            binding.Source = statisticManager.GetRecord(Constants.TypeNumber.KB_SK_TOP1);
            binding.Path = new PropertyPath("Desc");
            BindingOperations.SetBinding(SkTop1, TextBlock.TextProperty, binding);

            binding = new Binding();
            binding.Source = statisticManager.GetRecord(Constants.TypeNumber.KB_SK_TOP2);
            binding.Path = new PropertyPath("Desc");
            BindingOperations.SetBinding(SkTop2, TextBlock.TextProperty, binding);

            binding = new Binding();
            binding.Source = statisticManager.GetRecord(Constants.TypeNumber.KB_SK_TOP3);
            binding.Path = new PropertyPath("Desc");
            BindingOperations.SetBinding(SkTop3, TextBlock.TextProperty, binding);

            binding = new Binding();
            binding.Source = statisticManager.GetRecord(Constants.TypeNumber.KB_SK_TOP4);
            binding.Path = new PropertyPath("Desc");
            BindingOperations.SetBinding(SkTop4, TextBlock.TextProperty, binding);

            binding = new Binding();
            binding.Source = statisticManager.GetRecord(Constants.TypeNumber.KB_SK_TOP5);
            binding.Path = new PropertyPath("Desc");
            BindingOperations.SetBinding(SkTop5, TextBlock.TextProperty, binding);

            //全局鼠标统计
            binding = new Binding();
            binding.Source = statisticManager.GetRecord(Constants.TypeNumber.MOUSE_LEFT_BTN);
            binding.Path = new PropertyPath("Desc");
            BindingOperations.SetBinding(MsLeftBtn, TextBlock.TextProperty, binding);

            binding = new Binding();
            binding.Source = statisticManager.GetRecord(Constants.TypeNumber.MOUSE_RIGHT_BTN);
            binding.Path = new PropertyPath("Desc");
            BindingOperations.SetBinding(MsRightBtn, TextBlock.TextProperty, binding);

            binding = new Binding();
            binding.Source = statisticManager.GetRecord(Constants.TypeNumber.MOUSE_WHEEL_FORWARD);
            binding.Path = new PropertyPath("Desc");
            BindingOperations.SetBinding(MsWheelForward, TextBlock.TextProperty, binding);

            binding = new Binding();
            binding.Source = statisticManager.GetRecord(Constants.TypeNumber.MOUSE_WHEEL_BACKWARD);
            binding.Path = new PropertyPath("Desc");
            BindingOperations.SetBinding(MsWheelBackward, TextBlock.TextProperty, binding);

            //今日统计
            binding = new Binding();
            binding.Source = statisticManager.SttKeyboardTotalToday;
            binding.Path = new PropertyPath("Desc");
            BindingOperations.SetBinding(KbAllToday, TextBlock.TextProperty, binding);

            binding = new Binding();
            binding.Source = statisticManager.SttMouseTotalToday;
            binding.Path = new PropertyPath("Desc");
            BindingOperations.SetBinding(MsAllToday, TextBlock.TextProperty, binding);

            binding = new Binding();
            binding.Source = statisticManager.SttLetterTop1Today;
            binding.Path = new PropertyPath("Desc");
            BindingOperations.SetBinding(MostLetterToday, TextBlock.TextProperty, binding);

            binding = new Binding();
            binding.Source = statisticManager.SttMostOpHourToday;
            binding.Path = new PropertyPath("Desc");
            BindingOperations.SetBinding(MostOpHourToday, TextBlock.TextProperty, binding);
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
            src.tool.Timer.DestroyTimer();
        }
    }
}
