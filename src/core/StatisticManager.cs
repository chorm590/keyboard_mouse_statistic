using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using System.Threading;
using System.Windows;
using KMS.src.db;
using KMS.src.tool;

namespace KMS.src.core
{
    /// <summary>
    /// 统计信息管理。亦可简单理解成是事件的内存数据库。
    /// </summary>
    class StatisticManager
    {
        private const string TAG = "StatisticManager";

        private static StatisticManager instance;
        internal static StatisticManager GetInstance
        {
            get
            {
                if (instance == null)
                    instance = new StatisticManager();

                return instance;
            }
        }

        private readonly double screenAreaWidth;
        private readonly double screenAreaHeight;

        private readonly Dictionary<byte, ushort> opInHour; //今日各时段的操作记数。

        private readonly Statistic statisticGlobal;
        private readonly Statistic statisticYear;
        private readonly Statistic statisticMonth;
        private readonly Statistic statisticDay;
        private readonly Statistic statisticHour;

        internal Event SttKeyboardTotalToday
        {
            get;
        }

        internal Event SttMouseTotalToday
        {
            get;
        }

        internal Event SttLetterTop1Today //今日敲击最多的字母
        {
            get;
        }

        internal Event SttMostOpHourToday //今日操作键鼠最多的时间段
        {
            get;
        }

        private StatisticManager()
        {
            //Statistic of today
            SttKeyboardTotalToday = new Event()
            {
                Type = Constants.Statistic.KbTotalToday
            };

            SttMouseTotalToday = new Event()
            {
                Type = Constants.Statistic.MsTotalToday
            };

            SttLetterTop1Today = new Event()
            {
                Type = Constants.Statistic.KbLetterTop1Today
            };

            SttMostOpHourToday = new Event()
            {
                Type = Constants.Statistic.MostOpHourToday
            };

            opInHour = new Dictionary<byte, ushort>(24);

            //Initialize timer
            tool.Timer.RegisterTimerCallback(TimerCallback);

            screenAreaWidth = SystemParameters.PrimaryScreenWidth / 10;
            screenAreaHeight = SystemParameters.PrimaryScreenHeight / 10;
            Logger.v(TAG, "screen width:" + screenAreaWidth + ",height:" + screenAreaHeight);

            statisticGlobal = new Statistic();
            statisticYear = new Statistic();
            statisticMonth = new Statistic();
            statisticDay = new Statistic();
            statisticHour = new Statistic();

            if (SQLiteManager.GetInstance.Init())
            {
                QueryStatisticFromDb(SQLiteManager.GLOBAL_RECORD, statisticGlobal);
                if (statisticGlobal.KeyboardTotal.Value is 0)
                {
                    new Thread(() => {
                        DateTime niw = DateTime.Now;
                        Logger.v(TAG, "Init table begin, " + niw.Minute + ":" + niw.Second + "." + niw.Millisecond);

                        SQLiteManager.GetInstance.InitTable();
                        //year,month,day三张表的时间要初始化。
                        QueryStatisticFromDb(SQLiteManager.YEAR_RECORD, statisticYear);
                        QueryStatisticFromDb(SQLiteManager.YEAR_RECORD, statisticMonth);
                        QueryStatisticFromDb(SQLiteManager.YEAR_RECORD, statisticDay);

                        niw = DateTime.Now;
                        Logger.v(TAG, "Init table end, " + niw.Minute + ":" + niw.Second + "." + niw.Millisecond);
                    }).Start();
                }
                else
                {
                    //read statistic from year database
                    new Thread(()=> {
                        DateTime niw = DateTime.Now;
                        Logger.v(TAG, "Year statistic query begin, " + niw.Minute + ":" + niw.Second + "." + niw.Millisecond);

                        QueryStatisticFromDb(SQLiteManager.YEAR_RECORD, statisticYear);
                        QueryStatisticFromDb(SQLiteManager.YEAR_RECORD, statisticMonth);
                        QueryStatisticFromDb(SQLiteManager.YEAR_RECORD, statisticDay);

                        niw = DateTime.Now;
                        Logger.v(TAG, "Year statistic query end, " + niw.Minute + ":" + niw.Second + "." + niw.Millisecond);
                    }).Start();
                }
            }
            else
            {
                MessageBox.Show("Can't init the database");
                Application.Current.Shutdown();
            }
        }

        internal void Shutdown()
        {
            //TODO flush all data to disk.
        }

