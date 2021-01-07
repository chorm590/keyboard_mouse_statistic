using System;
using System.Collections.Generic;
using KMS.src.core;
using KMS.src.tool;

namespace KMS.src.db
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
                Type = Constants.KbAll
            };

            /*// 2
            sttKbSingleKey = new List<Event>(130);
            foreach (core.Type tp in Constants.keys)
            {
                if (tp.Desc is null)
                    continue;

                sttKbSingleKey.Add(new Event(tp));
            }
            Logger.v(TAG, "sttKbSingleKey capacity:" + sttKbSingleKey.Capacity + ",count:" + sttKbSingleKey.Count);

            // 3
            sttKbComboKey = new List<Event>(50);
            foreach (core.Type tp in Constants.ComboKeyType)
            {
                sttKbComboKey.Add(new Event(tp));
            }
            */
        }

        internal void shutdown()
        {
            //TODO flush all data to disk.
        }
    }
}
