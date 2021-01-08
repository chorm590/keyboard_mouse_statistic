using System;
using System.Collections.Generic;
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
        

        private List<Event> sttMsKey; //鼠标单键点击总数。

        private List<Event> sttOthers; //分时段的统计，每日、每月、每年。

        
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
        }

        internal void shutdown()
        {
            //TODO flush all data to disk.
        }

        internal void EventHappen(int typeCode, DateTime time)
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
            }
            else if ((typeCode >= Constants.ComboKey.LC_LS) && (typeCode <= Constants.ComboKey.QUADRA))
            {
                sttComboKeyTotal.Value++;
                kbComboKeyPressed(typeCode);
            }
            else
            {
                // Do nothing
            }
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
                sttKbSkTop1.Desc = Constants.Keyboard.Keys[(byte)(sttKbSingleKey[0].Type.Code)].DisplayName + " [" + sttKbSingleKey[0].Value + "次]";
            }

            if (sttKbSingleKey[1].Value > 0)
            {
                sttKbSkTop2.Desc = Constants.Keyboard.Keys[(byte)(sttKbSingleKey[1].Type.Code)].DisplayName + " [" + sttKbSingleKey[1].Value + "次]";
            }

            if (sttKbSingleKey[2].Value > 0)
            {
                sttKbSkTop3.Desc = Constants.Keyboard.Keys[(byte)(sttKbSingleKey[2].Type.Code)].DisplayName + " [" + sttKbSingleKey[2].Value + "次]";
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
                sttKbCkTop1.Desc = sttKbComboKey[0].Type.Desc + " [" + sttKbComboKey[0].Value + "次]";
            }

            if (sttKbComboKey[1].Value > 0)
            {
                sttKbCkTop2.Desc = sttKbComboKey[1].Type.Desc + " [" + sttKbComboKey[1].Value + "次]";
            }

            if (sttKbComboKey[2].Value > 0)
            {
                sttKbCkTop3.Desc = sttKbComboKey[2].Type.Desc + " [" + sttKbComboKey[2].Value + "次]";
            }
        }
    }
}
