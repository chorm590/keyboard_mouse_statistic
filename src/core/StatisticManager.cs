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

            if (typeCode < 256)
            {
                kbSingleKeyPressed(typeCode);
                sttKbTotal.Value++;
            }
            else if ((typeCode >= Constants.ComboKey.LC_LS) && (typeCode <= Constants.ComboKey.QUADRA))
            {
                sttComboKeyTotal.Value++;
                kbComboKeyPressed(typeCode);
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
        }
    }
}