        private void TimerCallback(object state)
        {
            DateTime now = DateTime.Now;
            Logger.v(TAG, "storage data, now1, " + now.Minute + ":" + now.Second + "." + now.Millisecond);
            //storage the global record.
            bool globalTransaction = false;
            bool yearTransaction = false;
            if (statisticGlobal.KeyboardTotal.IsUpdated)
            {
                if (SQLiteManager.GetInstance.BeginTransaction(SQLiteManager.GLOBAL_RECORD))
                {
                    globalTransaction = true;
                    yearTransaction = SQLiteManager.GetInstance.BeginTransaction(SQLiteManager.YEAR_RECORD);

                    SQLiteManager.GetInstance.UpdateGlobal(Constants.TypeNumber.KEYBOARD_TOTAL, statisticGlobal.KeyboardTotal.Value);
                    statisticGlobal.KeyboardTotal.IsUpdated = false;

                    if (yearTransaction)
                    {
                        SQLiteManager.GetInstance.UpdateYear(Constants.TypeNumber.KEYBOARD_TOTAL, statisticYear.KeyboardTotal.Value,
                            (ushort)statisticYear.KeyboardTotal.Year);
                        SQLiteManager.GetInstance.UpdateMonth(Constants.TypeNumber.KEYBOARD_TOTAL, statisticYear.KeyboardTotal.Value,
                            (ushort)statisticMonth.KeyboardTotal.Year, statisticMonth.KeyboardTotal.Month);
                        SQLiteManager.GetInstance.UpdateDay(Constants.TypeNumber.KEYBOARD_TOTAL, statisticDay.KeyboardTotal.Value,
                            (ushort)statisticDay.KeyboardTotal.Year, statisticDay.KeyboardTotal.Month, statisticDay.KeyboardTotal.Day);
                    }
                    
                    if (statisticGlobal.KeyboardComboTotal.IsUpdated)
                    {
                        SQLiteManager.GetInstance.UpdateGlobal(Constants.TypeNumber.KEYBOARD_COMBOL_TOTAL, statisticGlobal.KeyboardComboTotal.Value);
                        statisticGlobal.KeyboardComboTotal.IsUpdated = false;
                        if (yearTransaction)
                        {
                            SQLiteManager.GetInstance.UpdateYear(Constants.TypeNumber.KEYBOARD_COMBOL_TOTAL, statisticYear.KeyboardComboTotal.Value,
                                (ushort)statisticYear.KeyboardComboTotal.Year);
                            SQLiteManager.GetInstance.UpdateMonth(Constants.TypeNumber.KEYBOARD_COMBOL_TOTAL, statisticYear.KeyboardComboTotal.Value,
                                (ushort)statisticMonth.KeyboardComboTotal.Year, statisticMonth.KeyboardComboTotal.Month);
                            SQLiteManager.GetInstance.UpdateDay(Constants.TypeNumber.KEYBOARD_COMBOL_TOTAL, statisticDay.KeyboardComboTotal.Value,
                                (ushort)statisticDay.KeyboardComboTotal.Year, statisticDay.KeyboardComboTotal.Month, statisticDay.KeyboardComboTotal.Day);
                        }
                    }

                    //键盘单键
                    List<Record> list = new List<Record>();
                    if (statisticGlobal.CopyKeys(list))
                    {
                        foreach (Record rco in list)
                        {
                            if (rco.IsUpdated)
                            {
                                SQLiteManager.GetInstance.UpdateGlobal(rco.Type, rco.Value);
                                rco.IsUpdated = false;
                                
                            }
                        }
                    }

                    if (yearTransaction)
                    {
                        if (statisticYear.CopyKeys(list))
                        {
                            foreach (Record rco in list)
                            {
                                if (rco.IsUpdated)
                                {
                                    SQLiteManager.GetInstance.UpdateYear(rco.Type, rco.Value, (ushort)rco.Year);
                                    rco.IsUpdated = false;
                                }
                            }
                        }

                        if (statisticMonth.CopyKeys(list))
                        {
                            foreach (Record rco in list)
                            {
                                if (rco.IsUpdated)
                                {
                                    SQLiteManager.GetInstance.UpdateMonth(rco.Type, rco.Value, (ushort)rco.Year, rco.Month);
                                    rco.IsUpdated = false;
                                }
                            }
                        }

                        if (statisticDay.CopyKeys(list))
                        {
                            foreach (Record rco in list)
                            {
                                if (rco.IsUpdated)
                                {
                                    SQLiteManager.GetInstance.UpdateDay(rco.Type, rco.Value, (ushort)rco.Year, rco.Month, rco.Day);
                                    rco.IsUpdated = false;
                                }
                            }
                        }
                    }
                }
            }

            if (statisticGlobal.MouseTotal.IsUpdated)
            {
                if (!globalTransaction)
                {
                    globalTransaction = SQLiteManager.GetInstance.BeginTransaction(SQLiteManager.GLOBAL_RECORD);
                }

                if(globalTransaction)
                {
                    if (!yearTransaction)
                        yearTransaction = SQLiteManager.GetInstance.BeginTransaction(SQLiteManager.YEAR_RECORD);

                    //存储鼠标事件。
                    SQLiteManager.GetInstance.UpdateGlobal(Constants.TypeNumber.MOUSE_TOTAL, statisticGlobal.MouseTotal.Value);
                    statisticGlobal.MouseTotal.IsUpdated = false;
                    SQLiteManager.GetInstance.UpdateYear(Constants.TypeNumber.MOUSE_TOTAL, statisticYear.MouseTotal.Value,
                        (ushort)statisticYear.MouseTotal.Year);
                    SQLiteManager.GetInstance.UpdateMonth(Constants.TypeNumber.MOUSE_TOTAL, statisticMonth.MouseTotal.Value,
                        (ushort)statisticMonth.MouseTotal.Year, statisticMonth.MouseTotal.Month);
                    SQLiteManager.GetInstance.UpdateDay(Constants.TypeNumber.MOUSE_TOTAL, statisticDay.MouseTotal.Value,
                        (ushort)statisticDay.MouseTotal.Year, statisticDay.MouseTotal.Month, statisticDay.MouseTotal.Day);

                    if (statisticGlobal.MouseLeftBtn.IsUpdated)
                    {
                        SQLiteManager.GetInstance.UpdateGlobal(Constants.TypeNumber.MOUSE_LEFT_BTN, statisticGlobal.MouseLeftBtn.Value);
                        statisticGlobal.MouseLeftBtn.IsUpdated = false;
                        if (yearTransaction)
                        {
                            SQLiteManager.GetInstance.UpdateYear(Constants.TypeNumber.MOUSE_LEFT_BTN, statisticYear.MouseLeftBtn.Value,
                                (ushort)statisticYear.MouseLeftBtn.Year);
                            SQLiteManager.GetInstance.UpdateMonth(Constants.TypeNumber.MOUSE_LEFT_BTN, statisticMonth.MouseLeftBtn.Value,
                                (ushort)statisticMonth.MouseLeftBtn.Year, statisticMonth.MouseLeftBtn.Month);
                            SQLiteManager.GetInstance.UpdateDay(Constants.TypeNumber.MOUSE_LEFT_BTN, statisticDay.MouseLeftBtn.Value,
                                (ushort)statisticDay.MouseLeftBtn.Year, statisticDay.MouseLeftBtn.Month, statisticDay.MouseLeftBtn.Day);
                        }
                    }

                    if (statisticGlobal.MouseRightBtn.IsUpdated)
                    {
                        SQLiteManager.GetInstance.UpdateGlobal(Constants.TypeNumber.MOUSE_RIGHT_BTN, statisticGlobal.MouseRightBtn.Value);
                        statisticGlobal.MouseRightBtn.IsUpdated = false;
                        if (yearTransaction)
                        {
                            SQLiteManager.GetInstance.UpdateYear(Constants.TypeNumber.MOUSE_RIGHT_BTN, statisticYear.MouseRightBtn.Value,
                                (ushort)statisticYear.MouseRightBtn.Year);
                            SQLiteManager.GetInstance.UpdateMonth(Constants.TypeNumber.MOUSE_RIGHT_BTN, statisticMonth.MouseRightBtn.Value,
                                (ushort)statisticMonth.MouseRightBtn.Year, statisticMonth.MouseRightBtn.Month);
                            SQLiteManager.GetInstance.UpdateDay(Constants.TypeNumber.MOUSE_RIGHT_BTN, statisticDay.MouseRightBtn.Value,
                                (ushort)statisticDay.MouseRightBtn.Year, statisticDay.MouseRightBtn.Month, statisticDay.MouseRightBtn.Day);
                        }
                    }

                    if (statisticGlobal.MouseWheelForward.IsUpdated)
                    {
                        SQLiteManager.GetInstance.UpdateGlobal(Constants.TypeNumber.MOUSE_WHEEL_FORWARD, statisticGlobal.MouseWheelForward.Value);
                        statisticGlobal.MouseWheelForward.IsUpdated = false;
                        if (yearTransaction)
                        {
                            SQLiteManager.GetInstance.UpdateYear(Constants.TypeNumber.MOUSE_WHEEL_FORWARD, statisticYear.MouseWheelForward.Value,
                                (ushort)statisticYear.MouseWheelForward.Year);
                            SQLiteManager.GetInstance.UpdateMonth(Constants.TypeNumber.MOUSE_WHEEL_FORWARD, statisticMonth.MouseWheelForward.Value,
                                (ushort)statisticMonth.MouseWheelForward.Year, statisticMonth.MouseWheelForward.Month);
                            SQLiteManager.GetInstance.UpdateDay(Constants.TypeNumber.MOUSE_WHEEL_FORWARD, statisticDay.MouseWheelForward.Value,
                                (ushort)statisticDay.MouseWheelForward.Year, statisticDay.MouseWheelForward.Month, statisticDay.MouseWheelForward.Day);
                        }
                    }

                    if (statisticGlobal.MouseWheelBackward.IsUpdated)
                    {
                        SQLiteManager.GetInstance.UpdateGlobal(Constants.TypeNumber.MOUSE_WHEEL_BACKWARD, statisticGlobal.MouseWheelBackward.Value);
                        statisticGlobal.MouseWheelBackward.IsUpdated = false;
                        if (yearTransaction)
                        {
                            SQLiteManager.GetInstance.UpdateYear(Constants.TypeNumber.MOUSE_WHEEL_BACKWARD, statisticYear.MouseWheelBackward.Value,
                                (ushort)statisticYear.MouseWheelBackward.Year);
                            SQLiteManager.GetInstance.UpdateMonth(Constants.TypeNumber.MOUSE_WHEEL_BACKWARD, statisticMonth.MouseWheelBackward.Value,
                                (ushort)statisticMonth.MouseWheelBackward.Year, statisticMonth.MouseWheelBackward.Month);
                            SQLiteManager.GetInstance.UpdateDay(Constants.TypeNumber.MOUSE_WHEEL_BACKWARD, statisticDay.MouseWheelBackward.Value,
                                (ushort)statisticDay.MouseWheelBackward.Year, statisticDay.MouseWheelBackward.Month, statisticDay.MouseWheelBackward.Day);
                        }
                    }

                    if (statisticGlobal.MouseSideKeyForward.IsUpdated)
                    {
                        SQLiteManager.GetInstance.UpdateGlobal(Constants.TypeNumber.MOUSE_SIDE_FORWARD, statisticGlobal.MouseSideKeyForward.Value);
                        statisticGlobal.MouseSideKeyForward.IsUpdated = false;
                        if (yearTransaction)
                        {
                            SQLiteManager.GetInstance.UpdateYear(Constants.TypeNumber.MOUSE_SIDE_FORWARD, statisticYear.MouseSideKeyForward.Value,
                                (ushort)statisticYear.MouseSideKeyForward.Year);
                            SQLiteManager.GetInstance.UpdateMonth(Constants.TypeNumber.MOUSE_SIDE_FORWARD, statisticMonth.MouseSideKeyForward.Value,
                                (ushort)statisticMonth.MouseSideKeyForward.Year, statisticMonth.MouseSideKeyForward.Month);
                            SQLiteManager.GetInstance.UpdateDay(Constants.TypeNumber.MOUSE_SIDE_FORWARD, statisticDay.MouseSideKeyForward.Value,
                                (ushort)statisticDay.MouseSideKeyForward.Year, statisticDay.MouseSideKeyForward.Month, statisticDay.MouseSideKeyForward.Day);
                        }
                    }

                    if (statisticGlobal.MouseSideKeyBackward.IsUpdated)
                    {
                        SQLiteManager.GetInstance.UpdateGlobal(Constants.TypeNumber.MOUSE_SIDE_BACKWARD, statisticGlobal.MouseSideKeyBackward.Value);
                        statisticGlobal.MouseSideKeyBackward.IsUpdated = false;
                        if (yearTransaction)
                        {
                            SQLiteManager.GetInstance.UpdateYear(Constants.TypeNumber.MOUSE_SIDE_BACKWARD, statisticYear.MouseSideKeyBackward.Value,
                                (ushort)statisticYear.MouseSideKeyBackward.Year);
                            SQLiteManager.GetInstance.UpdateMonth(Constants.TypeNumber.MOUSE_SIDE_BACKWARD, statisticMonth.MouseSideKeyBackward.Value,
                                (ushort)statisticMonth.MouseSideKeyBackward.Year, statisticMonth.MouseSideKeyBackward.Month);
                            SQLiteManager.GetInstance.UpdateDay(Constants.TypeNumber.MOUSE_SIDE_BACKWARD, statisticDay.MouseSideKeyBackward.Value,
                                (ushort)statisticDay.MouseSideKeyBackward.Year, statisticDay.MouseSideKeyBackward.Month, statisticDay.MouseSideKeyBackward.Day);
                        }
                    }
                }
            }

            if(globalTransaction)
                SQLiteManager.GetInstance.CommitTransaction(SQLiteManager.GLOBAL_RECORD);
            if (yearTransaction)
                SQLiteManager.GetInstance.CommitTransaction(SQLiteManager.YEAR_RECORD);

            now = DateTime.Now;
            Logger.v(TAG, "storage data,now2, " + now.Minute + ":" + now.Second + "." + now.Millisecond);
        }

