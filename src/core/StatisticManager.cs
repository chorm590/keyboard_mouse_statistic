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
                if (SQLiteManager.GetInstance.IsGlobalTableExists())
                {
                    QueryStatisticFromDb();
                }
                else
                {
                    if (!InitDatabase())
                    {
                        MessageBox.Show("Can't init the database");
                        Application.Current.Shutdown();
                    }
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
            Logger.v(TAG, "TimerCallback,now1, " + now.Minute + ":" + now.Second + "." + now.Millisecond);
            //storage the global record.
            if (SQLiteManager.GetInstance.BeginTransaction(SQLiteManager.GLOBAL_RECORD))
            {
                if (statisticGlobal.KeyboardTotal.IsUpdated)
                {
                    SQLiteManager.GetInstance.UpdateGlobal(statisticGlobal.KeyboardTotal.Type, statisticGlobal.KeyboardTotal.Value);
                    statisticGlobal.KeyboardTotal.IsUpdated = false;

                    if (statisticGlobal.KeyboardComboTotal.IsUpdated)
                    {
                        SQLiteManager.GetInstance.UpdateGlobal(statisticGlobal.KeyboardComboTotal.Type, statisticGlobal.KeyboardComboTotal.Value);
                        statisticGlobal.KeyboardComboTotal.IsUpdated = false;
                    }

                    if (statisticGlobal.MouseLeftBtn.IsUpdated)
                    {
                        SQLiteManager.GetInstance.UpdateGlobal(statisticGlobal.MouseLeftBtn.Type, statisticGlobal.MouseLeftBtn.Value);
                        statisticGlobal.MouseLeftBtn.IsUpdated = false;
                    }

                    if(statisticGlobal.MouseRightBtn.IsUpdated)
                    {
                        SQLiteManager.GetInstance.UpdateGlobal(statisticGlobal.MouseRightBtn.Type, statisticGlobal.MouseRightBtn.Value);
                        statisticGlobal.MouseRightBtn.IsUpdated = false;
                    }

                    if(statisticGlobal.MouseWheelForward.IsUpdated)
                    {
                        SQLiteManager.GetInstance.UpdateGlobal(statisticGlobal.MouseWheelForward.Type, statisticGlobal.MouseWheelForward.Value);
                        statisticGlobal.MouseWheelForward.IsUpdated = false;
                    }

                    if (statisticGlobal.MouseWheelBackward.IsUpdated)
                    {
                        SQLiteManager.GetInstance.UpdateGlobal(statisticGlobal.MouseWheelBackward.Type, statisticGlobal.MouseWheelBackward.Value);
                        statisticGlobal.MouseWheelBackward.IsUpdated = false;
                    }

                    if (statisticGlobal.MouseSideKeyForward.IsUpdated)
                    {
                        SQLiteManager.GetInstance.UpdateGlobal(statisticGlobal.MouseSideKeyForward.Type, statisticGlobal.MouseSideKeyForward.Value);
                        statisticGlobal.MouseSideKeyForward.IsUpdated = false;
                    }

                    if (statisticGlobal.MouseSideKeyBackward.IsUpdated)
                    {
                        SQLiteManager.GetInstance.UpdateGlobal(statisticGlobal.MouseSideKeyBackward.Type, statisticGlobal.MouseSideKeyBackward.Value);
                        statisticGlobal.MouseSideKeyBackward.IsUpdated = false;
                    }

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

                    SQLiteManager.GetInstance.CommitTransaction(SQLiteManager.GLOBAL_RECORD);
                }

                if (statisticGlobal.MouseLeftBtn.IsUpdated)
                {
                    SQLiteManager.GetInstance.UpdateGlobal(statisticGlobal.MouseLeftBtn.Type, statisticGlobal.MouseLeftBtn.Value);
                    statisticGlobal.MouseLeftBtn.IsUpdated = false;
                }

                if (statisticGlobal.MouseRightBtn.IsUpdated)
                {
                    SQLiteManager.GetInstance.UpdateGlobal(statisticGlobal.MouseRightBtn.Type, statisticGlobal.MouseRightBtn.Value);
                    statisticGlobal.MouseRightBtn.IsUpdated = false;
                }

                if (statisticGlobal.MouseWheelForward.IsUpdated)
                {
                    SQLiteManager.GetInstance.UpdateGlobal(statisticGlobal.MouseWheelForward.Type, statisticGlobal.MouseWheelForward.Value);
                    statisticGlobal.MouseWheelForward.IsUpdated = false;
                }

                if (statisticGlobal.MouseWheelBackward.IsUpdated)
                {
                    SQLiteManager.GetInstance.UpdateGlobal(statisticGlobal.MouseWheelBackward.Type, statisticGlobal.MouseWheelBackward.Value);
                    statisticGlobal.MouseWheelBackward.IsUpdated = false;
                }

                if (statisticGlobal.MouseSideKeyForward.IsUpdated)
                {
                    SQLiteManager.GetInstance.UpdateGlobal(statisticGlobal.MouseSideKeyForward.Type, statisticGlobal.MouseSideKeyForward.Value);
                    statisticGlobal.MouseSideKeyForward.IsUpdated = false;
                }

                if (statisticGlobal.MouseSideKeyBackward.IsUpdated)
                {
                    SQLiteManager.GetInstance.UpdateGlobal(statisticGlobal.MouseSideKeyBackward.Type, statisticGlobal.MouseSideKeyBackward.Value);
                    statisticGlobal.MouseSideKeyBackward.IsUpdated = false;
                }
            }

            now = DateTime.Now;
            Logger.v(TAG, "TimerCallback,now2, " + now.Minute + ":" + now.Second + "." + now.Millisecond);
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
                statisticGlobal.KeyboardTotal.IsUpdated = true;

                //statisticYear.KeyboardTotal.Value++;
                //statisticMonth.KeyboardTotal.Value++;
                //statisticDay.KeyboardTotal.Value++;
                
                if (fkey > 0)
                {
                    statisticGlobal.KeyboardComboTotal.Value++;
                    statisticGlobal.KeyboardComboTotal.Desc = GetDesc1(statisticGlobal.KeyboardComboTotal.Value);
                    statisticGlobal.KeyboardComboTotal.IsUpdated = true;

                    //statisticYear.KeyboardComboTotal.Value++;
                    //statisticMonth.KeyboardComboTotal.Value++;
                    //statisticDay.KeyboardComboTotal.Value++;
                }

                SttKeyboardTotalToday.Value++;
                SttKeyboardTotalToday.Desc = GetDesc1(SttKeyboardTotalToday.Value);

                UpdateOpInHour(time.Hour, 1);

                KbSingleKeyPressed(typeCode);
                
                //storage total statistic
                //storage year statistic
                //storage month statistic
                //storage day statistic
                //storage hour statistic

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
                    statisticGlobal.MouseWheelBackward.IsUpdated = true;

                    statisticYear.MouseWheelBackward.Value += (ushort)mouseData;
                    statisticMonth.MouseWheelBackward.Value += (ushort)mouseData;
                    statisticDay.MouseWheelBackward.Value += (ushort)mouseData;
                    break;
                case Constants.TypeNumber.MOUSE_WHEEL_FORWARD:
                    statisticGlobal.MouseWheelForward.Value += (ushort)mouseData;
                    statisticGlobal.MouseWheelForward.Desc = GetDesc1(statisticGlobal.MouseWheelForward.Value);
                    statisticGlobal.MouseWheelForward.IsUpdated = true;

                    statisticYear.MouseWheelBackward.Value += (ushort)mouseData;
                    statisticMonth.MouseWheelBackward.Value += (ushort)mouseData;
                    statisticDay.MouseWheelBackward.Value += (ushort)mouseData;
                    break;
                case Constants.TypeNumber.MOUSE_WHEEL_CLICK:
                    statisticGlobal.MouseWheelClick.Value++;
                    statisticGlobal.MouseWheelClick.Desc = GetDesc1(statisticGlobal.MouseWheelClick.Value);
                    statisticGlobal.MouseWheelClick.IsUpdated = true;

                    statisticYear.MouseWheelBackward.Value++;
                    statisticMonth.MouseWheelBackward.Value++;
                    statisticDay.MouseWheelBackward.Value++;
                    break;
                case Constants.TypeNumber.MOUSE_LEFT_BTN:
                    statisticGlobal.MouseLeftBtn.Value++;
                    statisticGlobal.MouseLeftBtn.Desc = GetDesc1(statisticGlobal.MouseLeftBtn.Value);
                    statisticGlobal.MouseLeftBtn.IsUpdated = true;
                    mouseData = getScreenArea(x, y);

                    statisticYear.MouseWheelBackward.Value++;
                    statisticMonth.MouseWheelBackward.Value++;
                    statisticDay.MouseWheelBackward.Value++;
                    break;
                case Constants.TypeNumber.MOUSE_RIGHT_BTN:
                    statisticGlobal.MouseRightBtn.Value++;
                    statisticGlobal.MouseRightBtn.Desc = GetDesc1(statisticGlobal.MouseRightBtn.Value);
                    statisticGlobal.MouseRightBtn.IsUpdated = true;
                    mouseData = getScreenArea(x, y);

                    statisticYear.MouseWheelBackward.Value++;
                    statisticMonth.MouseWheelBackward.Value++;
                    statisticDay.MouseWheelBackward.Value++;
                    break;
                case Constants.TypeNumber.MOUSE_SIDE_FORWARD:
                    statisticGlobal.MouseSideKeyForward.Value++;
                    statisticGlobal.MouseSideKeyForward.Desc = GetDesc1(statisticGlobal.MouseSideKeyForward.Value);
                    statisticGlobal.MouseSideKeyForward.IsUpdated = true;

                    statisticYear.MouseWheelBackward.Value++;
                    statisticMonth.MouseWheelBackward.Value++;
                    statisticDay.MouseWheelBackward.Value++;
                    break;
                case Constants.TypeNumber.MOUSE_SIDE_BACKWARD:
                    statisticGlobal.MouseSideKeyBackward.Value++;
                    statisticGlobal.MouseSideKeyBackward.Desc = GetDesc1(statisticGlobal.MouseSideKeyBackward.Value);
                    statisticGlobal.MouseSideKeyBackward.IsUpdated = true;

                    statisticYear.MouseWheelBackward.Value++;
                    statisticMonth.MouseWheelBackward.Value++;
                    statisticDay.MouseWheelBackward.Value++;
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

        private ushort getScreenArea(short x, short y)
        {
            return (ushort)(Math.Floor((float)x / (float)screenAreaWidth) + 10 * Math.Floor((float)y / (float)screenAreaHeight) + 1);
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
                    rco.IsUpdated = true;
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

        private bool InitDatabase()
        {
            DateTime now = DateTime.Now;
            Logger.v(TAG, "init table,now1, " + now.Minute + ":" + now.Second + "." + now.Millisecond);
            SQLiteManager.GetInstance.CreateGlobalTable();
            //插入所有记录。
            if (SQLiteManager.GetInstance.BeginTransaction(SQLiteManager.GLOBAL_RECORD))
            {
                SQLiteManager.GetInstance.InsertGlobal(statisticGlobal.KeyboardTotal.Type, 0);
                SQLiteManager.GetInstance.InsertGlobal(statisticGlobal.KeyboardComboTotal.Type, 0);
                SQLiteManager.GetInstance.InsertGlobal(statisticGlobal.MouseTotal.Type, 0);
                SQLiteManager.GetInstance.InsertGlobal(statisticGlobal.MouseLeftBtn.Type, 0);
                SQLiteManager.GetInstance.InsertGlobal(statisticGlobal.MouseRightBtn.Type, 0);
                SQLiteManager.GetInstance.InsertGlobal(statisticGlobal.MouseWheelForward.Type, 0);
                SQLiteManager.GetInstance.InsertGlobal(statisticGlobal.MouseWheelBackward.Type, 0);
                SQLiteManager.GetInstance.InsertGlobal(statisticGlobal.MouseWheelClick.Type, 0);
                SQLiteManager.GetInstance.InsertGlobal(statisticGlobal.MouseSideKeyForward.Type, 0);
                SQLiteManager.GetInstance.InsertGlobal(statisticGlobal.MouseSideKeyBackward.Type, 0);
                List<Record> list = new List<Record>();
                if (statisticGlobal.CopyKeys(list))
                {
                    foreach (Record rco in list)
                    {
                        SQLiteManager.GetInstance.InsertGlobal(rco.Type, 0);
                    }
                }

                SQLiteManager.GetInstance.CommitTransaction(SQLiteManager.GLOBAL_RECORD);
                now = DateTime.Now;
                Logger.v(TAG, "init table,now2, " + now.Minute + ":" + now.Second + "." + now.Millisecond);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 初始化并从数据库中读取统计数据。
        /// </summary>
        private void QueryStatisticFromDb()
        {
            SQLiteDataReader reader = SQLiteManager.GetInstance.QueryTotalStatistic();
            if (reader is null)
            {
                Logger.v(TAG, "No statistic record found");
                return;
            }

            DateTime now = DateTime.Now;
            Logger.v(TAG, "bootup,now,minue:" + now.Minute + ",second:" + now.Second + ",ms:" + now.Millisecond);
            ushort type;
            uint value;
            bool noRecord = true;
            while (reader.Read())
            {
                noRecord = false;
                type = (ushort)reader.GetInt16(0);
                value = (ushort)reader.GetInt16(1);

                if (type > 0 && type < 256)
                {
                    foreach (Record rco in statisticGlobal.KeyboardKeys)
                    {
                        if (rco.Type == type)
                        {
                            rco.Value += value;
                            break;
                        }
                    }
                }
                else
                {
                    switch (type)
                    {
                        case Constants.TypeNumber.KEYBOARD_TOTAL:
                            statisticGlobal.KeyboardTotal.Value += value;
                            break;
                        case Constants.TypeNumber.KEYBOARD_COMBOL_TOTAL:
                            statisticGlobal.KeyboardComboTotal.Value += value;
                            break;
                        case Constants.TypeNumber.MOUSE_LEFT_BTN:
                            statisticGlobal.MouseLeftBtn.Value += value;
                            break;
                        case Constants.TypeNumber.MOUSE_RIGHT_BTN:
                            statisticGlobal.MouseRightBtn.Value += value;
                            break;
                        case Constants.TypeNumber.MOUSE_WHEEL_FORWARD:
                            statisticGlobal.MouseWheelForward.Value += value;
                            break;
                        case Constants.TypeNumber.MOUSE_WHEEL_BACKWARD:
                            statisticGlobal.MouseWheelBackward.Value += value;
                            break;
                        case Constants.TypeNumber.MOUSE_SIDE_FORWARD:
                            statisticGlobal.MouseSideKeyForward.Value += value;
                            break;
                        case Constants.TypeNumber.MOUSE_SIDE_BACKWARD:
                            statisticGlobal.MouseSideKeyBackward.Value += value;
                            break;
                    }
                }
            }
            reader.Close();

            if (noRecord)
            {
                Logger.v(TAG, "No statistic record found");
                return;
            }

            statisticGlobal.KeyboardTotal.Desc = GetDesc1(statisticGlobal.KeyboardTotal.Value);
            statisticGlobal.KeyboardComboTotal.Desc = GetDesc1(statisticGlobal.KeyboardComboTotal.Value);
            statisticGlobal.SortKeys();
            statisticGlobal.KeyboardSkTop1.Desc = GetDesc2(Constants.Keyboard[(byte)statisticGlobal.KeyboardKeys[0].Type].DisplayName, statisticGlobal.KeyboardKeys[0].Value);
            statisticGlobal.KeyboardSkTop2.Desc = GetDesc2(Constants.Keyboard[(byte)statisticGlobal.KeyboardKeys[1].Type].DisplayName, statisticGlobal.KeyboardKeys[1].Value);
            statisticGlobal.KeyboardSkTop3.Desc = GetDesc2(Constants.Keyboard[(byte)statisticGlobal.KeyboardKeys[2].Type].DisplayName, statisticGlobal.KeyboardKeys[2].Value);
            statisticGlobal.KeyboardSkTop4.Desc = GetDesc2(Constants.Keyboard[(byte)statisticGlobal.KeyboardKeys[3].Type].DisplayName, statisticGlobal.KeyboardKeys[3].Value);
            statisticGlobal.KeyboardSkTop5.Desc = GetDesc2(Constants.Keyboard[(byte)statisticGlobal.KeyboardKeys[4].Type].DisplayName, statisticGlobal.KeyboardKeys[4].Value);

            statisticGlobal.MouseLeftBtn.Desc = GetDesc1(statisticGlobal.MouseLeftBtn.Value);
            statisticGlobal.MouseRightBtn.Desc = GetDesc1(statisticGlobal.MouseRightBtn.Value);
            statisticGlobal.MouseWheelForward.Desc = GetDesc1(statisticGlobal.MouseWheelForward.Value);
            statisticGlobal.MouseWheelBackward.Desc = GetDesc1(statisticGlobal.MouseWheelBackward.Value);

            now = DateTime.Now;
            Logger.v(TAG, "bootup,now2,minue:" + now.Minute + ",second:" + now.Second + ",ms:" + now.Millisecond);
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
                    Type = Constants.TypeNumber.KEYBOARD_TOTAL
                };

                KeyboardComboTotal = new Record
                {
                    Type = Constants.TypeNumber.KEYBOARD_COMBOL_TOTAL
                };

                KeyboardSkTop1 = new Record
                {
                    Type = Constants.TypeNumber.KB_SK_TOP1
                };

                KeyboardSkTop2 = new Record
                {
                    Type = Constants.TypeNumber.KB_SK_TOP2
                };

                KeyboardSkTop3 = new Record
                {
                    Type = Constants.TypeNumber.KB_SK_TOP3
                };

                KeyboardSkTop4 = new Record
                {
                    Type = Constants.TypeNumber.KB_SK_TOP4
                };

                KeyboardSkTop5 = new Record
                {
                    Type = Constants.TypeNumber.KB_SK_TOP5
                };

                Dictionary<byte, Key> sgKey = Constants.Keyboard;
                KeyboardKeys = new List<Record>(sgKey.Count);
                Dictionary<byte, Key>.ValueCollection values = sgKey.Values;
                foreach (Key key in values)
                {
                    KeyboardKeys.Add(new Record((ushort)key.Code));
                }

                MouseTotal = new Record
                {
                    Type = Constants.TypeNumber.MOUSE_TOTAL
                };

                MouseLeftBtn = new Record
                {
                    Type = Constants.TypeNumber.MOUSE_LEFT_BTN
                };

                MouseRightBtn = new Record
                {
                    Type = Constants.TypeNumber.MOUSE_RIGHT_BTN
                };

                MouseWheelForward = new Record
                {
                    Type = Constants.TypeNumber.MOUSE_WHEEL_FORWARD
                };

                MouseWheelBackward = new Record
                {
                    Type = Constants.TypeNumber.MOUSE_WHEEL_BACKWARD
                };

                MouseWheelClick = new Record
                {
                    Type = Constants.TypeNumber.MOUSE_WHEEL_CLICK
                };

                MouseSideKeyForward = new Record
                {
                    Type = Constants.TypeNumber.MOUSE_SIDE_FORWARD
                };

                MouseSideKeyBackward = new Record
                {
                    Type = Constants.TypeNumber.MOUSE_SIDE_BACKWARD
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
