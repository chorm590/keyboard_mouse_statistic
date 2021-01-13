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

        private readonly List<Event> sttKbSingleKey; //键盘各单键的敲击总数。
        private readonly Dictionary<byte, ushort> opInHour; //今日各时段的操作记数。

        private readonly EventToDbManager eventToDbManager;

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
            //Statistic of all


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

            //Statistic of keyboard single key
            Dictionary<byte, Key> sgKey = Constants.Keyboard;
            sttKbSingleKey = new List<Event>(sgKey.Count);
            Dictionary<byte, Key>.ValueCollection values = sgKey.Values;
            foreach (Key key in values)
            {
                sttKbSingleKey.Add(new Event(key.Type)); //Should not care about the display name
            }
            Logger.v(TAG, "single key count:" + sttKbSingleKey.Count + ", capacity:" + sttKbSingleKey.Capacity);

            //Statistic of mouse


            //Initialize timer
            //tool.Timer.RegisterTimerCallback(timerCallback);
            eventToDbManager = new EventToDbManager();

            screenAreaWidth = System.Windows.SystemParameters.PrimaryScreenWidth / 10;
            screenAreaHeight = System.Windows.SystemParameters.PrimaryScreenHeight / 10;
            Logger.v(TAG, "screen width:" + screenAreaWidth + ",height:" + screenAreaHeight);

            statisticGlobal = new Statistic();
            statisticYear = new Statistic();
            statisticMonth = new Statistic();
            statisticDay = new Statistic();
            statisticHour = new Statistic();

            if (InitDatabase())
            {
                QueryStatisticFromDb();
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
            eventToDbManager.storageRecord();
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
                SttKeyboardTotal.Value++;
                SttKeyboardTotal.Desc = GetDesc1(SttKeyboardTotal.Value);
                
                if (fkey > 0)
                {
                    SttComboKeyTotal.Value++;
                    SttComboKeyTotal.Desc = GetDesc1(SttComboKeyTotal.Value);
                }

                SttKeyboardTotalToday.Value++;
                SttKeyboardTotalToday.Desc = GetDesc1(SttKeyboardTotalToday.Value);

                UpdateOpInHour(time.Hour, 1);

                KbSingleKeyPressed(typeCode);

                //eventToDbManager.Add((ushort)time.Year, (byte)time.Month, (byte)time.Day, (byte)time.Hour, (byte)time.Minute, (byte)time.Second,
                //    (short)typeCode, fkey, 0);
                
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
                    SttMsWheelBackward.Value += (ushort)mouseData;
                    SttMsWheelBackward.Desc = SttMsWheelBackward.Value + " 次";
                    break;
                case Constants.TypeNumber.MOUSE_WHEEL_FORWARD:
                    SttMsWheelForward.Value += (ushort)mouseData;
                    SttMsWheelForward.Desc = SttMsWheelForward.Value + " 次";
                    break;
                case Constants.TypeNumber.MOUSE_WHEEL_CLICK:
                    SttMsWheelClick.Value += (ushort)mouseData;
                    SttMsWheelClick.Desc = SttMsWheelClick.Value + " 次";
                    break;
                case Constants.TypeNumber.MOUSE_LEFT_BTN:
                    SttMsLeftBtn.Value++;
                    SttMsLeftBtn.Desc = SttMsLeftBtn.Value + " 次";
                    mouseData = getScreenArea(x, y);
                    break;
                case Constants.TypeNumber.MOUSE_RIGHT_BTN:
                    SttMsRightBtn.Value++;
                    SttMsRightBtn.Desc = SttMsRightBtn.Value + " 次";
                    mouseData = getScreenArea(x, y);
                    break;
                case Constants.TypeNumber.MOUSE_SIDE_FORWARD:
                    SttMsSideForward.Value++;
                    SttMsSideForward.Desc = SttMsSideForward.Value + " 次";
                    mouseData = getScreenArea(x, y);
                    break;
                case Constants.TypeNumber.MOUSE_SIDE_BACKWARD:
                    SttMsSideBackward.Value++;
                    SttMsSideBackward.Desc = SttMsSideBackward.Value + " 次";
                    mouseData = getScreenArea(x, y);
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

            SttMouseTotalToday.Desc = SttMouseTotalToday.Value + " 次";

            eventToDbManager.Add((ushort)time.Year, (byte)time.Month, (byte)time.Day, (byte)time.Hour, (byte)time.Minute, (byte)time.Second,
                    (short)typeCode,
                    0,
                    (ushort)mouseData);
        }

        private ushort getScreenArea(short x, short y)
        {
            return (ushort)(Math.Floor((float)x / (float)screenAreaWidth) + 10 * Math.Floor((float)y / (float)screenAreaHeight) + 1);
        }

        private void KbSingleKeyPressed(int keycode)
        {
            foreach (Event kevt in sttKbSingleKey)
            {
                if (kevt.Type.Code == keycode)
                {
                    kevt.Value++;
                    break;
                }
            }

            sttKbSingleKey.Sort();
            if (sttKbSingleKey[0].Value > 0)
            {
                SttKeyboardSingleKeyTop1.Desc = GetDesc2(Constants.Keyboard[(byte)(sttKbSingleKey[0].Type.Code)].DisplayName, sttKbSingleKey[0].Value);
            }

            if (sttKbSingleKey[1].Value > 0)
            {
                SttKeyboardSingleKeyTop2.Desc = GetDesc2(Constants.Keyboard[(byte)(sttKbSingleKey[1].Type.Code)].DisplayName, sttKbSingleKey[1].Value);
            }

            if (sttKbSingleKey[2].Value > 0)
            {
                SttKeyboardSingleKeyTop3.Desc = GetDesc2(Constants.Keyboard[(byte)(sttKbSingleKey[2].Type.Code)].DisplayName, sttKbSingleKey[2].Value);
            }

            if (sttKbSingleKey[3].Value > 0)
            {
                SttKeyboardSingleKeyTop4.Desc = GetDesc2(Constants.Keyboard[(byte)(sttKbSingleKey[3].Type.Code)].DisplayName, sttKbSingleKey[3].Value);
            }

            if (sttKbSingleKey[4].Value > 0)
            {
                SttKeyboardSingleKeyTop5.Desc = GetDesc2(Constants.Keyboard[(byte)(sttKbSingleKey[4].Type.Code)].DisplayName, sttKbSingleKey[4].Value);
            }

            //Find the most typed letter
            foreach (Event kevt in sttKbSingleKey)
            {
                if (kevt.Type.Code >= Constants.TypeNumber.A && kevt.Type.Code <= Constants.TypeNumber.Z)
                {
                    SttLetterTop1Today.Value = kevt.Value;
                    SttLetterTop1Today.Desc = GetDesc2(kevt.Type.Desc, kevt.Value);
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
            return SQLiteManager.GetInstance.Init();
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
            Logger.v(TAG, "now,minue:" + now.Minute + ",second:" + now.Second + ",ms:" + now.Millisecond);
            ushort type;
            uint value;
            bool noRecord = true;
            while (reader.Read())
            {
                noRecord = false;
                type = (ushort)reader.GetInt16(1);
                value = (ushort)reader.GetInt16(2);

                if (type > 0 && type < 256)
                {
                    foreach (Event kev in sttKbSingleKey)
                    {
                        if (kev.Type.Code == type)
                        {
                            kev.Value++;
                            break;
                        }
                    }
                }
                else
                {
                    switch (type)
                    {
                        case Constants.TypeNumber.KEYBOARD_TOTAL:
                            SttKeyboardTotal.Value += value;
                            break;
                        case Constants.TypeNumber.KEYBOARD_COMBOL_TOTAL:
                            SttComboKeyTotal.Value += value;
                            break;
                        case Constants.TypeNumber.MOUSE_LEFT_BTN:
                            SttMsLeftBtn.Value++;
                            break;
                        case Constants.TypeNumber.MOUSE_RIGHT_BTN:
                            SttMsRightBtn.Value++;
                            break;
                        case Constants.TypeNumber.MOUSE_WHEEL_FORWARD:
                            SttMsWheelForward.Value++;
                            break;
                        case Constants.TypeNumber.MOUSE_WHEEL_BACKWARD:
                            SttMsWheelBackward.Value++;
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

            SttKeyboardTotal.Desc = GetDesc1(SttKeyboardTotal.Value);
            SttComboKeyTotal.Desc = GetDesc1(SttComboKeyTotal.Value);
            sttKbSingleKey.Sort();
            SttKeyboardSingleKeyTop1.Desc = GetDesc2(sttKbSingleKey[0].Type.Desc, sttKbSingleKey[0].Value);
            SttKeyboardSingleKeyTop2.Desc = GetDesc2(sttKbSingleKey[1].Type.Desc, sttKbSingleKey[1].Value);
            SttKeyboardSingleKeyTop3.Desc = GetDesc2(sttKbSingleKey[2].Type.Desc, sttKbSingleKey[2].Value);
            SttKeyboardSingleKeyTop4.Desc = GetDesc2(sttKbSingleKey[3].Type.Desc, sttKbSingleKey[3].Value);
            SttKeyboardSingleKeyTop5.Desc = GetDesc2(sttKbSingleKey[4].Type.Desc, sttKbSingleKey[4].Value);

            SttMsLeftBtn.Desc = GetDesc1(SttMsLeftBtn.Value);
            SttMsRightBtn.Desc = GetDesc1(SttMsRightBtn.Value);
            SttMsWheelForward.Desc = GetDesc1(SttMsWheelForward.Value);
            SttMsWheelBackward.Desc = GetDesc1(SttMsWheelBackward.Value);

            now = DateTime.Now;
            Logger.v(TAG, "now2,minue:" + now.Minute + ",second:" + now.Second + ",ms:" + now.Millisecond);
        }

        private string GetDesc1(long value)
        {
            return value + " 次";
        }

        private string GetDesc2(string value1, long value2)
        {
            return value1 + " [" + value2 + " 次]";
        }

        /// <summary>
        /// 将由StatisticManager管理的在内存中的事件落地到数据库中。
        /// </summary>
        private class EventToDbManager
        {
            private const short DEEPTH = 1000; //Enough?

            private EventToDb[] cur;
            private short counter;
            private byte status;

            internal EventToDbManager()
            {
                cur = new EventToDb[DEEPTH];
            }

            internal void Add(ushort year, byte month, byte day, byte hour, byte minute, byte second, short type, byte fkey, ushort value)
            {
                if (status == 0)
                    status = 1;
                else
                {
                    int tmp = 0;
                    while (tmp++ < 5)
                    {
                        Thread.Sleep(10);
                        if (status == 0)
                            break;
                    }

                    if (status == 0)
                        status = 1;
                    else
                        return;
                }

                if (status != 1)
                    return;

                if(cur is null)
                    return;

                if (counter >= DEEPTH)
                    return;

                cur[counter].year = year;
                cur[counter].month = month;
                cur[counter].day = day;
                cur[counter].hour = hour;
                cur[counter].minute = minute;
                cur[counter].second = second;
                cur[counter].type = type;
                cur[counter].fkey = fkey;
                cur[counter].value = value;

                counter++;
                status = 0;
            }

            /// <summary>
            /// 由TimeManager中的定时器子线程触发调用。
            /// 将前面积累到的事件存储到数据库中。
            /// </summary>
            internal void storageRecord()
            {
                if (status == 0)
                    status = 2;
                else
                {
                    int tmp = 0;
                    while (tmp++ < 5)
                    {
                        Thread.Sleep(10);
                        if (status == 0)
                            break;
                    }

                    if (status == 0)
                        status = 2;
                    else
                        return;
                }

                if (status != 2)
                    return;

                short counter2 = counter;
                EventToDb[] tmp2 = new EventToDb[counter2];
                for (int i = 0; i < counter2; i++)
                {
                    tmp2[i].type = cur[i].type;
                    tmp2[i].year = cur[i].year;
                    tmp2[i].month = cur[i].month;
                    tmp2[i].day = cur[i].day;
                    tmp2[i].hour = cur[i].hour;
                    tmp2[i].minute = cur[i].minute;
                    tmp2[i].second = cur[i].second;
                    tmp2[i].fkey = cur[i].fkey;
                    tmp2[i].value = cur[i].value;
                }
                counter = 0; //reset it.
                status = 0; //Important

                Logger.v(TAG, "There's " + counter2 + " records need to be storage.");
                DateTime dt = DateTime.Now;
                Logger.v(TAG, "[1] hour:" + dt.Hour + ",minute:" + dt.Minute + ",second:" + dt.Second + ",ms:" + dt.Millisecond);
                if (SQLiteManager.GetInstance.BeginTransaction())
                {
                    string str;
                    foreach (EventToDb etd in tmp2)
                    {
                        str = "VALUES(" + etd.year + "," + etd.month + "," + etd.day + "," + etd.hour + "," + etd.minute + "," + etd.second;
                        str += "," + etd.type + "," + etd.fkey + "," + etd.value + ")";
                        SQLiteManager.GetInstance.InsertDetail(str);
                    }
                    SQLiteManager.GetInstance.CommitTransaction();
                    dt = DateTime.Now;
                    Logger.v(TAG, "[2] hour:" + dt.Hour + ",minute:" + dt.Minute + ",second:" + dt.Second + ",ms:" + dt.Millisecond);
                }
                else
                {
                    //Do nothing
                    Logger.v(TAG, "Transaction begin failed");
                }
            }

            private struct EventToDb
            {
                internal ushort year;
                internal byte month;
                internal byte day;
                internal byte hour;
                internal byte minute;
                internal byte second;
                internal short type;
                internal byte fkey;
                internal ushort value;
            }
        }

        private class Statistic
        {
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
                Logger.v(TAG, "single key count:" + KeyboardKeys.Count + ", capacity:" + KeyboardKeys.Capacity);

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
        }
    }
}
