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
                    sqliteHelper.createTable(curTable);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Database exception");
                Application.Current.Shutdown();
                return;
            }

            refreshStatistic();
        }

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

        internal void EventHappen(int typeCode, DateTime time)
        {

        }
    }
}