        internal void KeyboardEventHappen(int typeCode, byte fkey, DateTime time)
        {
            // This function should only be call by CountThread with sub-thread.
            //TODO 窗体没在前台运行的时候不要实时刷新统计数据，不要实时重新排序。

            if (typeCode < 1)
            {
                // Do nothing
            }
            else if (typeCode < 256)
            {
                statisticGlobal.KeyboardTotal.Value++;
                statisticGlobal.KeyboardTotal.Desc = GetDesc1(statisticGlobal.KeyboardTotal.Value);

                statisticYear.KeyboardTotal.Value++;
                statisticMonth.KeyboardTotal.Value++;
                statisticDay.KeyboardTotal.Value++;

                if (fkey > 0)
                {
                    statisticGlobal.KeyboardComboTotal.Value++;
                    statisticGlobal.KeyboardComboTotal.Desc = GetDesc1(statisticGlobal.KeyboardComboTotal.Value);

                    statisticYear.KeyboardComboTotal.Value++;
                    statisticMonth.KeyboardComboTotal.Value++;
                    statisticDay.KeyboardComboTotal.Value++;
                }

                SttKeyboardTotalToday.Value++;
                SttKeyboardTotalToday.Desc = GetDesc1(SttKeyboardTotalToday.Value);

                UpdateOpInHour(time.Hour, 1);
                KbSingleKeyPressed(typeCode);
            }
            else
            {
                // Do nothing
            }
        }

