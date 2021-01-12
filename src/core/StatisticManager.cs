using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using System.Threading;
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
        
        internal Event SttKeyboardTotal
        {
            get;
        }

        internal Event SttComboKeyTotal
        {
            get;
        }
        
        internal Event SttKeyboardSingleKeyTop1
        {
            get;
        }

        internal Event SttKeyboardSingleKeyTop2
        {
            get;
        }

        internal Event SttKeyboardSingleKeyTop3
        {
            get;
        }

        internal Event SttKeyboardSingleKeyTop4
        {
            get;
        }

        internal Event SttKeyboardSingleKeyTop5
        {
            get;
        }

        internal Event SttMsLeftBtn
        {
            get;
        }

        internal Event SttMsRightBtn
        {
            get;
        }

        internal Event SttMsWheelForward
        {
            get;
        }

        internal Event SttMsWheelBackward
        {
            get;
        }

        internal Event SttMsWheelClick
        {
            get;
        }

        internal Event SttMsSideForward
        {
            get;
        }

        internal Event SttMsSideBackward
        {
            get;
        }

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
            SttKeyboardTotal = new Event
            {
                Type = Constants.Statistic.KbTotal
            };

            SttComboKeyTotal = new Event
            {
                Type = Constants.Statistic.KbComboTotal
            };

            SttKeyboardSingleKeyTop1 = new Event
            {
                Type = Constants.Statistic.KbSkTop1
            };

            SttKeyboardSingleKeyTop2 = new Event
            {
                Type = Constants.Statistic.KbSkTop2
            };

            SttKeyboardSingleKeyTop3 = new Event
            {
                Type = Constants.Statistic.KbSkTop3
            };

            SttKeyboardSingleKeyTop4 = new Event
            {
                Type = Constants.Statistic.KbSkTop4
            };

            SttKeyboardSingleKeyTop5 = new Event
            {
                Type = Constants.Statistic.KbSkTop5
            };

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
            SttMsLeftBtn = new Event
            {
                Type = Constants.MouseKeys[Constants.TypeNumber.MOUSE_LEFT_BTN]
            };
            SttMsRightBtn = new Event
            {
                Type = Constants.MouseKeys[Constants.TypeNumber.MOUSE_RIGHT_BTN]
            };
            SttMsWheelForward = new Event
            {
                Type = Constants.MouseKeys[Constants.TypeNumber.MOUSE_WHEEL_FORWARD]
            };
            SttMsWheelBackward = new Event
            {
                Type = Constants.MouseKeys[Constants.TypeNumber.MOUSE_WHEEL_BACKWARD]
            };
            SttMsWheelClick = new Event
            {
                Type = Constants.MouseKeys[Constants.TypeNumber.MOUSE_WHEEL_CLICK]
            };
            SttMsSideForward = new Event
            {
                Type = Constants.MouseKeys[Constants.TypeNumber.MOUSE_SIDE_FORWARD]
            };
            SttMsSideBackward = new Event
            {
                Type = Constants.MouseKeys[Constants.TypeNumber.MOUSE_SIDE_BACKWARD]
            };

            //Initialize timer
            //tool.Timer.RegisterTimerCallback(timerCallback);
            eventToDbManager = new EventToDbManager();

            screenAreaWidth = System.Windows.SystemParameters.PrimaryScreenWidth / 10;
            screenAreaHeight = System.Windows.SystemParameters.PrimaryScreenHeight / 10;
            Logger.v(TAG, "screen width:" + screenAreaWidth + ",height:" + screenAreaHeight);

            statisticFromDb();
        }

        internal void shutdown()
        {
            //TODO flush all data to disk.
        }

        private void timerCallback(object state)
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
                SttKeyboardTotal.Desc = SttKeyboardTotal.Value + " 次";
                
                if (fkey > 0)
                {
                    SttComboKeyTotal.Value++;
                    SttComboKeyTotal.Desc = SttComboKeyTotal.Value + " 次";
                }

                SttKeyboardTotalToday.Value++;
                SttKeyboardTotalToday.Desc = SttKeyboardTotalToday.Value + " 次";

                updateOpInHour(time.Hour, 1);

                kbSingleKeyPressed(typeCode);

                eventToDbManager.Add((ushort)time.Year, (byte)time.Month, (byte)time.Day, (byte)time.Hour, (byte)time.Minute, (byte)time.Second,
                    (short)typeCode, fkey, 0);
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
                    updateOpInHour(time.Hour, mouseData);
                    break;
                case Constants.TypeNumber.MOUSE_WHEEL_CLICK:
                case Constants.TypeNumber.MOUSE_LEFT_BTN:
                case Constants.TypeNumber.MOUSE_RIGHT_BTN:
                case Constants.TypeNumber.MOUSE_SIDE_FORWARD:
                case Constants.TypeNumber.MOUSE_SIDE_BACKWARD:
                    SttMouseTotalToday.Value++;
                    updateOpInHour(time.Hour, 1);
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

        private void kbSingleKeyPressed(int keycode)
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
                SttKeyboardSingleKeyTop1.Desc = Constants.Keyboard[(byte)(sttKbSingleKey[0].Type.Code)].DisplayName + " [" + sttKbSingleKey[0].Value + " 次]";
            }

            if (sttKbSingleKey[1].Value > 0)
            {
                SttKeyboardSingleKeyTop2.Desc = Constants.Keyboard[(byte)(sttKbSingleKey[1].Type.Code)].DisplayName + " [" + sttKbSingleKey[1].Value + " 次]";
            }

            if (sttKbSingleKey[2].Value > 0)
            {
                SttKeyboardSingleKeyTop3.Desc = Constants.Keyboard[(byte)(sttKbSingleKey[2].Type.Code)].DisplayName + " [" + sttKbSingleKey[2].Value + " 次]";
            }

            if (sttKbSingleKey[3].Value > 0)
            {
                SttKeyboardSingleKeyTop4.Desc = Constants.Keyboard[(byte)(sttKbSingleKey[3].Type.Code)].DisplayName + " [" + sttKbSingleKey[3].Value + " 次]";
            }

            if (sttKbSingleKey[4].Value > 0)
            {
                SttKeyboardSingleKeyTop5.Desc = Constants.Keyboard[(byte)(sttKbSingleKey[4].Type.Code)].DisplayName + " [" + sttKbSingleKey[4].Value + " 次]";
            }

            //Find the most typed letter
            foreach (Event kevt in sttKbSingleKey)
            {
                if (kevt.Type.Code >= Constants.TypeNumber.A && kevt.Type.Code <= Constants.TypeNumber.Z)
                {
                    SttLetterTop1Today.Value = kevt.Value;
                    SttLetterTop1Today.Desc = kevt.Type.Desc + "[" + kevt.Value + " 次]";
                    break;
                }
            }
        }

        /// <summary>
        /// 更新今日统计中的各时段操作总数。
        /// </summary>
        private void updateOpInHour(int hour, int value)
        {
            ushort value2;
            ushort maxValue = 0;
            ushort maxHour = 0;
            if (opInHour.TryGetValue((byte)hour, out value2))
            {
                value = value + value2;
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
        /// 从数据库中读取之前保存的数据。
        /// </summary>
        private void statisticFromDb()
        {
            //1. 
            List<string> dbPath = SQLiteManager.GetInstance.IterateDbs();
            if (dbPath == null)
                return;

            Logger.v(TAG, "There is " + dbPath.Count + " database file");
            DateTime now = DateTime.Now;
            Logger.v(TAG, "now,minue:" + now.Minute + ",second:" + now.Second + ",ms:" + now.Millisecond);
            ushort type;
            byte fkey;
            ushort value;
            short idx;
            foreach (string db in dbPath)
            {
                if(SQLiteManager.GetInstance.UseDatabase(db)) //event record of a month
                {
                    for (byte i = 1; i < 32; i++) //day1 ~ day31
                    {
                        SQLiteDataReader reader = SQLiteManager.GetInstance.GetEventDetail(i);
                        if (reader is null)
                            continue;

                        Logger.v(TAG, "day" + i + ", field count:" + reader.FieldCount);

                        while(reader.Read())
                        {
                            type = (ushort)reader.GetInt16(6);
                            fkey = reader.GetByte(7);
                            value = (ushort)reader.GetInt16(8);

                            if (type > 0 && type < 256)
                            {
                                SttKeyboardTotal.Value++;
                                if (fkey > 0)
                                {
                                    SttComboKeyTotal.Value++;
                                }
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
                    }

                    SttKeyboardTotal.Desc = SttKeyboardTotal.Value + " 次";
                    SttComboKeyTotal.Desc = SttComboKeyTotal.Value + " 次";
                    sttKbSingleKey.Sort();
                    SttKeyboardSingleKeyTop1.Desc = sttKbSingleKey[0].Type.Desc + " [" + sttKbSingleKey[0].Value + " 次]";
                    SttKeyboardSingleKeyTop2.Desc = sttKbSingleKey[1].Type.Desc + " [" + sttKbSingleKey[1].Value + " 次]";
                    SttKeyboardSingleKeyTop3.Desc = sttKbSingleKey[2].Type.Desc + " [" + sttKbSingleKey[2].Value + " 次]";
                    SttKeyboardSingleKeyTop4.Desc = sttKbSingleKey[3].Type.Desc + " [" + sttKbSingleKey[3].Value + " 次]";
                    SttKeyboardSingleKeyTop5.Desc = sttKbSingleKey[4].Type.Desc + " [" + sttKbSingleKey[4].Value + " 次]";

                    SttMsLeftBtn.Desc = SttMsLeftBtn.Value + " 次";
                    SttMsRightBtn.Desc = SttMsRightBtn.Value + " 次";
                    SttMsWheelForward.Desc = SttMsWheelForward.Value + " 次";
                    SttMsWheelBackward.Desc = SttMsWheelBackward.Value + " 次";



                    //TODO close the database
                }
            }
            now = DateTime.Now;
            Logger.v(TAG, "now2,minue:" + now.Minute + ",second:" + now.Second + ",ms:" + now.Millisecond);
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
    }
}
