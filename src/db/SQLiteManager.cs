using KMS.src.tool;
using System;
using System.IO;
using System.Windows;

namespace KMS.src.db
{
    class SQLiteManager
    {
        private const string TAG = "SQLiteManager";

        private static SQLiteManager instance;

        private SQLiteHelper sqliteHelper;
        private string curTable;

        private string dbFilePath
        {
            get
            {
                //One file per month. ok? I don't known...
                DateTime current = DateTime.Now;
                return "db/" + current.Year + "/" + "kms" + current.ToString("yyyyMM") + ".db";
            }
        }

        private string todayInMonth
        {
            get
            {
                return DateTime.Now.Day.ToString();
            }
        }


        internal static SQLiteManager getInstance()
        {
            if (instance == null)
                instance = new SQLiteManager();

            return instance;
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

            curTable = "day" + todayInMonth + "_detail";
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
    }
}
