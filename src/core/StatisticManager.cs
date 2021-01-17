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
                if (instance is null)
                    instance = new StatisticManager();

                return instance;
            }
        }

        private readonly Statistic statisticGlobal;
        private readonly Statistic statisticYear;
        private readonly Statistic statisticMonth;
        private readonly Statistic statisticDay;
        private readonly HourStatistic[] hourStatistic;

        private StatisticManager()
        {
            //Statistic of today
            SttKeyboardTotalToday = new Record()
            {
                Type = Constants.TypeNumber.KEYBOARD_TOTAL
            };
            SttMouseTotalToday = new Record()
            {
                Type = Constants.TypeNumber.MOUSE_TOTAL
            };
            SttMostOpHourToday = new Record()
            {
                Type = Constants.TypeNumber.INVALID
            };

            //Initialize timer
            /*
                Bug提示：Timer检查时间有一定的间隔，且这一间隔不能缩的很短。再加上程序使用的时间系统是TimeManager中的记录的模糊时间值。
                因此程序的时间系统同步真实时间存在延迟。这一延迟可能会导致键鼠事件记录出错。
                例如：在小时统计数据中，当操作系统日期刚切换到程序日期切换之前这一时期内产生的原本属于新一天的记录会被误认为是前一天0点的数据。
                这一Bug看起来是一个无解的/解决成本很高的问题，目前采用规避的办法应付解决。
                规避方案：在每次存储前对比操作系统时间与程序时间。发现操作系统已是新一天时，将小时数据强行存到前一点23点。
                chorm, 2021-01-18 10:51
             */
            tool.Timer.RegisterTimerCallback(TimingStorage); //Must be ahead of 'DateWatcher'
            tool.Timer.RegisterTimerCallback(DateWatcher);

            statisticGlobal = new Statistic();
            statisticYear = new Statistic();
            statisticMonth = new Statistic();
            statisticDay = new Statistic();
            hourStatistic = new HourStatistic[24];

            if (SQLiteManager.GetInstance.Init())
            {
                QueryStatisticFromDB(SQLiteManager.GLOBAL_RECORD, statisticGlobal);
                if (statisticGlobal.KeyboardTotal.Value is 0)
                {
                    new Thread(() => {
                        DateTime niw = DateTime.Now;
                        Logger.v(TAG, "Init table begin, " + niw.Minute + ":" + niw.Second + "." + niw.Millisecond);

                        SQLiteManager.GetInstance.InitTables();
                        //year,month,day三张表的时间要初始化。
                        QueryStatisticFromDB(SQLiteManager.YEAR_RECORD, statisticYear);
                        QueryStatisticFromDB(SQLiteManager.YEAR_RECORD, statisticMonth);

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

                        QueryStatisticFromDB(SQLiteManager.YEAR_RECORD, statisticYear);
                        QueryStatisticFromDB(SQLiteManager.YEAR_RECORD, statisticMonth);
                        QueryDayStatisticFromDB();
                        QueryHourStatisticFromDB();

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

        /// <summary>
        /// 定时执行函数，约每60秒执行一次。主要在此将新数据存储到数据库中。
        /// </summary>
        private void TimingStorage(object state)
        {
            DateTime now = DateTime.Now;
            Logger.v(TAG, "TimingStorage, now1 " + now.Minute + ":" + now.Second + "." + now.Millisecond);
            //storage the global record.
            bool globalTransaction = false;
            bool yearTransaction = false;

            // [1/2]keyboard event
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
                        SQLiteManager.GetInstance.UpdateDay(Constants.TypeNumber.KEYBOARD_TOTAL, statisticDay.KeyboardTotal.Value);
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
                            SQLiteManager.GetInstance.UpdateDay(Constants.TypeNumber.KEYBOARD_COMBOL_TOTAL, statisticDay.KeyboardComboTotal.Value);
                        }
                    }

                    //Single key
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
                                    SQLiteManager.GetInstance.UpdateDay(rco.Type, rco.Value);
                                    rco.IsUpdated = false;
                                }
                            }
                        }
                    }
                }
            }

            // [2/2]mouse event
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
                    SQLiteManager.GetInstance.UpdateDay(Constants.TypeNumber.MOUSE_TOTAL, statisticDay.MouseTotal.Value);

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
                            SQLiteManager.GetInstance.UpdateDay(Constants.TypeNumber.MOUSE_LEFT_BTN, statisticDay.MouseLeftBtn.Value);
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
                            SQLiteManager.GetInstance.UpdateDay(Constants.TypeNumber.MOUSE_RIGHT_BTN, statisticDay.MouseRightBtn.Value);
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
                            SQLiteManager.GetInstance.UpdateDay(Constants.TypeNumber.MOUSE_WHEEL_FORWARD, statisticDay.MouseWheelForward.Value);
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
                            SQLiteManager.GetInstance.UpdateDay(Constants.TypeNumber.MOUSE_WHEEL_BACKWARD, statisticDay.MouseWheelBackward.Value);
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
                            SQLiteManager.GetInstance.UpdateDay(Constants.TypeNumber.MOUSE_SIDE_FORWARD, statisticDay.MouseSideKeyForward.Value);
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
                            SQLiteManager.GetInstance.UpdateDay(Constants.TypeNumber.MOUSE_SIDE_BACKWARD, statisticDay.MouseSideKeyBackward.Value);
                        }
                    }
                }
            }

            if(globalTransaction)
                SQLiteManager.GetInstance.CommitTransaction(SQLiteManager.GLOBAL_RECORD);
            if (yearTransaction)
                SQLiteManager.GetInstance.CommitTransaction(SQLiteManager.YEAR_RECORD);

            //hour-statistic data
            //为了避免事务的干扰，将小时统计统计数据放在这里存储。2021-01-15 14:30
            HourStatistic[] hstmp = null;
            for (byte i = 0; i < 24; i++) //maybe the system clock was changed after last time to storage.
            {
                if (hstmp is null)
                {
                    if (hourStatistic[i].IsKBUpdated || hourStatistic[i].IsMSUpdated)
                    {
                        hstmp = new HourStatistic[24];

                        SQLiteDataReader reader = SQLiteManager.GetInstance.QueryHourStatistic();
                        if (reader != null)
                        {
                            byte hour;
                            short type;
                            int val;
                            while (reader.Read())
                            {
                                type = reader.GetInt16(0);
                                val = reader.GetInt32(1);
                                hour = reader.GetByte(2);

                                if (type == Constants.TypeNumber.KEYBOARD_TOTAL)
                                {
                                    hstmp[hour].KbTotal = (uint)val;
                                    hstmp[hour].RecordStatus |= 1;
                                }
                                else if (type == Constants.TypeNumber.MOUSE_TOTAL)
                                {
                                    hstmp[hour].MsTotal = (uint)val;
                                    hstmp[hour].RecordStatus |= 2;
                                }
                            }
                            reader.Close();
                        }
                    }
                }

                if (hourStatistic[i].IsKBUpdated) //It won't be a lot
                {
                    Logger.v(TAG, "hour " + i + " kb value in db:" + hstmp[i].KbTotal);
                    
                    if ((hstmp[i].RecordStatus & 1) == 0)
                    {
                        SQLiteManager.GetInstance.InsertHour(Constants.TypeNumber.KEYBOARD_TOTAL, hourStatistic[i].KbTotal, i);
                        hourStatistic[i].IsKBUpdated = false;
                    }
                    else if (hstmp[i].KbTotal != hourStatistic[i].KbTotal)
                    {
                        SQLiteManager.GetInstance.UpdateHour(Constants.TypeNumber.KEYBOARD_TOTAL, hourStatistic[i].KbTotal, i);
                        hourStatistic[i].IsKBUpdated = false;
                    }
                }

                if (hourStatistic[i].IsMSUpdated)
                {
                    Logger.v(TAG, "hour " + i + " ms value in db:" + hstmp[i].MsTotal);
                    if ((hstmp[i].RecordStatus & 2) == 0)
                    {
                        SQLiteManager.GetInstance.InsertHour(Constants.TypeNumber.MOUSE_TOTAL, hourStatistic[i].MsTotal, i);
                        hourStatistic[i].IsMSUpdated = false;
                    }
                    else if (hstmp[i].MsTotal != hourStatistic[i].MsTotal)
                    {
                        SQLiteManager.GetInstance.UpdateHour(Constants.TypeNumber.MOUSE_TOTAL, hourStatistic[i].MsTotal, i);
                        hourStatistic[i].IsMSUpdated = false;
                    }
                }
            }

            now = DateTime.Now;
            Logger.v(TAG, "TimingStorage, now2 " + now.Minute + ":" + now.Second + "." + now.Millisecond);
        }


        /// <summary>
        /// 检查是否已经换日，若是，更新TimeManager中的时间，然后更换数据库或数据库并初始化。
        /// </summary>
        private void DateWatcher(object state)
        {
            DateTime now = DateTime.Now;

            int status = CheckDate(now, TimeManager.TimeUsing);
            Logger.v(TAG, "check date ret:" + status);

            switch (status)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                    TimeManager.TimeUsing = now; //Important
                    break;
            }

            /*
                Important:接受因更换的瞬间而造成的数据丢失问题。2021-01-17 22:49
             */
            switch (status)
            {
                case 1: //year switch
                    ////1. 更换数据库文件
                    //SQLiteManager.GetInstance.SwitchYearDB();

                    ////2. 重置小时统计表
                    //ClearHourStatistic();
                    //QueryHourStatisticFromDB();

                    ////3. 更新日统计表
                    ////SwitchDayTable();

                    ////4. 更新月统计表
                    //ClearStatistic(statisticMonth);
                    //QueryStatisticFromDB(SQLiteManager.YEAR_RECORD, statisticMonth);

                    ////5. 更新年统计表
                    //SwitchYearTable();
                    break;
                case 2: //month switch
                    ////1. 更新小时统计表
                    //ClearHourStatistic();
                    //QueryHourStatisticFromDB();

                    ////2. 更新日统计
                    //ClearStatistic(statisticDay);
                    //QueryStatisticFromDB(SQLiteManager.YEAR_RECORD, statisticDay);

                    ////3. 更新月统计
                    //ClearStatistic(statisticMonth);
                    //QueryStatisticFromDB(SQLiteManager.YEAR_RECORD, statisticMonth);
                    break;
                case 3: //day switch
                    //1. 更新小时统计表
                    SwitchHourStatistic();

                    //2.更新日统计
                    SwitchDayStatistic();
                    break;
            }
        }

        private int CheckDate(DateTime now, DateTime timeUsing)
        {
            if (now.Year != timeUsing.Year)
                return 1;
            else if (now.Month != timeUsing.Month)
                return 2;
            else if (now.Day != timeUsing.Day)
                return 3;
            else if (now.Hour != timeUsing.Hour)
                return 4;
            else
                return 0;
        }

        private void SwitchHourStatistic()
        {
            ClearHourStatistic();
            QueryHourStatisticFromDB();
        }

        /// <summary>
        /// 日期更换，重新准备日统计记录。
        /// </summary>
        private void SwitchDayStatistic()
        {
            ClearStatistic(statisticDay);
            SQLiteDataReader reader = SQLiteManager.GetInstance.TryQueryDayStatistic();
            if (reader != null)
            {
                //将查上来的数据记录到statisticDay中。
                while (reader.Read())
                {
                    ApplyKeyboardRecordFromDB(reader.GetInt16(0), reader.GetInt32(1), statisticDay);
                }
            }
        }

        /// <summary>
        /// 日期更换
        /// </summary>
        private void SwitchMonthTable()
        {
            
        }

        /// <summary>
        /// 检查当前年表是否存在。若不存在，则新建并插入初始值。若存在，则读取数据并应用到 statisticYear 中。2021-01-17 21:47
        /// </summary>
        private void SwitchYearTable()
        {
            SQLiteDataReader reader = SQLiteManager.GetInstance.InitYearTable();
            if (reader != null)
            {
                //从SQLiteDataReader中提取数据出来。

            }
        }

        private void ClearHourStatistic()
        {
            for (byte i = 0; i < 24; i++)
            {
                hourStatistic[i].RecordStatus = 0;
                hourStatistic[i].IsKBUpdated = false;
                hourStatistic[i].IsMSUpdated = false;
                hourStatistic[i].KbTotal = 0;
                hourStatistic[i].MsTotal = 0;
            }
            Logger.v(TAG, "hour statistic cleared");
        }

        private void ClearStatistic(Statistic stt)
        {
            stt.KeyboardTotal.Value = 0;
            stt.KeyboardTotal.IsUpdated = false;

            stt.KeyboardComboTotal.Value = 0;
            stt.KeyboardComboTotal.IsUpdated = false;

            stt.MouseTotal.Value = 0;
            stt.MouseTotal.IsUpdated = false;

            stt.MouseLeftBtn.Value = 0;
            stt.MouseLeftBtn.IsUpdated = false;

            stt.MouseRightBtn.Value = 0;
            stt.MouseRightBtn.IsUpdated = false;

            stt.MouseWheelForward.Value = 0;
            stt.MouseWheelForward.IsUpdated = false;

            stt.MouseWheelBackward.Value = 0;
            stt.MouseWheelBackward.IsUpdated = false;

            stt.MouseWheelClick.Value = 0;
            stt.MouseWheelClick.IsUpdated = false;

            stt.MouseSideKeyForward.Value = 0;
            stt.MouseSideKeyForward.IsUpdated = false;

            stt.MouseSideKeyBackward.Value = 0;
            stt.MouseSideKeyBackward.IsUpdated = false;

            foreach (Record rco in stt.KeyboardKeys)
            {
                rco.Value = 0;
                rco.IsUpdated = false;
            }
            Logger.v(TAG, "clear statistic finish");
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
                //跨日处理
                if ((time.Hour == 0) && (time.Day != TimeManager.TimeUsing.Day))
                {
                    hourStatistic[23].KbTotal++;
                    hourStatistic[23].IsKBUpdated = true;
                }
                else if (time.Hour == 23 && TimeManager.TimeUsing.Hour == 0)
                {
                    hourStatistic[0].KbTotal++;
                    hourStatistic[0].IsKBUpdated = true;
                }
                else
                {
                    hourStatistic[time.Hour].KbTotal++;
                    hourStatistic[time.Hour].IsKBUpdated = true;
                }

                if (fkey > 0)
                {
                    statisticGlobal.KeyboardComboTotal.Value++;
                    statisticGlobal.KeyboardComboTotal.Desc = GetDesc1(statisticGlobal.KeyboardComboTotal.Value);

                    statisticYear.KeyboardComboTotal.Value++;
                    statisticMonth.KeyboardComboTotal.Value++;
                    statisticDay.KeyboardComboTotal.Value++;
                }

                KbSingleKeyPressed(typeCode);
                CountHoursRecord();
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
                    statisticYear.MouseWheelBackward.Value += (ushort)mouseData;
                    statisticMonth.MouseWheelBackward.Value += (ushort)mouseData;
                    statisticDay.MouseWheelBackward.Value += (ushort)mouseData;
                    break;
                case Constants.TypeNumber.MOUSE_WHEEL_FORWARD:
                    statisticGlobal.MouseWheelForward.Value += (ushort)mouseData;
                    statisticGlobal.MouseWheelForward.Desc = GetDesc1(statisticGlobal.MouseWheelForward.Value);
                    statisticYear.MouseWheelForward.Value += (ushort)mouseData;
                    statisticMonth.MouseWheelForward.Value += (ushort)mouseData;
                    statisticDay.MouseWheelForward.Value += (ushort)mouseData;
                    break;
                case Constants.TypeNumber.MOUSE_WHEEL_CLICK:
                    statisticGlobal.MouseWheelClick.Value++;
                    statisticGlobal.MouseWheelClick.Desc = GetDesc1(statisticGlobal.MouseWheelClick.Value);
                    statisticYear.MouseWheelClick.Value++;
                    statisticMonth.MouseWheelClick.Value++;
                    statisticDay.MouseWheelClick.Value++;
                    break;
                case Constants.TypeNumber.MOUSE_LEFT_BTN:
                    statisticGlobal.MouseLeftBtn.Value++;
                    statisticGlobal.MouseLeftBtn.Desc = GetDesc1(statisticGlobal.MouseLeftBtn.Value);
                    statisticYear.MouseLeftBtn.Value++;
                    statisticMonth.MouseLeftBtn.Value++;
                    statisticDay.MouseLeftBtn.Value++;
                    break;
                case Constants.TypeNumber.MOUSE_RIGHT_BTN:
                    statisticGlobal.MouseRightBtn.Value++;
                    statisticGlobal.MouseRightBtn.Desc = GetDesc1(statisticGlobal.MouseRightBtn.Value);
                    statisticYear.MouseRightBtn.Value++;
                    statisticMonth.MouseRightBtn.Value++;
                    statisticDay.MouseRightBtn.Value++;
                    break;
                case Constants.TypeNumber.MOUSE_SIDE_FORWARD:
                    statisticGlobal.MouseSideKeyForward.Value++;
                    statisticGlobal.MouseSideKeyForward.Desc = GetDesc1(statisticGlobal.MouseSideKeyForward.Value);
                    statisticYear.MouseSideKeyForward.Value++;
                    statisticMonth.MouseSideKeyForward.Value++;
                    statisticDay.MouseSideKeyForward.Value++;
                    break;
                case Constants.TypeNumber.MOUSE_SIDE_BACKWARD:
                    statisticGlobal.MouseSideKeyBackward.Value++;
                    statisticGlobal.MouseSideKeyBackward.Desc = GetDesc1(statisticGlobal.MouseSideKeyBackward.Value);
                    statisticYear.MouseSideKeyBackward.Value++;
                    statisticMonth.MouseSideKeyBackward.Value++;
                    statisticDay.MouseSideKeyBackward.Value++;
                    break;
                default:
                    break;
            }

            switch (typeCode)
            {
                case Constants.TypeNumber.MOUSE_WHEEL_BACKWARD:
                case Constants.TypeNumber.MOUSE_WHEEL_FORWARD:
                    statisticGlobal.MouseTotal.Value += (ushort)mouseData;
                    statisticYear.MouseTotal.Value += (ushort)mouseData;
                    statisticMonth.MouseTotal.Value += (ushort)mouseData;
                    statisticDay.MouseTotal.Value += (ushort)mouseData;
                    //跨日处理。只考虑自然跨日，忽略手动调整时间导致的日期改变。
                    if ((time.Hour == 0) && (time.Day != TimeManager.TimeUsing.Day))
                    {
                        hourStatistic[23].MsTotal += (ushort)mouseData;
                        hourStatistic[23].IsMSUpdated = true;
                    }
                    else if (time.Hour == 23 && TimeManager.TimeUsing.Hour == 0)
                    {
                        hourStatistic[0].MsTotal++;
                        hourStatistic[0].IsMSUpdated = true;
                    }
                    else
                    {
                        hourStatistic[time.Hour].MsTotal += (ushort)mouseData;
                        hourStatistic[time.Hour].IsMSUpdated = true;
                    }
                    break;
                case Constants.TypeNumber.MOUSE_WHEEL_CLICK:
                case Constants.TypeNumber.MOUSE_LEFT_BTN:
                case Constants.TypeNumber.MOUSE_RIGHT_BTN:
                case Constants.TypeNumber.MOUSE_SIDE_FORWARD:
                case Constants.TypeNumber.MOUSE_SIDE_BACKWARD:
                    statisticGlobal.MouseTotal.Value++;
                    statisticYear.MouseTotal.Value++;
                    statisticMonth.MouseTotal.Value++;
                    statisticDay.MouseTotal.Value++;
                    //跨日处理。
                    if ((time.Hour == 0) && (time.Day != TimeManager.TimeUsing.Day))
                    {
                        hourStatistic[23].MsTotal++;
                        hourStatistic[23].IsMSUpdated = true;
                    }
                    else
                    {
                        hourStatistic[time.Hour].MsTotal++;
                        hourStatistic[time.Hour].IsMSUpdated = true;
                    }
                    break;
                default:
                    break;
            }

            CountHoursRecord();
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
        /// 从数据库中读取统计数据。
        /// </summary>
        private void QueryStatisticFromDB(byte which, Statistic statistic)
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

        private void QueryDayStatisticFromDB()
        {
            SQLiteDataReader reader = SQLiteManager.GetInstance.QueryDayStatistic();
            if (reader != null)
            {
                while (reader.Read())
                {
                    ApplyKeyboardRecordFromDB(reader.GetInt16(0), reader.GetInt32(1), statisticDay);
                }
            }
        }

        /// <summary>
        /// 程序启动时从数据库中读取今天的小时统计数据。
        /// </summary>
        private void QueryHourStatisticFromDB()
        {
            SQLiteDataReader reader = SQLiteManager.GetInstance.QueryHourStatistic();
            if (reader != null)
            {
                byte hour;
                int type;
                int value;
                while (reader.Read())
                {
                    type = reader.GetInt16(0);
                    value = reader.GetInt32(1);
                    hour = reader.GetByte(2);

                    if (type == Constants.TypeNumber.KEYBOARD_TOTAL)
                    {
                        hourStatistic[hour].KbTotal = (uint)value;
                        hourStatistic[hour].RecordStatus |= 1;
                    }
                    else if (type == Constants.TypeNumber.MOUSE_TOTAL)
                    {
                        hourStatistic[hour].MsTotal = (uint)value;
                        hourStatistic[hour].RecordStatus |= 2;
                    }
                }
                reader.Close();

                //整理数据库中的数据。
                CountHoursRecord();
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

        /*
         开放给窗口获取Binding源的接口。
         */
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

        /// <summary>
        /// 整理今日内的数据，使结果显示在窗口上。2021-01-15 15:37
        /// </summary>
        private void CountHoursRecord()
        {
            byte mostHour = 0;
            uint mostTimes = 0;
            uint tmp;
            
            SttKeyboardTotalToday.Value = 0;
            SttMouseTotalToday.Value = 0;

            for (byte i = 0; i < 24; i++)
            {
                SttKeyboardTotalToday.Value += hourStatistic[i].KbTotal;
                SttMouseTotalToday.Value += hourStatistic[i].MsTotal;

                tmp = hourStatistic[i].KbTotal + hourStatistic[i].MsTotal;
                if (mostTimes < tmp)
                {
                    mostHour = i;
                    mostTimes = tmp;
                }
            }

            if (SttKeyboardTotalToday.Value > 0)
            {
                SttKeyboardTotalToday.Desc = GetDesc1(SttKeyboardTotalToday.Value);
            }

            if (SttMouseTotalToday.Value > 0)
            {
                SttMouseTotalToday.Desc = GetDesc1(SttMouseTotalToday.Value);
            }

            if (mostTimes > 0)
            {
                SttMostOpHourToday.Value = mostTimes;
                SttMostOpHourToday.Desc = GetDesc2(mostHour.ToString(), mostTimes);
            }
        }

        /// <summary>
        /// 将从数据库中读上来的事件记录存储到内存中的statistic对象中。2021-01-18 10:12
        /// </summary>
        private void ApplyKeyboardRecordFromDB(short type, int value, Statistic statistic)
        {
            if (type > 0 && type < 256)
            {
                foreach (Record rco in statistic.KeyboardKeys)
                {
                    if (rco.Type == type)
                    {
                        rco.Value += (uint)value;
                        rco.IsUpdated = false;
                        break;
                    }
                }
            }
            else
            {
                switch ((ushort)type)
                {
                    case Constants.TypeNumber.KEYBOARD_TOTAL:
                        statistic.KeyboardTotal.Value += (uint)value;
                        statistic.KeyboardTotal.IsUpdated = false;
                        break;
                    case Constants.TypeNumber.KEYBOARD_COMBOL_TOTAL:
                        statistic.KeyboardComboTotal.Value += (uint)value;
                        statistic.KeyboardComboTotal.IsUpdated = false;
                        break;
                    case Constants.TypeNumber.MOUSE_TOTAL:
                        statistic.MouseTotal.Value += (uint)value;
                        statistic.MouseTotal.IsUpdated = false;
                        break;
                    case Constants.TypeNumber.MOUSE_LEFT_BTN:
                        statistic.MouseLeftBtn.Value += (uint)value;
                        statistic.MouseLeftBtn.IsUpdated = false;
                        break;
                    case Constants.TypeNumber.MOUSE_RIGHT_BTN:
                        statistic.MouseRightBtn.Value += (uint)value;
                        statistic.MouseRightBtn.IsUpdated = false;
                        break;
                    case Constants.TypeNumber.MOUSE_WHEEL_FORWARD:
                        statistic.MouseWheelForward.Value += (uint)value;
                        statistic.MouseWheelForward.IsUpdated = false;
                        break;
                    case Constants.TypeNumber.MOUSE_WHEEL_BACKWARD:
                        statistic.MouseWheelBackward.Value += (uint)value;
                        statistic.MouseWheelBackward.IsUpdated = false;
                        break;
                    case Constants.TypeNumber.MOUSE_SIDE_FORWARD:
                        statistic.MouseSideKeyForward.Value += (uint)value;
                        statistic.MouseSideKeyForward.IsUpdated = false;
                        break;
                    case Constants.TypeNumber.MOUSE_SIDE_BACKWARD:
                        statistic.MouseSideKeyBackward.Value += (uint)value;
                        statistic.MouseSideKeyBackward.IsUpdated = false;
                        break;
                }
            }
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
                };

                KeyboardComboTotal = new Record
                {
                    Type = Constants.TypeNumber.KEYBOARD_COMBOL_TOTAL,
                };

                KeyboardSkTop1 = new Record
                {
                    Type = Constants.TypeNumber.KB_SK_TOP1,
                };

                KeyboardSkTop2 = new Record
                {
                    Type = Constants.TypeNumber.KB_SK_TOP2,
                };

                KeyboardSkTop3 = new Record
                {
                    Type = Constants.TypeNumber.KB_SK_TOP3,
                };

                KeyboardSkTop4 = new Record
                {
                    Type = Constants.TypeNumber.KB_SK_TOP4,
                };

                KeyboardSkTop5 = new Record
                {
                    Type = Constants.TypeNumber.KB_SK_TOP5,
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
                    Type = Constants.TypeNumber.MOUSE_TOTAL,
                };

                MouseLeftBtn = new Record
                {
                    Type = Constants.TypeNumber.MOUSE_LEFT_BTN,
                };

                MouseRightBtn = new Record
                {
                    Type = Constants.TypeNumber.MOUSE_RIGHT_BTN,
                };

                MouseWheelForward = new Record
                {
                    Type = Constants.TypeNumber.MOUSE_WHEEL_FORWARD,
                };

                MouseWheelBackward = new Record
                {
                    Type = Constants.TypeNumber.MOUSE_WHEEL_BACKWARD,
                };

                MouseWheelClick = new Record
                {
                    Type = Constants.TypeNumber.MOUSE_WHEEL_CLICK,
                };

                MouseSideKeyForward = new Record
                {
                    Type = Constants.TypeNumber.MOUSE_SIDE_FORWARD,
                };

                MouseSideKeyBackward = new Record
                {
                    Type = Constants.TypeNumber.MOUSE_SIDE_BACKWARD,
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
                    ListStatus = IDLE;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private struct HourStatistic
        {
            /// <summary>
            /// 第0位表示当前小时值键盘记录是否已插入过记录。第1位表示鼠标记录。
            /// 用于指导在定时存储时小时记录是该插入还是更新。
            /// </summary>
            internal byte RecordStatus;
            internal uint KbTotal;
            internal uint MsTotal;
            internal bool IsKBUpdated;
            internal bool IsMSUpdated;
        }

        internal Record SttKeyboardTotalToday
        {
            get;
        }

        internal Record SttMouseTotalToday
        {
            get;
        }

        internal Record SttMostOpHourToday //今日操作键鼠最多的时间段
        {
            get;
        }
    }
}