        internal void MouseEventHappen(int typeCode, int mouseData, short x, short y, DateTime time)
        {
            switch (typeCode)
            {
                case Constants.TypeNumber.MOUSE_WHEEL_BACKWARD:
                    statisticGlobal.MouseWheelBackward.Value += (ushort)mouseData;
                    statisticGlobal.MouseWheelBackward.Desc = GetDesc1(statisticGlobal.MouseWheelBackward.Value);
                    statisticGlobal.MouseTotal.Value += (ushort)mouseData;
                    statisticYear.MouseWheelBackward.Value += (ushort)mouseData;
                    statisticYear.MouseTotal.Value += (ushort)mouseData;
                    statisticMonth.MouseWheelBackward.Value += (ushort)mouseData;
                    statisticMonth.MouseTotal.Value += (ushort)mouseData;
                    statisticDay.MouseWheelBackward.Value += (ushort)mouseData;
                    statisticDay.MouseTotal.Value += (ushort)mouseData;
                    break;
                case Constants.TypeNumber.MOUSE_WHEEL_FORWARD:
                    statisticGlobal.MouseWheelForward.Value += (ushort)mouseData;
                    statisticGlobal.MouseWheelForward.Desc = GetDesc1(statisticGlobal.MouseWheelForward.Value);
                    statisticGlobal.MouseTotal.Value += (ushort)mouseData;
                    statisticYear.MouseWheelForward.Value += (ushort)mouseData;
                    statisticYear.MouseTotal.Value += (ushort)mouseData;
                    statisticMonth.MouseWheelForward.Value += (ushort)mouseData;
                    statisticMonth.MouseTotal.Value += (ushort)mouseData;
                    statisticDay.MouseWheelForward.Value += (ushort)mouseData;
                    statisticDay.MouseTotal.Value += (ushort)mouseData;
                    break;
                case Constants.TypeNumber.MOUSE_WHEEL_CLICK:
                    statisticGlobal.MouseWheelClick.Value++;
                    statisticGlobal.MouseWheelClick.Desc = GetDesc1(statisticGlobal.MouseWheelClick.Value);
                    statisticGlobal.MouseTotal.Value++;
                    statisticYear.MouseWheelClick.Value++;
                    statisticYear.MouseTotal.Value++;
                    statisticMonth.MouseWheelClick.Value++;
                    statisticMonth.MouseTotal.Value++;
                    statisticDay.MouseWheelClick.Value++;
                    statisticDay.MouseTotal.Value++;
                    break;
                case Constants.TypeNumber.MOUSE_LEFT_BTN:
                    statisticGlobal.MouseLeftBtn.Value++;
                    statisticGlobal.MouseLeftBtn.Desc = GetDesc1(statisticGlobal.MouseLeftBtn.Value);
                    statisticGlobal.MouseTotal.Value++;
                    statisticYear.MouseLeftBtn.Value++;
                    statisticYear.MouseTotal.Value++;
                    statisticMonth.MouseLeftBtn.Value++;
                    statisticMonth.MouseTotal.Value++;
                    statisticDay.MouseLeftBtn.Value++;
                    statisticDay.MouseTotal.Value++;
                    break;
                case Constants.TypeNumber.MOUSE_RIGHT_BTN:
                    statisticGlobal.MouseRightBtn.Value++;
                    statisticGlobal.MouseRightBtn.Desc = GetDesc1(statisticGlobal.MouseRightBtn.Value);
                    statisticGlobal.MouseTotal.Value++;
                    statisticYear.MouseRightBtn.Value++;
                    statisticYear.MouseTotal.Value++;
                    statisticMonth.MouseRightBtn.Value++;
                    statisticMonth.MouseTotal.Value++;
                    statisticDay.MouseRightBtn.Value++;
                    statisticDay.MouseTotal.Value++;
                    break;
                case Constants.TypeNumber.MOUSE_SIDE_FORWARD:
                    statisticGlobal.MouseSideKeyForward.Value++;
                    statisticGlobal.MouseSideKeyForward.Desc = GetDesc1(statisticGlobal.MouseSideKeyForward.Value);
                    statisticGlobal.MouseTotal.Value++;
                    statisticYear.MouseSideKeyForward.Value++;
                    statisticYear.MouseTotal.Value++;
                    statisticMonth.MouseSideKeyForward.Value++;
                    statisticMonth.MouseTotal.Value++;
                    statisticDay.MouseSideKeyForward.Value++;
                    statisticDay.MouseTotal.Value++;
                    break;
                case Constants.TypeNumber.MOUSE_SIDE_BACKWARD:
                    statisticGlobal.MouseSideKeyBackward.Value++;
                    statisticGlobal.MouseSideKeyBackward.Desc = GetDesc1(statisticGlobal.MouseSideKeyBackward.Value);
                    statisticGlobal.MouseTotal.Value++;
                    statisticYear.MouseSideKeyBackward.Value++;
                    statisticYear.MouseTotal.Value++;
                    statisticMonth.MouseSideKeyBackward.Value++;
                    statisticMonth.MouseTotal.Value++;
                    statisticDay.MouseSideKeyBackward.Value++;
                    statisticDay.MouseTotal.Value++;
                    break;
                default:
                    break;
            }

            switch (typeCode)
            {
                case Constants.TypeNumber.MOUSE_WHEEL_BACKWARD:
                case Constants.TypeNumber.MOUSE_WHEEL_FORWARD:
                    SttMouseTotalToday.Value += (ushort)mouseData;
                    UpdateOpInHour(time.Hour, mouseData);
                    break;
                case Constants.TypeNumber.MOUSE_WHEEL_CLICK:
                case Constants.TypeNumber.MOUSE_LEFT_BTN:
                case Constants.TypeNumber.MOUSE_RIGHT_BTN:
                case Constants.TypeNumber.MOUSE_SIDE_FORWARD:
                case Constants.TypeNumber.MOUSE_SIDE_BACKWARD:
                    SttMouseTotalToday.Value++;
                    UpdateOpInHour(time.Hour, 1);
                    break;
                default:
                    break;
            }

            SttMouseTotalToday.Desc = GetDesc1(SttMouseTotalToday.Value);
        }

