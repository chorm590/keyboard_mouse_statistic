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

        private readonly SQLiteHelper totalDatabase; //存储全局统计数据
        private SQLiteHelper yearDatabase; //存储本年度统计数据

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

            if (!totalDatabase.OpenDatabase(GlobalDb))
                return false;

            if (!yearDatabase.OpenDatabase(YearDb))
                return false;

            return true;
        }

        /// <summary>
        /// 检查全局库、年度库中各对应表是否存在，若不存在，则创建表并为各统计字段插入初始值。
        /// 2021-01-14 12:07
        /// </summary>
        internal void InitTables()
        {
            if (!totalDatabase.IsTableExist(GLOBAL_TABLE))
            {
                Logger.v(TAG, "creating global table");
                totalDatabase.ExecuteNonQuery("CREATE TABLE " + GLOBAL_TABLE + "(type SMALLINT PRIMARY KEY NOT NULL,value INTEGER)");
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
                yearDatabase.ExecuteNonQuery("CREATE TABLE " + YEAR_TABLE + "(type SMALLINT PRIMARY KEY NOT NULL,value INTEGER,year SMALLINT NOT NULL)");
                InsertYearTable();
            }

            if (!yearDatabase.IsTableExist(MONTH_TABLE))
            {
                Logger.v(TAG, "creating month table");
                yearDatabase.ExecuteNonQuery("CREATE TABLE " + MONTH_TABLE +
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
                CreateDayTable();
                InitDayTable();
            }

            if (!yearDatabase.IsTableExist(HOUR_TABLE))
            {
                Logger.v(TAG, "creating hour table");
                yearDatabase.ExecuteNonQuery("CREATE TABLE " + HOUR_TABLE +
                    "(type SMALLINT NOT NULL,value INTEGER,year SMALLINT NOT NULL,month TINYINT NOT NULL,day TINYINT NOT NULL,hour TINYINT NOT NULL)");
                //Enough!
            }
        }

        internal SQLiteDataReader InitYearTable()
        {
            if (!yearDatabase.IsTableExist(YEAR_TABLE))
            {
                Logger.v(TAG, "creating year table");
                yearDatabase.ExecuteNonQuery("CREATE TABLE " + YEAR_TABLE + "(type SMALLINT PRIMARY KEY NOT NULL,value INTEGER,year SMALLINT NOT NULL)");
                InsertYearTable();
                return null;
            }
            else
            {
                Logger.v(TAG, "year table existed,querying");
                return yearDatabase.ExecuteQuery("SELECT*FROM " + YEAR_TABLE);
            }
        }

        internal SQLiteDataReader InitMonthTable()
        {
            return null;
        }

        

        /// <summary>
        /// 日期更换时由StatisticManager调用。
        /// </summary>
        internal SQLiteDataReader TryQueryDayWhileSwitchDate()
        {
            // 检查指定日期记录是否存在。
            SQLiteDataReader reader = yearDatabase.ExecuteQuery(QueryDayTableSQL);
            if (reader is null)
            {
                Logger.v(TAG, "query null");
                return null;
            }
            else
            {
                if (!reader.HasRows)
                {
                    Logger.v(TAG, "No record found, initing");
                    InitDayTable();
                    return null;
                }
            }

            return reader;
        }

        private void InsertYearTable()
        {
            if (yearDatabase.BeginTransaction())
            {
                InitYearItem(Constants.TypeNumber.KEYBOARD_TOTAL);
                InitYearItem(Constants.TypeNumber.KEYBOARD_COMBOL_TOTAL);
                InitYearItem(Constants.TypeNumber.MOUSE_TOTAL);
                InitYearItem(Constants.TypeNumber.MOUSE_LEFT_BTN);
                InitYearItem(Constants.TypeNumber.MOUSE_RIGHT_BTN);
                InitYearItem(Constants.TypeNumber.MOUSE_WHEEL_FORWARD);
                InitYearItem(Constants.TypeNumber.MOUSE_WHEEL_BACKWARD);
                InitYearItem(Constants.TypeNumber.MOUSE_WHEEL_CLICK);
                InitYearItem(Constants.TypeNumber.MOUSE_SIDE_FORWARD);
                InitYearItem(Constants.TypeNumber.MOUSE_SIDE_BACKWARD);

                Dictionary<byte, Key>.KeyCollection keys = Constants.Keyboard.Keys;
                foreach (byte keycode in keys)
                {
                    InitYearItem(keycode);
                }

                yearDatabase.CommitTransaction();
            }
            else
            {
                MessageBox.Show("Init database failed");
                throw new Exception();
            }
        }

        private void CreateDayTable()
        {
            yearDatabase.ExecuteNonQuery("CREATE TABLE " + DAY_TABLE + "(type SMALLINT NOT NULL,value INTEGER,year SMALLINT NOT NULL,month TINYINT NOT NULL,day TINYINT NOT NULL)");
        }

        private void InitDayTable()
        {
            if (yearDatabase.BeginTransaction())
            {
                InsertDayItem(Constants.TypeNumber.KEYBOARD_TOTAL);
                InsertDayItem(Constants.TypeNumber.KEYBOARD_COMBOL_TOTAL);
                InsertDayItem(Constants.TypeNumber.MOUSE_TOTAL);
                InsertDayItem(Constants.TypeNumber.MOUSE_LEFT_BTN);
                InsertDayItem(Constants.TypeNumber.MOUSE_RIGHT_BTN);
                InsertDayItem(Constants.TypeNumber.MOUSE_WHEEL_FORWARD);
                InsertDayItem(Constants.TypeNumber.MOUSE_WHEEL_BACKWARD);
                InsertDayItem(Constants.TypeNumber.MOUSE_WHEEL_CLICK);
                InsertDayItem(Constants.TypeNumber.MOUSE_SIDE_FORWARD);
                InsertDayItem(Constants.TypeNumber.MOUSE_SIDE_BACKWARD);

                Dictionary<byte, Key>.KeyCollection keys = Constants.Keyboard.Keys;
                foreach (byte keycode in keys)
                {
                    InsertDayItem(keycode);
                }

                yearDatabase.CommitTransaction();
            }
            else
            {
                MessageBox.Show("Init database failed");
                throw new Exception();
            }
        }

        private void InitYearItem(ushort type)
        {
            yearDatabase.ExecuteNonQuery("INSERT INTO " + YEAR_TABLE + " VALUES(" + type + ",0," + TimeManager.TimeUsing.Year + ")");
        }

        /// <summary>
        /// 将指定日期下的 type 字段值插入表中。
        /// </summary>
        private void InsertDayItem(ushort type)
        {
            yearDatabase.ExecuteNonQuery("INSERT INTO " + DAY_TABLE + " VALUES(" + type + ",0," + TimeManager.TimeUsing.Year + "," + TimeManager.TimeUsing.Month + "," + TimeManager.TimeUsing.Day + ")");
        }

        internal void InsertGlobal(ushort type, uint value)
        {
            totalDatabase.ExecuteNonQuery("INSERT INTO " + GLOBAL_TABLE + " VALUES(" + type + "," + value + ")");
        }

        private void InsertMonth(ushort type)
        {
            yearDatabase.ExecuteNonQuery("INSERT INTO " + MONTH_TABLE + " VALUES(" + type + ",0," + TimeManager.TimeUsing.Year + "," + TimeManager.TimeUsing.Month + ")");
        }

        internal void InsertHour(ushort type, uint value, byte hour)
        {
            Logger.v(TAG, "InsertHour,type:" + type + ",hour:" + hour + ",value:" + value);
            yearDatabase.ExecuteNonQuery("INSERT INTO " + HOUR_TABLE + " VALUES(" + type + "," + value + "," +
                TimeManager.TimeUsing.Year + "," + TimeManager.TimeUsing.Month + "," + TimeManager.TimeUsing.Day + "," + hour + ")");
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
            //Logger.v(TAG, "update global,type:" + type + ",value:" + value);
            totalDatabase.ExecuteNonQuery("UPDATE " + GLOBAL_TABLE + " SET value=" + value + " WHERE type=" + type);
        }

        internal void UpdateYear(ushort type, uint value, ushort year)
        {
            //Logger.v(TAG, "update year,type:" + type + ",value:" + value + ",year:" + year);
            yearDatabase.ExecuteNonQuery("UPDATE " + YEAR_TABLE + " SET value=" + value + " WHERE type=" + type);
        }

        internal void UpdateMonth(ushort type, uint value, ushort year, byte month)
        {
            //Logger.v(TAG, "update month,type:" + type + ",value:" + value + ",year:" + year + ",month:" + month);
            yearDatabase.ExecuteNonQuery("UPDATE " + MONTH_TABLE + " SET value=" + value + " WHERE type=" + type + " AND month=" + month);
        }

        internal void UpdateDay(ushort type, uint value)
        {
            yearDatabase.ExecuteNonQuery("UPDATE " + DAY_TABLE + " SET value=" + value
                + " WHERE type=" + type
                + " AND year=" + TimeManager.TimeUsing.Year
                + " AND month=" + TimeManager.TimeUsing.Month
                + " AND day=" + TimeManager.TimeUsing.Day);
        }

        internal void UpdateHour(ushort type, uint value, byte hour)
        {
            Logger.v(TAG, "UpdateHour,type:" + type + ",hour:" + hour + ",value:" + value);
            yearDatabase.ExecuteNonQuery("UPDATE " + HOUR_TABLE + " SET value=" + value
                + " WHERE year=" + TimeManager.TimeUsing.Year
                + " AND month=" + TimeManager.TimeUsing.Month
                + " AND day=" + TimeManager.TimeUsing.Day
                + " AND hour=" + hour
                + " AND type=" + type);
        }

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
            return Query(totalDatabase, GLOBAL_TABLE, QueryGlobalTableSQL);
        }

        internal SQLiteDataReader QueryYearStatistic()
        {
            return Query(totalDatabase, YEAR_TABLE, QueryYearTableSQL);
        }

        internal SQLiteDataReader QueryMonthStatistic()
        {
            return Query(yearDatabase, MONTH_TABLE, QueryMonthTableSQL);
        }

        internal SQLiteDataReader QueryDayStatistic()
        {
            return Query(yearDatabase, DAY_TABLE, QueryDayTableSQL);
        }

        internal SQLiteDataReader QueryHourStatistic()
        {
            return Query(yearDatabase, HOUR_TABLE, QueryHourTableSQL);
        }

        private SQLiteDataReader Query(SQLiteHelper sqlite, string table, string sql)
        {
            if (sqlite.IsDbReady())
            {
                if (sqlite.IsTableExist(table))
                {
                    return sqlite.ExecuteQuery(sql);
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

        internal void SwitchYearDB()
        {
            yearDatabase.CloseDababase();
            yearDatabase.OpenDatabase(YearDb);
        }

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

        private string QueryGlobalTableSQL
        {
            get
            {
                return "SELECT*FROM " + GLOBAL_TABLE;
            }
        }

        private string QueryYearTableSQL
        {
            get
            {
                return "SELECT*FROM " + YEAR_TABLE;
            }
        }

        private string QueryMonthTableSQL
        {
            get
            {
                return "SELECT*FROM " + MONTH_TABLE
                    + " WHERE year=" + TimeManager.TimeUsing.Year
                    + " AND month=" + TimeManager.TimeUsing.Month;
            }
        }

        private string QueryDayTableSQL
        {
            get
            {
                return "SELECT*FROM " + DAY_TABLE + " WHERE year=" + TimeManager.TimeUsing.Year
                    + " AND month=" + TimeManager.TimeUsing.Month
                    + " AND day=" + TimeManager.TimeUsing.Day;
            }
        }

        private string QueryHourTableSQL
        {
            get
            {
                return "SELECT type,value,hour FROM " + HOUR_TABLE
                    + " WHERE year=" + TimeManager.TimeUsing.Year
                    + " AND month=" + TimeManager.TimeUsing.Month
                    + " AND day=" + TimeManager.TimeUsing.Day;
            }
        }
    }
}
