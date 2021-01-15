using KMS.src.core;
using KMS.src.tool;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;

namespace KMS.src.db
{
    class SQLiteManager
    {
        private const string TAG = "SQLiteManager";

        internal const byte GLOBAL_RECORD = 1;
        internal const byte YEAR_RECORD = 2;

        private const string DATABASE_DIR = "data";
        private const string GLOBAL_TABLE = "global";
        private const string YEAR_TABLE = "year";
        private const string MONTH_TABLE = "month";
        private const string DAY_TABLE = "day";
        private const string HOUR_TABLE = "hour";

        private static SQLiteManager instance;

        private SQLiteHelper totalDatabase; //存储全局统计数据
        private SQLiteHelper yearDatabase; //存储本年度统计数据

        /// <summary>
        /// 全局统计数据数据库文件。
        /// </summary>
        private string GlobalDb
        {
            get
            {
                return DATABASE_DIR + "/kmsall.db";
            }
        }

        /// <summary>
        /// 本年度统计数据库文件。
        /// </summary>
        private string YearDb
        {
            get
            {
                return DATABASE_DIR + "/kms" + TimeManager.TimeUsing.Year + ".db";
            }
        }

        internal static SQLiteManager GetInstance
        {
            get
            {
                if (instance == null)
                    instance = new SQLiteManager();

                return instance;
            }
        }

        private SQLiteManager()
        {
            //make sure the data path ok
            if (File.Exists(DATABASE_DIR))
            {
                File.Move(DATABASE_DIR, DATABASE_DIR + "_rename");
            }
            if (!Directory.Exists(DATABASE_DIR))
            {
                Directory.CreateDirectory(DATABASE_DIR);
            }

            totalDatabase = new SQLiteHelper();
            yearDatabase = new SQLiteHelper();
        }

        /// <summary>
        /// 连接到全局数据库与年度数据库。
        /// </summary>
        /// <returns>true 两个数据库均已正常连接。false 未能全部正常创建连接。</returns>
        internal bool Init()
        {
            if (totalDatabase is null || yearDatabase is null)
                return false;

            if (!totalDatabase.openDatabase(GlobalDb))
                return false;

            if (!yearDatabase.openDatabase(YearDb))
                return false;

            return true;
        }