        private void KbSingleKeyPressed(int keycode)
        {
            SingleKeyRecord(statisticGlobal.KeyboardKeys, (byte)keycode);
            SingleKeyRecord(statisticYear.KeyboardKeys, (byte)keycode);
            SingleKeyRecord(statisticMonth.KeyboardKeys, (byte)keycode);
            SingleKeyRecord(statisticDay.KeyboardKeys, (byte)keycode);

            statisticGlobal.SortKeys();
            if (statisticGlobal.KeyboardKeys[0].Value > 0)
            {
                statisticGlobal.KeyboardSkTop1.Desc = GetDesc2(Constants.Keyboard[(byte)(statisticGlobal.KeyboardKeys[0].Type)].DisplayName, statisticGlobal.KeyboardKeys[0].Value);
            }

            if (statisticGlobal.KeyboardKeys[1].Value > 0)
            {
                statisticGlobal.KeyboardSkTop2.Desc = GetDesc2(Constants.Keyboard[(byte)(statisticGlobal.KeyboardKeys[1].Type)].DisplayName, statisticGlobal.KeyboardKeys[1].Value);
            }

            if (statisticGlobal.KeyboardKeys[2].Value > 0)
            {
                statisticGlobal.KeyboardSkTop3.Desc = GetDesc2(Constants.Keyboard[(byte)(statisticGlobal.KeyboardKeys[2].Type)].DisplayName, statisticGlobal.KeyboardKeys[2].Value);
            }

            if (statisticGlobal.KeyboardKeys[3].Value > 0)
            {
                statisticGlobal.KeyboardSkTop4.Desc = GetDesc2(Constants.Keyboard[(byte)(statisticGlobal.KeyboardKeys[3].Type)].DisplayName, statisticGlobal.KeyboardKeys[3].Value);
            }

            if (statisticGlobal.KeyboardKeys[4].Value > 0)
            {
                statisticGlobal.KeyboardSkTop5.Desc = GetDesc2(Constants.Keyboard[(byte)(statisticGlobal.KeyboardKeys[4].Type)].DisplayName, statisticGlobal.KeyboardKeys[4].Value);
            }

            //Find the most typed letter
            foreach (Record rco in statisticGlobal.KeyboardKeys)
            {
                if (rco.Type >= Constants.TypeNumber.A && rco.Type <= Constants.TypeNumber.Z)
                {
                    SttLetterTop1Today.Value = rco.Value;
                    SttLetterTop1Today.Desc = GetDesc2(Constants.Keyboard[(byte)(rco.Type)].DisplayName, rco.Value);
                    break;
                }
            }
        }

        private void SingleKeyRecord(List<Record> list, byte keycode)
        {
            if (list == null)
                return;

            foreach (Record rco in list)
            {
                if (rco.Type == keycode)
                {
                    rco.Value++;
                    break;
                }
            }
        }

