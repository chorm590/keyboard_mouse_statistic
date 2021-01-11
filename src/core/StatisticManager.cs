using System;
using System.Collections.Generic;
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

        private Event sttKbTotal; //键盘总计数。
        private Event sttComboKeyTotal; //组合键总计数。
        private Event sttKbSkTop1;
        private Event sttKbSkTop2;
        private Event sttKbSkTop3;
        private Event sttKbCkTop1;
        private Event sttKbCkTop2;
        private Event sttKbCkTop3;
        private List<Event> sttKbSingleKey; //键盘各单键的敲击总数。
        private List<Event> sttKbComboKey; //键盘组合键敲击总数。
        

        private Dictionary<short, Event> sttMsKey; //鼠标单键统计。
        private Event sttMsLBtn;
        private Event sttMsRBtn;
        private Event sttMsWheelFw;
        private Event sttMsWheelBw;

        private List<Event> sttOthers; //分时段的统计，每日、每月、每年。
        private EventToDbManager eventToDbManager;
        private double screenAreaWidth;
        private double screenAreaHeight;

        
        internal Event SttKeyboardTotal
        {
            get
            {
                return sttKbTotal;
            }
        }

        internal Event SttComboKeyTotal
        {
            get
            {
                return sttComboKeyTotal;
            }
        }
        
        internal List<Event> SttKeyboardSingle
        {
            get
            {
                return sttKbSingleKey;
            }
        }

        internal Event SttKeyboardSingleKeyTop1
        {
            get
            {
                return sttKbSkTop1;
            }
        }

        internal Event SttKeyboardSingleKeyTop2
        {
            get
            {
                return sttKbSkTop2;
            }
        }

        internal Event SttKeyboardSingleKeyTop3
        {
            get
            {
                return sttKbSkTop3;
            }
        }

        internal Event SttKeyboardComboKeyTop1
        {
            get
            {
                return sttKbCkTop1;
            }
        }

        internal Event SttKeyboardComboKeyTop2
        {
            get
            {
                return sttKbCkTop2;
            }
        }

        internal Event SttKeyboardComboKeyTop3
        {
            get
            {
                return sttKbCkTop3;
            }
        }

        internal List<Event> SttKeyboardComboKey
        {
            get
            {
                return sttKbComboKey;
            }
        }

        internal Event SttMsLeftBtn
        {
            get
            {
                return sttMsLBtn;
            }
        }

        internal Event SttMsRightBtn
        {
            get
            {
                return sttMsRBtn;
            }
        }

        internal Event SttMsWheelForward
        {
            get
            {
                return sttMsWheelFw;
            }
        }

        internal Event SttMsWheelBackward
        {
            get
            {
                return sttMsWheelBw;
            }
        }

        private StatisticManager()
        {
            // 1
            sttKbTotal = new Event
            {
                Type = Constants.Statistic.KbAll
            };

            sttComboKeyTotal = new Event
            {
                Type = Constants.Statistic.KbCombo
            };

            sttKbSkTop1 = new Event
            {
                Type = Constants.Statistic.KbSkTop1
            };

            sttKbSkTop2 = new Event
            {
                Type = Constants.Statistic.KbSkTop2
            };

            sttKbSkTop3 = new Event
            {
                Type = Constants.Statistic.KbSkTop3
            };

            sttKbCkTop1 = new Event
            {
                Type = Constants.Statistic.KbCkTop1
            };

            sttKbCkTop2 = new Event
            {
                Type = Constants.Statistic.KbCkTop2
            };

            sttKbCkTop3 = new Event
            {
                Type = Constants.Statistic.KbCkTop3
            };

            // 2
            Dictionary<byte, Key> sgKey = Constants.Keyboard.Keys;
            sttKbSingleKey = new List<Event>(sgKey.Count);
            Dictionary<byte, Key>.ValueCollection values = sgKey.Values;
            foreach (Key key in values)
            {
                sttKbSingleKey.Add(new Event(key.Type)); //Should not care about the display name
            }
            Logger.v(TAG, "single key count:" + sttKbSingleKey.Count + ", capacity:" + sttKbSingleKey.Capacity);

            // 3
            Dictionary<ushort, Type> cbKey = Constants.ComboKey.Keys;
            sttKbComboKey = new List<Event>(cbKey.Count);
            Dictionary<ushort, Type>.ValueCollection values2 = cbKey.Values;
            foreach (Type ck in values2)
            {
                sttKbComboKey.Add(new Event(ck));
            }
            Logger.v(TAG, "combo key count:" + sttKbComboKey.Count + ", capacity:" + sttKbComboKey.Capacity);

            Dictionary<short, Type> msKeyDic = Constants.MouseKey.Keys;
            sttMsKey = new Dictionary<short, Event>(msKeyDic.Count);
            Dictionary<short, Type>.ValueCollection values3 = msKeyDic.Values;
            foreach (Type tp in values3)
            {
                sttMsKey.Add((short)tp.Code, new Event(tp));
            }
            Logger.v(TAG, "mouse key count:" + sttMsKey.Count);
            sttMsLBtn = sttMsKey[Constants.MouseKey.MOUSE_LEFT_BTN];
            sttMsRBtn = sttMsKey[Constants.MouseKey.MOUSE_RIGHT_BTN];
            sttMsWheelFw = sttMsKey[Constants.MouseKey.MOUSE_WHEEL_FORWARD];
            sttMsWheelBw = sttMsKey[Constants.MouseKey.MOUSE_WHEEL_BACKWARD];

            //Initialize timer
            //tool.Timer.RegisterTimerCallback(timerCallback);
            eventToDbManager = new EventToDbManager();

            screenAreaWidth = System.Windows.SystemParameters.PrimaryScreenWidth / 10;
            screenAreaHeight = System.Windows.SystemParameters.PrimaryScreenHeight / 10;
            Logger.v(TAG, "screen width:" + screenAreaWidth + ",height:" + screenAreaHeight);

            SQLiteManager sql = SQLiteManager.GetInstance; //必须放在注册StatisticManager的timerCallback后面。
        }

        internal void shutdown()
        {
            //TODO flush all data to disk.
        }

        private void timerCallback(object state)
        {
            eventToDbManager.storageRecord();
        }

        internal void KeyboardEventHappen(int typeCode, DateTime time)
        {
            // This call may from sub-thread.

            //TODO 窗体没在前台运行的时候不要实时刷新统计数据，不要实时重新排序。

            if (typeCode < 1)
            {
                // Do nothing
            }
            else if (typeCode < 256)
            {
                kbSingleKeyPressed(typeCode);
                sttKbTotal.Value++;
                sttKbTotal.Desc = sttKbTotal.Value + " 次";
                eventToDbManager.Add((short)typeCode, (ushort)time.Year, (byte)time.Month, (byte)time.Day, (byte)time.Hour, (byte)time.Minute, (byte)time.Second, 0);
            }
            else if ((typeCode >= Constants.ComboKey.LC_LS) && (typeCode <= Constants.ComboKey.QUADRA))
            {
                kbComboKeyPressed(typeCode);
                sttComboKeyTotal.Value++;
                sttComboKeyTotal.Desc = sttComboKeyTotal.Value + " 次";
                eventToDbManager.Add((short)typeCode, (ushort)time.Year, (byte)time.Month, (byte)time.Day, (byte)time.Hour, (byte)time.Minute, (byte)time.Second, 0);
            }
            else
            {
                // Do nothing
            }
        }

        internal void MouseEventHappen(int typeCode, int mouseData, short x, short y, DateTime time)
        {
            if (sttMsKey.TryGetValue((short)typeCode, out Event evt))
            {
                if (evt is null)
                    return;

                switch (typeCode)
                {
                    case Constants.MouseKey.MOUSE_WHEEL_BACKWARD:
                    case Constants.MouseKey.MOUSE_WHEEL_FORWARD:
                        evt.Value += (ushort)mouseData;
                        evt.Desc = evt.Value + " 次";
                        break;
                    case Constants.MouseKey.MOUSE_LEFT_BTN:
                        evt.Value++;
                        evt.Desc = evt.Value + " 次";
                        eventToDbManager.Add(Constants.MouseEvent.MOUSE_LEFT_BTN_AREA,
                            (ushort)time.Year, (byte)time.Month, (byte)time.Day, (byte)time.Hour, (byte)time.Minute, (byte)time.Second,
                            getScreenArea(x, y));
                        break;
                    case Constants.MouseKey.MOUSE_RIGHT_BTN:
                        evt.Value++;
                        evt.Desc = evt.Value + " 次";
                        eventToDbManager.Add(Constants.MouseEvent.MOUSE_RIGHT_BTN_AREA,
                            (ushort)time.Year, (byte)time.Month, (byte)time.Day, (byte)time.Hour, (byte)time.Minute, (byte)time.Second,
                            getScreenArea(x, y));
                        break;
                    default:
                        evt.Value++;
                        evt.Desc = evt.Value + " 次";
                        break;
                }

                eventToDbManager.Add((short)typeCode,
                    (ushort)time.Year, (byte)time.Month, (byte)time.Day, (byte)time.Hour, (byte)time.Minute, (byte)time.Second,
                    (ushort)mouseData);
            }
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
                sttKbSkTop1.Desc = Constants.Keyboard.Keys[(byte)(sttKbSingleKey[0].Type.Code)].DisplayName + " [" + sttKbSingleKey[0].Value + " 次]";
            }

            if (sttKbSingleKey[1].Value > 0)
            {
                sttKbSkTop2.Desc = Constants.Keyboard.Keys[(byte)(sttKbSingleKey[1].Type.Code)].DisplayName + " [" + sttKbSingleKey[1].Value + " 次]";
            }

            if (sttKbSingleKey[2].Value > 0)
            {
                sttKbSkTop3.Desc = Constants.Keyboard.Keys[(byte)(sttKbSingleKey[2].Type.Code)].DisplayName + " [" + sttKbSingleKey[2].Value + " 次]";
            }
        }

        private void kbComboKeyPressed(int typeValue)
        {
            foreach (Event ckevt in sttKbComboKey)
            {
                if (ckevt.Type.Code == typeValue)
                {
                    ckevt.Value++;
                    break;
                }
            }

            sttKbComboKey.Sort();
            if (sttKbComboKey[0].Value > 0)
            {
                sttKbCkTop1.Desc = sttKbComboKey[0].Type.Desc + " [" + sttKbComboKey[0].Value + " 次]";
            }

            if (sttKbComboKey[1].Value > 0)
            {
                sttKbCkTop2.Desc = sttKbComboKey[1].Type.Desc + " [" + sttKbComboKey[1].Value + " 次]";
            }

            if (sttKbComboKey[2].Value > 0)
            {
                sttKbCkTop3.Desc = sttKbComboKey[2].Type.Desc + " [" + sttKbComboKey[2].Value + " 次]";
            }
        }

        /// <summary>
        /// 将由StatisticManager管理的在内存中的事件落地到数据库中。
        /// </summary>
        private class EventToDbManager
        {
            private const short DEEPTH = 1000;

            private EventToDb[] cur;
            private short counter;
            private byte status;

            internal EventToDbManager()
            {
                cur = new EventToDb[DEEPTH];
            }

            internal void Add(short type, ushort year, byte month, byte day, byte hour, byte minute, byte second, ushort value)
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

                cur[counter].type = type;
                cur[counter].year = year;
                cur[counter].month = month;
                cur[counter].day = day;
                cur[counter].hour = hour;
                cur[counter].minute = minute;
                cur[counter].second = second;
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
                        str += "," + etd.type + "," + etd.value + ")";
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
                internal short type;
                internal ushort year;
                internal byte month;
                internal byte day;
                internal byte hour;
                internal byte minute;
                internal byte second;
                internal ushort value;
            }
        }
    }
}
