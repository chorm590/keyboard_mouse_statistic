using System;
using System.Collections.Generic;
using System.Text;
using KMS.src.core;

namespace KMS.src.db
{
    /// <summary>
    /// 统计信息管理。亦可简单理解成是事件的内存数据库。
    /// </summary>
    class StatisticManager
    {
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
        internal Event SttKeyboardTotal
        {
            get
            {
                return sttKbTotal;
            }
        }

        //TODO 创建各单键的敲击统计


        private StatisticManager()
        {
            sttKbTotal = new Event
            {
                Type = (ushort)Constants.DbType.KB_ALL
            };
        }
    }
}