        /// <summary>
        /// 更新今日统计中的各时段操作总数。
        /// </summary>
        private void UpdateOpInHour(int hour, int value)
        {
            ushort value2;
            ushort maxValue = 0;
            ushort maxHour = 0;
            if (opInHour.TryGetValue((byte)hour, out value2))
            {
                value += value2;
                opInHour.Remove((byte)hour);
            }

            opInHour.Add((byte)hour, (ushort)value);

            //Find the hour which has most op
            for (byte i = 0; i < 24; i++) //Must iterate from head to tail?
            {
                if (opInHour.TryGetValue(i, out value2))
                {
                    if (value2 > maxValue)
                    {
                        maxValue = value2;
                        maxHour = i;
                    }
                }
            }

            if (maxValue > 0)
            {
                SttMostOpHourToday.Desc = maxHour + " 时[" + maxValue + " 次]";
            }
        }

        /// <summary>
        /// 从数据库中读取统计数据。
        /// </summary>
        private void QueryStatisticFromDb(byte which, Statistic statistic)
        {
            SQLiteDataReader reader = null;
            if (which == SQLiteManager.GLOBAL_RECORD)
            {
                reader = SQLiteManager.GetInstance.QueryGlobalStatistic();
            }
            else if (which == SQLiteManager.YEAR_RECORD)
            {
                if (statistic == statisticYear)
                    reader = SQLiteManager.GetInstance.QueryYearStatistic();
                else if(statistic == statisticMonth)
                    reader = SQLiteManager.GetInstance.QueryMonthStatistic();
                else if (statistic == statisticDay)
                    reader = SQLiteManager.GetInstance.QueryDayStatistic();
            }

            if (reader is null)
            {
                Logger.v(TAG, "No statistic record found");
                return;
            }

            short year = 0;
            byte month = 0;
            byte day = 0;
            byte hour = 0;
            ushort type;
            uint value;
            bool noRecord = true;
            while (reader.Read())
            {
                noRecord = false;
                type = (ushort)reader.GetInt16(0);
                value = (ushort)reader.GetInt16(1);
                if (statistic == statisticYear)
                {
                    year = reader.GetInt16(2);
                }
                else if (statistic == statisticMonth)
                {
                    year = reader.GetInt16(2);
                    month = reader.GetByte(3);
                }
                else if (statistic == statisticDay)
                {
                    year = reader.GetInt16(2);
                    month = reader.GetByte(3);
                    day = reader.GetByte(4);
                }

                if (type > 0 && type < 256)
                {
                    foreach (Record rco in statistic.KeyboardKeys)
                    {
                        if (rco.Type == type)
                        {
                            rco.Value += value;
                            rco.IsUpdated = false;
                            rco.Year = year;
                            rco.Month = month;
                            rco.Day = day;
                            break;
                        }
                    }
                }
                else
                {
                    switch (type)
                    {
                        case Constants.TypeNumber.KEYBOARD_TOTAL:
                            statistic.KeyboardTotal.Value += value;
                            statistic.KeyboardTotal.IsUpdated = false;
                            statistic.KeyboardTotal.Year = year;
                            statistic.KeyboardTotal.Month = month;
                            statistic.KeyboardTotal.Day = day;
                            break;
                        case Constants.TypeNumber.KEYBOARD_COMBOL_TOTAL:
                            statistic.KeyboardComboTotal.Value += value;
                            statistic.KeyboardComboTotal.IsUpdated = false;
                            statistic.KeyboardComboTotal.Year = year;
                            statistic.KeyboardComboTotal.Month = month;
                            statistic.KeyboardComboTotal.Day = day;
                            break;
                        case Constants.TypeNumber.MOUSE_TOTAL:
                            statistic.MouseTotal.Value += value;
                            statistic.MouseTotal.IsUpdated = false;
                            statistic.MouseTotal.Year = year;
                            statistic.MouseTotal.Month = month;
                            statistic.MouseTotal.Day = day;
                            break;
                        case Constants.TypeNumber.MOUSE_LEFT_BTN:
                            statistic.MouseLeftBtn.Value += value;
                            statistic.MouseLeftBtn.IsUpdated = false;
                            statistic.MouseLeftBtn.Year = year;
                            statistic.MouseLeftBtn.Month = month;
                            statistic.MouseLeftBtn.Day = day;
                            break;
                        case Constants.TypeNumber.MOUSE_RIGHT_BTN:
                            statistic.MouseRightBtn.Value += value;
                            statistic.MouseRightBtn.IsUpdated = false;
                            statistic.MouseRightBtn.Year = year;
                            statistic.MouseRightBtn.Month = month;
                            statistic.MouseRightBtn.Day = day;
                            break;
                        case Constants.TypeNumber.MOUSE_WHEEL_FORWARD:
                            statistic.MouseWheelForward.Value += value;
                            statistic.MouseWheelForward.IsUpdated = false;
                            statistic.MouseWheelForward.Year = year;
                            statistic.MouseWheelForward.Month = month;
                            statistic.MouseWheelForward.Day = day;
                            break;
                        case Constants.TypeNumber.MOUSE_WHEEL_BACKWARD:
                            statistic.MouseWheelBackward.Value += value;
                            statistic.MouseWheelBackward.IsUpdated = false;
                            statistic.MouseWheelBackward.Year = year;
                            statistic.MouseWheelBackward.Month = month;
                            statistic.MouseWheelBackward.Day = day;
                            break;
                        case Constants.TypeNumber.MOUSE_SIDE_FORWARD:
                            statistic.MouseSideKeyForward.Value += value;
                            statistic.MouseSideKeyForward.IsUpdated = false;
                            statistic.MouseSideKeyForward.Year = year;
                            statistic.MouseSideKeyForward.Month = month;
                            statistic.MouseSideKeyForward.Day = day;
                            break;
                        case Constants.TypeNumber.MOUSE_SIDE_BACKWARD:
                            statistic.MouseSideKeyBackward.Value += value;
                            statistic.MouseSideKeyBackward.IsUpdated = false;
                            statistic.MouseSideKeyBackward.Year = year;
                            statistic.MouseSideKeyBackward.Month = month;
                            statistic.MouseSideKeyBackward.Day = day;
                            break;
                    }
                }
            }
            reader.Close();

            if (noRecord)
            {
                Logger.v(TAG, "No statistic record found2");
                return;
            }
            else if (statistic.KeyboardTotal.Value is 0 && statistic.MouseTotal.Value is 0)
            {
                Logger.v(TAG, "Empty record");
                return;
            }

            if (statistic == statisticGlobal)
            {
                statistic.KeyboardTotal.Desc = GetDesc1(statistic.KeyboardTotal.Value);
                statistic.KeyboardComboTotal.Desc = GetDesc1(statistic.KeyboardComboTotal.Value);
                statistic.MouseTotal.Desc = GetDesc1(statistic.MouseTotal.Value);
                statistic.SortKeys();
                statistic.KeyboardSkTop1.Desc = GetDesc2(Constants.Keyboard[(byte)statistic.KeyboardKeys[0].Type].DisplayName, statistic.KeyboardKeys[0].Value);
                statistic.KeyboardSkTop2.Desc = GetDesc2(Constants.Keyboard[(byte)statistic.KeyboardKeys[1].Type].DisplayName, statistic.KeyboardKeys[1].Value);
                statistic.KeyboardSkTop3.Desc = GetDesc2(Constants.Keyboard[(byte)statistic.KeyboardKeys[2].Type].DisplayName, statistic.KeyboardKeys[2].Value);
                statistic.KeyboardSkTop4.Desc = GetDesc2(Constants.Keyboard[(byte)statistic.KeyboardKeys[3].Type].DisplayName, statistic.KeyboardKeys[3].Value);
                statistic.KeyboardSkTop5.Desc = GetDesc2(Constants.Keyboard[(byte)statistic.KeyboardKeys[4].Type].DisplayName, statistic.KeyboardKeys[4].Value);

                statistic.MouseLeftBtn.Desc = GetDesc1(statistic.MouseLeftBtn.Value);
                statistic.MouseRightBtn.Desc = GetDesc1(statistic.MouseRightBtn.Value);
                statistic.MouseWheelForward.Desc = GetDesc1(statistic.MouseWheelForward.Value);
                statistic.MouseWheelBackward.Desc = GetDesc1(statistic.MouseWheelBackward.Value);
            }
        }

