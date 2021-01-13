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

        private const string DATABASE_DIR = "data";
        private const string SUMMARY_TABLE = "summary";
        private const string MONTH_SUMMARY_PREFIX = "m";
        private const string DAY_SUMMARY_PREFIX = "d";
        private const string HOUR_SUMMARY_PREFIX = "h";

        private static SQLiteManager instance;

        private SQLiteHelper totalDatabase; //存储全局统计数据
        private SQLiteHelper curDatabase; //存储本年度统计数据

        /// <summary>
        /// 全局统计数据数据库文件。
        /// </summary>
        private string TotalDb
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

        private string YearTable
        {
            get
            {
                return SUMMARY_TABLE;
            }
        }

        private string MonthTable
        {
            get
            {
                return MONTH_SUMMARY_PREFIX + TimeManager.TimeUsing.Month;
            }
        }

        private string DayTable
        {
            get
            {
                return DAY_SUMMARY_PREFIX + TimeManager.TimeUsing.Day;
            }
        }

        private string HourTable
        {
            get
            {
                return HOUR_SUMMARY_PREFIX + TimeManager.TimeUsing.Day; //yes, it is 'Day'!
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
            totalDatabase = new SQLiteHelper();
            curDatabase = new SQLiteHelper();
        }

        internal bool Init()
        {
            if (totalDatabase is null || curDatabase is null)
                return false;

            if (!totalDatabase.openDatabase(TotalDb))
                return false;

            if (!curDatabase.openDatabase(YearDb))
                return false;

            totalDatabase.ExecuteSQL("CREATE TABLE IF NOT EXISTS " + SUMMARY_TABLE + "(type SMALLINT NOT NULL,value INTEGER DEFAULT 0)");
            curDatabase.ExecuteSQL("CREATE TABLE IF NOT EXISTS " + YearTable + "(year SMALLINT NOT NULL,type SMALLINT NOT NULL,value INTEGER DEFAULT 0)");
            curDatabase.ExecuteSQL("CREATE TABLE IF NOT EXISTS " + MonthTable + "(year SMALLINT NOT NULL,month TINYINT NOT NULL,type SMALLINT NOT NULL,value INTEGER DEFAULT 0)");
            curDatabase.ExecuteSQL("CREATE TABLE IF NOT EXISTS " + DayTable + 
                "(year SMALLINT NOT NULL,month TINYINT NOT NULL,day TINYINT NOT NULL,type SMALLINT NOT NULL,value INTEGER DEFAULT 0)");
            curDatabase.ExecuteSQL("CREATE TABLE IF NOT EXISTS " + HourTable + 
                "(year SMALLINT NOT NULL,month TINYINT NOT NULL,day TINYINT NOT NULL,hour TINYINT NOT NULL,type SMALLINT NOT NULL,value INTEGER DEFAULT 0)");

            return true;
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

        /// <summary>
        /// 准备好存储数据库文件的路径。
        /// </summary>
        /// <param name="fp"></param>
        private void initDbFilePath(string fp)
        {
            Logger.v(TAG, "path:" + fp);
            if (fp == null || fp.Length == 0)
                throw new ArgumentNullException("file path cannot be null or empty");

            if (!File.Exists(fp))
            {
                string parentPath = Toolset.GetParentPath(fp);
                Logger.v(TAG, "parent path:" + parentPath);
                if (!Directory.Exists(parentPath))
                {
                    Directory.CreateDirectory(parentPath);
                }
            }
        }

        internal void close()
        {
            //if (sqliteHelper != null)
            //    sqliteHelper.closeDababase();
        }

        internal bool BeginTransaction()
        {
            //return sqliteHelper.BeginTransaction();
            return false;
        }

        internal void InsertDetail(string str)
        {
            //sqliteHelper.InsertDetail("INSERT INTO " + curTable + "(year,month,day,hour,minute,second,type,fkey,value) " + str);
        }

        internal void CommitTransaction()
        {
            //sqliteHelper.CommitTransaction();
        }

        private void timerCallback(object state)
        {
            //Should only be call from TimeManager sub-thread.

            //检查是否需要切换表、库。
            //这个函数与执行数据落地的函数是串行的，并且它将在数据落地之后执行，因此这里可以安全地切换数据库。
            DateTime now = DateTime.Now;
            if (now.Year != TimeManager.TimeUsing.Year)
            {

            }
            else if (now.Month != TimeManager.TimeUsing.Month)
            {

            }
            else if (now.Day != TimeManager.TimeUsing.Day)
            {
                sumDay();
                switchTable();
            }
            else
            {
                //Do nothing.
            }
        }

        /// <summary>
        /// 统计day*_detail数据表。
        /// </summary>
        private void sumDay()
        {
            Logger.v(TAG, "sum the day");
            //string summaryToday = "day" + TimeManager.TimeUsing.Day.ToString() + "_summary";
            //if (sqliteHelper.isTableExist(summaryToday))
            //{
            //    sqliteHelper.deleteTable(summaryToday); //Assume it will always success.
            //}

            //sqliteHelper.createDaySummaryTable(summaryToday); //Assume it will always success.

            //TODO 
        }

        private void switchTable()
        {
            
        }

        /// <summary>
        /// 查找所有数据库文件
        /// </summary>
        /// <returns>包含各数据库文件路径的列表</returns>
        internal List<string> IterateDbs()
        {
            if (Directory.Exists(DATABASE_DIR))
            {
                string[] files = Directory.GetFiles(DATABASE_DIR);
                string[] dirs = Directory.GetDirectories(DATABASE_DIR);
                string[] dirs2;
                List<string> validDb = new List<string>();
                string str;
                Regex regex = new Regex("[1][9][7-9][0-9]|[2][0][0-9][0-9]");
                foreach (string dir in dirs)
                {
                    str = Toolset.GetBasename(dir);
                    if (str is null || str.Length == 0)
                        continue;

                    //Hummmmmm,2021-01-12 19:08
                    if (str.Length != 4)
                        continue;

                    if (!regex.IsMatch(str))
                    {
                        continue;
                    }

                    //directory valid
                    dirs2 = Directory.GetFiles(dir);
                    string str2;
                    foreach(string file in dirs2)
                    {
                        str2 = Toolset.GetBasename(file);
                        if (str2.StartsWith("kms" + str))
                        {
                            if (str2.EndsWith(".db"))
                            {
                                validDb.Add(file);
                            }
                        }
                    }
                }

                if (validDb.Count > 0)
                    return validDb;
                else
                    return null;
            }
            else
            {
                return null;
            }
        }

        internal SQLiteDataReader QueryTotalStatistic()
        {
            if (Directory.Exists(DATABASE_DIR))
            {
                if (File.Exists(TotalDb))
                {
                    if (totalDatabase.IsDbReady())
                    {
                        if (totalDatabase.isTableExist(SUMMARY_TABLE))
                        {
                            return totalDatabase.QueryTable(SUMMARY_TABLE);
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
