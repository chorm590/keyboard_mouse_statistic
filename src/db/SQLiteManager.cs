using KMS.src.tool;
using System;
using System.IO;
using System.Reflection.Metadata;
using System.Threading;
using System.Windows;

namespace KMS.src.db
{
    class SQLiteManager
    {
        private const string TAG = "SQLiteManager";

        private const string DATABASE_DIR = "db";

        private static SQLiteManager instance;

        private string curTable;
        private SQLiteHelper sqliteHelper;

        private string dbFilePath
        {
            get
            {
                return DATABASE_DIR + "/" + TimeManager.TimeUsing.Year + "/" + "kms" + TimeManager.TimeUsing.ToString("yyyyMM") + ".db";
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
            string curDbFile = dbFilePath;
            //make sure the directories.
            try
            {
                initDbFilePath(curDbFile);
            }
            catch (Exception e)
            {
                MessageBox.Show("Database initialization failed");
                Application.Current.Shutdown();
                return;
            }

            //open db.
            sqliteHelper = new SQLiteHelper();
            if (!sqliteHelper.openDatabase(curDbFile))
            {
                MessageBox.Show("Database open failed");
                Application.Current.Shutdown();
                return;
            }

            curTable = "day" + TimeManager.TimeUsing.Day.ToString() + "_detail";
            Logger.v(TAG, "current table:" + curTable);
            try
            {
                if (!sqliteHelper.isTableExist(curTable))
                {
                    sqliteHelper.createDetailTable(curTable);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Database exception");
                Application.Current.Shutdown();
                return;
            }

            refreshStatistic();
            //tool.Timer.RegisterTimerCallback(timerCallback);
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
            if (sqliteHelper != null)
                sqliteHelper.closeDababase();
        }

        /// <summary>
        /// 从各年度统计数据库、各月度统计数据库中累加数值。
        /// </summary>
        private void refreshStatistic()
        {
            if (Directory.Exists(DATABASE_DIR))
            {
                string[] dirs = Directory.GetDirectories(DATABASE_DIR);
                int idx;
                string year;
                foreach (string dir in dirs)
                {
                    idx = dir.LastIndexOf("\\");
                    if (idx < 0)
                        continue;

                    idx++;
                    year = dir.Substring(idx);

                    if (File.Exists(dir + "\\kms" + year + ".db"))
                    {
                        //TODO 从年度统计表中取出数据。
                    }
                    else
                    {
                        //TODO 查询各月度统计表。
                        refreshFromMonthDb(dir);
                    }
                }
            }
        }

        private void refreshFromMonthDb(string dir)
        {
            if (dir == null || dir.Length == 0)
                return;

            if (!Directory.Exists(dir))
                return;

            string[] dbs = Directory.GetFiles(dir);
            foreach (string db in dbs)
            {
                if (db.Contains("kms") && db.Contains(".db"))
                { 
                    //TODO 从各月份数据库中取出统计数据，当前日实时计算总数，非当前月查询统计表。
                }
            }
        }

        internal bool BeginTransaction()
        {
            return sqliteHelper.BeginTransaction();
        }

        internal void InsertDetail(string str)
        {
            sqliteHelper.InsertDetail("INSERT INTO " + curTable + "(year,month,day,hour,minute,second,type,fkey,value) " + str);
        }

        internal void CommitTransaction()
        {
            sqliteHelper.CommitTransaction();
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
            string summaryToday = "day" + TimeManager.TimeUsing.Day.ToString() + "_summary";
            if (sqliteHelper.isTableExist(summaryToday))
            {
                sqliteHelper.deleteTable(summaryToday); //Assume it will always success.
            }

            sqliteHelper.createDaySummaryTable(summaryToday); //Assume it will always success.

            //TODO 
        }

        private void switchTable()
        {
            
        }
    }
}