        private string GetDesc1(long value)
        {
            return value + " 次";
        }

        private string GetDesc2(string value1, long value2)
        {
            return value1 + " [" + value2 + " 次]";
        }

        internal Record GetRecord(int type)
        {
            switch (type)
            {
                case Constants.TypeNumber.KEYBOARD_TOTAL:
                    return statisticGlobal.KeyboardTotal;
                case Constants.TypeNumber.KEYBOARD_COMBOL_TOTAL:
                    return statisticGlobal.KeyboardComboTotal;
                case Constants.TypeNumber.KB_SK_TOP1:
                    return statisticGlobal.KeyboardSkTop1;
                case Constants.TypeNumber.KB_SK_TOP2:
                    return statisticGlobal.KeyboardSkTop2;
                case Constants.TypeNumber.KB_SK_TOP3:
                    return statisticGlobal.KeyboardSkTop3;
                case Constants.TypeNumber.KB_SK_TOP4:
                    return statisticGlobal.KeyboardSkTop4;
                case Constants.TypeNumber.KB_SK_TOP5:
                    return statisticGlobal.KeyboardSkTop5;
                case Constants.TypeNumber.MOUSE_LEFT_BTN:
                    return statisticGlobal.MouseLeftBtn;
                case Constants.TypeNumber.MOUSE_RIGHT_BTN:
                    return statisticGlobal.MouseRightBtn;
                case Constants.TypeNumber.MOUSE_WHEEL_FORWARD:
                    return statisticGlobal.MouseWheelForward;
                case Constants.TypeNumber.MOUSE_WHEEL_BACKWARD:
                    return statisticGlobal.MouseWheelBackward;
            }

            return null;
        }

        private class Statistic
        {
            internal const byte IDLE = 0;
            internal const byte SORT1 = 1;
            internal const byte SORT2 = 2;
            internal const byte COPY1 = 3;
            internal const byte COPY2 = 4;

            internal byte ListStatus;

            internal Record KeyboardTotal { get; }
            internal Record KeyboardComboTotal { get; }
            internal Record KeyboardSkTop1 { get; }
            internal Record KeyboardSkTop2 { get; }
            internal Record KeyboardSkTop3 { get; }
            internal Record KeyboardSkTop4 { get; }
            internal Record KeyboardSkTop5 { get; }
            internal Record MouseTotal { get; }
            internal Record MouseLeftBtn { get; }
            internal Record MouseRightBtn { get; }
            internal Record MouseWheelForward { get; }
            internal Record MouseWheelBackward { get; }
            internal Record MouseWheelClick { get; }
            internal Record MouseSideKeyBackward { get; }
            internal Record MouseSideKeyForward { get; }
            internal List<Record> KeyboardKeys;