        /// <summary>
        /// 检查全局库、年度库中各对应表是否存在，若不存在，则创建表并为各统计字段插入初始值。
        /// 2021-01-14 12:07
        /// </summary>
        internal void InitTable()
        {
            if (!totalDatabase.IsTableExist(GLOBAL_TABLE))
            {
                Logger.v(TAG, "creating global table");
                totalDatabase.ExecuteSQL("CREATE TABLE " + GLOBAL_TABLE + "(type SMALLINT PRIMARY KEY NOT NULL,value INTEGER)");
                if (totalDatabase.BeginTransaction())
                {
                    InsertGlobal(Constants.TypeNumber.KEYBOARD_TOTAL, 0);
                    InsertGlobal(Constants.TypeNumber.KEYBOARD_COMBOL_TOTAL, 0);
                    InsertGlobal(Constants.TypeNumber.MOUSE_TOTAL, 0);
                    InsertGlobal(Constants.TypeNumber.MOUSE_LEFT_BTN, 0);
                    InsertGlobal(Constants.TypeNumber.MOUSE_RIGHT_BTN, 0);
                    InsertGlobal(Constants.TypeNumber.MOUSE_WHEEL_FORWARD, 0);
                    InsertGlobal(Constants.TypeNumber.MOUSE_WHEEL_BACKWARD, 0);
                    InsertGlobal(Constants.TypeNumber.MOUSE_WHEEL_CLICK, 0);
                    InsertGlobal(Constants.TypeNumber.MOUSE_SIDE_FORWARD, 0);
                    InsertGlobal(Constants.TypeNumber.MOUSE_SIDE_BACKWARD, 0);

                    Dictionary<byte, Key>.KeyCollection keys = Constants.Keyboard.Keys;
                    foreach (byte keycode in keys)
                    {
                        InsertGlobal(keycode, 0);
                    }

                    totalDatabase.CommitTransaction();
                }
                else
                {
                    MessageBox.Show("Init database failed");
                    throw new Exception();
                }
            }

            if (!yearDatabase.IsTableExist(YEAR_TABLE))
            {
                Logger.v(TAG, "creating year table");
                yearDatabase.ExecuteSQL("CREATE TABLE " + YEAR_TABLE + "(type SMALLINT PRIMARY KEY NOT NULL,value INTEGER,year SMALLINT NOT NULL)");
                if (yearDatabase.BeginTransaction())
                {
                    InsertYear(Constants.TypeNumber.KEYBOARD_TOTAL);
                    InsertYear(Constants.TypeNumber.KEYBOARD_COMBOL_TOTAL);
                    InsertYear(Constants.TypeNumber.MOUSE_TOTAL);
                    InsertYear(Constants.TypeNumber.MOUSE_LEFT_BTN);
                    InsertYear(Constants.TypeNumber.MOUSE_RIGHT_BTN);
                    InsertYear(Constants.TypeNumber.MOUSE_WHEEL_FORWARD);
                    InsertYear(Constants.TypeNumber.MOUSE_WHEEL_BACKWARD);
                    InsertYear(Constants.TypeNumber.MOUSE_WHEEL_CLICK);
                    InsertYear(Constants.TypeNumber.MOUSE_SIDE_FORWARD);
                    InsertYear(Constants.TypeNumber.MOUSE_SIDE_BACKWARD);

                    Dictionary<byte, Key>.KeyCollection keys = Constants.Keyboard.Keys;
                    foreach (byte keycode in keys)
                    {
                        InsertYear(keycode);
                    }

                    yearDatabase.CommitTransaction();
                }
                else
                {
                    MessageBox.Show("Init database failed");
                    throw new Exception();
                }
            }

            if (!yearDatabase.IsTableExist(MONTH_TABLE))
            {
                Logger.v(TAG, "creating month table");
                yearDatabase.ExecuteSQL("CREATE TABLE " + MONTH_TABLE +
                    "(type SMALLINT NOT NULL,value INTEGER,year SMALLINT NOT NULL,month TINYINT NOT NULL)");
                if (yearDatabase.BeginTransaction())
                {
                    InsertMonth(Constants.TypeNumber.KEYBOARD_TOTAL);
                    InsertMonth(Constants.TypeNumber.KEYBOARD_COMBOL_TOTAL);
                    InsertMonth(Constants.TypeNumber.MOUSE_TOTAL);
                    InsertMonth(Constants.TypeNumber.MOUSE_LEFT_BTN);
                    InsertMonth(Constants.TypeNumber.MOUSE_RIGHT_BTN);
                    InsertMonth(Constants.TypeNumber.MOUSE_WHEEL_FORWARD);
                    InsertMonth(Constants.TypeNumber.MOUSE_WHEEL_BACKWARD);
                    InsertMonth(Constants.TypeNumber.MOUSE_WHEEL_CLICK);
                    InsertMonth(Constants.TypeNumber.MOUSE_SIDE_FORWARD);
                    InsertMonth(Constants.TypeNumber.MOUSE_SIDE_BACKWARD);

                    Dictionary<byte, Key>.KeyCollection keys = Constants.Keyboard.Keys;
                    foreach (byte keycode in keys)
                    {
                        InsertMonth(keycode);
                    }

                    yearDatabase.CommitTransaction();
                }
                else
                {
                    MessageBox.Show("Init database failed");
                    throw new Exception();
                }
            }

            if (!yearDatabase.IsTableExist(DAY_TABLE))
            {
                Logger.v(TAG, "creating day table");
                yearDatabase.ExecuteSQL("CREATE TABLE " + DAY_TABLE +
                    "(type SMALLINT NOT NULL,value INTEGER,year SMALLINT NOT NULL,month TINYINT NOT NULL,day TINYINT NOT NULL)");
                if (yearDatabase.BeginTransaction())
                {
                    InsertDay(Constants.TypeNumber.KEYBOARD_TOTAL);
                    InsertDay(Constants.TypeNumber.KEYBOARD_COMBOL_TOTAL);
                    InsertDay(Constants.TypeNumber.MOUSE_TOTAL);
                    InsertDay(Constants.TypeNumber.MOUSE_LEFT_BTN);
                    InsertDay(Constants.TypeNumber.MOUSE_RIGHT_BTN);
                    InsertDay(Constants.TypeNumber.MOUSE_WHEEL_FORWARD);
                    InsertDay(Constants.TypeNumber.MOUSE_WHEEL_BACKWARD);
                    InsertDay(Constants.TypeNumber.MOUSE_WHEEL_CLICK);
                    InsertDay(Constants.TypeNumber.MOUSE_SIDE_FORWARD);
                    InsertDay(Constants.TypeNumber.MOUSE_SIDE_BACKWARD);

                    Dictionary<byte, Key>.KeyCollection keys = Constants.Keyboard.Keys;
                    foreach (byte keycode in keys)
                    {
                        InsertDay(keycode);
                    }

                    yearDatabase.CommitTransaction();
                }
                else
                {
                    MessageBox.Show("Init database failed");
                    throw new Exception();
                }
            }

            if (!yearDatabase.IsTableExist(HOUR_TABLE))
            {
                Logger.v(TAG, "creating hour table");
                yearDatabase.ExecuteSQL("CREATE TABLE " + HOUR_TABLE +
                    "(type SMALLINT NOT NULL,value INTEGER,year SMALLINT NOT NULL,month TINYINT NOT NULL,day TINYINT NOT NULL,hour TINYINT NOT NULL");
                //Enough!
            }
        }

        internal void InsertGlobal(ushort type, uint value)
        {
            totalDatabase.ExecuteSQL("INSERT INTO " + GLOBAL_TABLE + " VALUES(" + type + "," + value + ")");
        }

        private void InsertYear(ushort type)
        {
            yearDatabase.ExecuteSQL("INSERT INTO " + YEAR_TABLE + " VALUES(" + type + ",0," + TimeManager.TimeUsing.Year + ")");
        }

        private void InsertMonth(ushort type)
        {
            yearDatabase.ExecuteSQL("INSERT INTO " + MONTH_TABLE + " VALUES(" + type + ",0," + TimeManager.TimeUsing.Year + "," + TimeManager.TimeUsing.Month + ")");
        }

        private void InsertDay(ushort type)
        {
            yearDatabase.ExecuteSQL("INSERT INTO " + DAY_TABLE + " VALUES(" + type + ",0," + TimeManager.TimeUsing.Year + "," + TimeManager.TimeUsing.Month + "," + TimeManager.TimeUsing.Day + ")");
        }

        internal void InsertHour(ushort type, byte hour, uint value)
        {
            Logger.v(TAG, "InsertHour,type:" + type + ",hour:" + hour + ",value:" + value);
            yearDatabase.ExecuteSQL("INSERT INTO " + HOUR_TABLE + " VALUES(" + type + "," + value + "," +
                TimeManager.TimeUsing.Year + "," + TimeManager.TimeUsing.Month + "," + TimeManager.TimeUsing.Day + "," + hour);
        }

        internal bool UseDatabase(string db)
        {
            if (db is null || db.Length == 0)
                return false;

            //return sqliteHelper.openDatabase(db);
            return false;
        }

        /// <summary>
        /// 查询指定日的详细数据。
        /// </summary>
        internal SQLiteDataReader GetEventDetail(byte day)
        {
            //if (sqliteHelper.isTableExist(DETAIL_TABLE_PREFIX + day + DETAIL_TABLE_SUFFIX))
            //{
            //    return sqliteHelper.QueryTable(DETAIL_TABLE_PREFIX + day + DETAIL_TABLE_SUFFIX);
            //}

            return null;
        }

        internal void close()
        {
            //if (sqliteHelper != null)
            //    sqliteHelper.closeDababase();
        }

        internal bool BeginTransaction(byte which)
        {
            if (which == GLOBAL_RECORD)
                return totalDatabase.BeginTransaction();
            else if (which == YEAR_RECORD)
                return yearDatabase.BeginTransaction();
            else
                return false;
        }

        internal void CommitTransaction(byte which)
        {
            if (which == GLOBAL_RECORD)
                totalDatabase.CommitTransaction();
            else if (which == YEAR_RECORD)
                yearDatabase.CommitTransaction();
        }

        internal void UpdateGlobal(ushort type, uint value)
        {
            Logger.v(TAG, "update global,type:" + type + ",value:" + value);
            totalDatabase.ExecuteSQL("UPDATE " + GLOBAL_TABLE + " SET value=" + value + " WHERE type=" + type);
        }

        internal void UpdateYear(ushort type, uint value, ushort year)
        {
            Logger.v(TAG, "update year,type:" + type + ",value:" + value + ",year:" + year);
            yearDatabase.ExecuteSQL("UPDATE " + YEAR_TABLE + " SET value=" + value + " WHERE type=" + type);
        }

        internal void UpdateMonth(ushort type, uint value, ushort year, byte month)
        {
            Logger.v(TAG, "update month,type:" + type + ",value:" + value + ",year:" + year + ",month:" + month);
            yearDatabase.ExecuteSQL("UPDATE " + MONTH_TABLE + " SET value=" + value + " WHERE type=" + type + " AND month=" + month);
        }

        internal void UpdateDay(ushort type, uint value, ushort year, byte month, byte day)
        {
            Logger.v(TAG, "update day,type:" + type + ",value:" + value + ",year:" + year + ",month:" + month + ",day:" + day);
            yearDatabase.ExecuteSQL("UPDATE " + DAY_TABLE + " SET value=" + value + " WHERE type=" + type + " AND month=" + month + " AND day=" + day);
        }

        internal void UpdateHour(ushort type, byte hour, uint value)
        {
            Logger.v(TAG, "UpdateHour,type:" + type + ",hour:" + hour + ",value:" + value);
            yearDatabase.ExecuteSQL("UPDATE " + HOUR_TABLE + " SET value=" + value + " WHERE year=" + TimeManager.TimeUsing.Year +
                " AND month=" + TimeManager.TimeUsing.Month + " AND day=" + TimeManager.TimeUsing.Day + " AND hour=" + hour + " AND type=" + type);
        }