            internal Statistic()
            {
                KeyboardTotal = new Record
                {
                    Type = Constants.TypeNumber.KEYBOARD_TOTAL,
                    Year = (short)TimeManager.TimeUsing.Year,
                    Month = (byte)TimeManager.TimeUsing.Month,
                    Day = (byte)TimeManager.TimeUsing.Day
                };

                KeyboardComboTotal = new Record
                {
                    Type = Constants.TypeNumber.KEYBOARD_COMBOL_TOTAL,
                    Year = (short)TimeManager.TimeUsing.Year,
                    Month = (byte)TimeManager.TimeUsing.Month,
                    Day = (byte)TimeManager.TimeUsing.Day
                };

                KeyboardSkTop1 = new Record
                {
                    Type = Constants.TypeNumber.KB_SK_TOP1,
                    Year = (short)TimeManager.TimeUsing.Year,
                    Month = (byte)TimeManager.TimeUsing.Month,
                    Day = (byte)TimeManager.TimeUsing.Day
                };

                KeyboardSkTop2 = new Record
                {
                    Type = Constants.TypeNumber.KB_SK_TOP2,
                    Year = (short)TimeManager.TimeUsing.Year,
                    Month = (byte)TimeManager.TimeUsing.Month,
                    Day = (byte)TimeManager.TimeUsing.Day
                };

                KeyboardSkTop3 = new Record
                {
                    Type = Constants.TypeNumber.KB_SK_TOP3,
                    Year = (short)TimeManager.TimeUsing.Year,
                    Month = (byte)TimeManager.TimeUsing.Month,
                    Day = (byte)TimeManager.TimeUsing.Day
                };

                KeyboardSkTop4 = new Record
                {
                    Type = Constants.TypeNumber.KB_SK_TOP4,
                    Year = (short)TimeManager.TimeUsing.Year,
                    Month = (byte)TimeManager.TimeUsing.Month,
                    Day = (byte)TimeManager.TimeUsing.Day
                };

                KeyboardSkTop5 = new Record
                {
                    Type = Constants.TypeNumber.KB_SK_TOP5,
                    Year = (short)TimeManager.TimeUsing.Year,
                    Month = (byte)TimeManager.TimeUsing.Month,
                    Day = (byte)TimeManager.TimeUsing.Day
                };

                Dictionary<byte, Key> sgKey = Constants.Keyboard;
                KeyboardKeys = new List<Record>(sgKey.Count);
                Dictionary<byte, Key>.ValueCollection values = sgKey.Values;
                foreach (Key key in values)
                {
                    KeyboardKeys.Add(new Record((ushort)key.Code)
                        {
                            Year = (short)TimeManager.TimeUsing.Year,
                            Month = (byte)TimeManager.TimeUsing.Month,
                            Day = (byte)TimeManager.TimeUsing.Day
                        });
                }

                MouseTotal = new Record
                {
                    Type = Constants.TypeNumber.MOUSE_TOTAL,
                    Year = (short)TimeManager.TimeUsing.Year,
                    Month = (byte)TimeManager.TimeUsing.Month,
                    Day = (byte)TimeManager.TimeUsing.Day
                };

                MouseLeftBtn = new Record
                {
                    Type = Constants.TypeNumber.MOUSE_LEFT_BTN,
                    Year = (short)TimeManager.TimeUsing.Year,
                    Month = (byte)TimeManager.TimeUsing.Month,
                    Day = (byte)TimeManager.TimeUsing.Day
                };

                MouseRightBtn = new Record
                {
                    Type = Constants.TypeNumber.MOUSE_RIGHT_BTN,
                    Year = (short)TimeManager.TimeUsing.Year,
                    Month = (byte)TimeManager.TimeUsing.Month,
                    Day = (byte)TimeManager.TimeUsing.Day
                };

                MouseWheelForward = new Record
                {
                    Type = Constants.TypeNumber.MOUSE_WHEEL_FORWARD,
                    Year = (short)TimeManager.TimeUsing.Year,
                    Month = (byte)TimeManager.TimeUsing.Month,
                    Day = (byte)TimeManager.TimeUsing.Day
                };

                MouseWheelBackward = new Record
                {
                    Type = Constants.TypeNumber.MOUSE_WHEEL_BACKWARD,
                    Year = (short)TimeManager.TimeUsing.Year,
                    Month = (byte)TimeManager.TimeUsing.Month,
                    Day = (byte)TimeManager.TimeUsing.Day
                };

                MouseWheelClick = new Record
                {
                    Type = Constants.TypeNumber.MOUSE_WHEEL_CLICK,
                    Year = (short)TimeManager.TimeUsing.Year,
                    Month = (byte)TimeManager.TimeUsing.Month,
                    Day = (byte)TimeManager.TimeUsing.Day
                };

                MouseSideKeyForward = new Record
                {
                    Type = Constants.TypeNumber.MOUSE_SIDE_FORWARD,
                    Year = (short)TimeManager.TimeUsing.Year,
                    Month = (byte)TimeManager.TimeUsing.Month,
                    Day = (byte)TimeManager.TimeUsing.Day
                };

                MouseSideKeyBackward = new Record
                {
                    Type = Constants.TypeNumber.MOUSE_SIDE_BACKWARD,
                    Year = (short)TimeManager.TimeUsing.Year,
                    Month = (byte)TimeManager.TimeUsing.Month,
                    Day = (byte)TimeManager.TimeUsing.Day
                };
            }

            internal void SortKeys()
            {
                if (ListStatus == IDLE)
                    ListStatus = SORT1;

                if (ListStatus == SORT1)
                    ListStatus = SORT2;

                if (ListStatus == SORT2)
                {
                    KeyboardKeys.Sort();
                }

                ListStatus = IDLE;
            }

            internal bool CopyKeys(List<Record> dest)
            {
                if (dest is null)
                    return false;
                byte counter = 0;

                HELLO:
                if (ListStatus == IDLE)
                {
                    ListStatus = COPY1;
                }
                else
                {
                    if (counter < 3)
                    {
                        Thread.Sleep(30);
                        counter++;
                        goto HELLO;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (ListStatus == COPY1)
                {
                    ListStatus = COPY2;
                }
                else
                {
                    return false;
                }

                if (ListStatus == COPY2)
                {
                    dest.Clear();
                    dest.AddRange(KeyboardKeys);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