        private void timerCallback(object state)
        {
            ////Should only be call from TimeManager sub-thread.

            ////检查是否需要切换表、库。
            ////这个函数与执行数据落地的函数是串行的，并且它将在数据落地之后执行，因此这里可以安全地切换数据库。
            //DateTime now = DateTime.Now;
            //if (now.Year != TimeManager.TimeUsing.Year)
            //{

            //}
            //else if (now.Month != TimeManager.TimeUsing.Month)
            //{

            //}
            //else if (now.Day != TimeManager.TimeUsing.Day)
            //{
            //    sumDay();
            //    switchTable();
            //}
            //else
            //{
            //    //Do nothing.
            //}
        }

        ///// <summary>
        ///// 统计day*_detail数据表。
        ///// </summary>
        //private void sumDay()
        //{
        //    Logger.v(TAG, "sum the day");
        //    //string summaryToday = "day" + TimeManager.TimeUsing.Day.ToString() + "_summary";
        //    //if (sqliteHelper.isTableExist(summaryToday))
        //    //{
        //    //    sqliteHelper.deleteTable(summaryToday); //Assume it will always success.
        //    //}

        //    //sqliteHelper.createDaySummaryTable(summaryToday); //Assume it will always success.

        //    //TODO 
        //}

        //private void switchTable()
        //{
            
        //}

        ///// <summary>
        ///// 查找所有数据库文件
        ///// </summary>
        ///// <returns>包含各数据库文件路径的列表</returns>
        //internal List<string> IterateDbs()
        //{
        //    if (Directory.Exists(DATABASE_DIR))
        //    {
        //        string[] files = Directory.GetFiles(DATABASE_DIR);
        //        string[] dirs = Directory.GetDirectories(DATABASE_DIR);
        //        string[] dirs2;
        //        List<string> validDb = new List<string>();
        //        string str;
        //        Regex regex = new Regex("[1][9][7-9][0-9]|[2][0][0-9][0-9]");
        //        foreach (string dir in dirs)
        //        {
        //            str = Toolset.GetBasename(dir);
        //            if (str is null || str.Length == 0)
        //                continue;

        //            //Hummmmmm,2021-01-12 19:08
        //            if (str.Length != 4)
        //                continue;

        //            if (!regex.IsMatch(str))
        //            {
        //                continue;
        //            }

        //            //directory valid
        //            dirs2 = Directory.GetFiles(dir);
        //            string str2;
        //            foreach(string file in dirs2)
        //            {
        //                str2 = Toolset.GetBasename(file);
        //                if (str2.StartsWith("kms" + str))
        //                {
        //                    if (str2.EndsWith(".db"))
        //                    {
        //                        validDb.Add(file);
        //                    }
        //                }
        //            }
        //        }

        //        if (validDb.Count > 0)
        //            return validDb;
        //        else
        //            return null;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        internal SQLiteDataReader QueryGlobalStatistic()
        {
            return QueryWholeTable(totalDatabase, GLOBAL_TABLE);
        }

        internal SQLiteDataReader QueryYearStatistic()
        {
            return QueryWholeTable(yearDatabase, YEAR_TABLE);
        }

        internal SQLiteDataReader QueryMonthStatistic()
        {
            return QueryWithSQL(yearDatabase, MONTH_TABLE, "SELECT*FROM" + MONTH_TABLE +
                " WHERE year=" + TimeManager.TimeUsing.Year + " AND month=" + TimeManager.TimeUsing.Month);
        }

        internal SQLiteDataReader QueryDayStatistic()
        {
            return QueryWithSQL(yearDatabase, DAY_TABLE, "SELECT*FROM " + DAY_TABLE +
                " WHERE year=" + TimeManager.TimeUsing.Year + " AND month=" + TimeManager.TimeUsing.Month + " AND day=" + TimeManager.TimeUsing.Day);
        }

        internal SQLiteDataReader QueryHourStatistic()
        {
            return QueryWithSQL(yearDatabase, HOUR_TABLE, "SELECT type,value,hour FROM " + HOUR_TABLE +
                " where year=" + TimeManager.TimeUsing.Year + " AND month=" + TimeManager.TimeUsing.Month + " AND day=" + TimeManager.TimeUsing.Day);
        }

        private SQLiteDataReader QueryWholeTable(SQLiteHelper sqlite, string table)
        {
            if (sqlite.IsDbReady())
            {
                if (sqlite.IsTableExist(table))
                {
                    return sqlite.Query("SELECT*FROM " + table);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private SQLiteDataReader QueryWithSQL(SQLiteHelper sqlite, string table, string sql)
        {
            if (sqlite.IsDbReady())
            {
                if (sqlite.IsTableExist(table))
                {
                    return sqlite.Query(sql);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
