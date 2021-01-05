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
                prepareDir(curDbFile);
            }
            catch (Exception e)
            {
                MessageBox.Show("Database initialization failed");
                Application.Current.Shutdown();
            }

            //open db.
            sqliteHelper = new SQLiteHelper();
            if (!sqliteHelper.openDatabase(curDbFile))
            {
                MessageBox.Show("Database open failed");
                Application.Current.Shutdown();
            }

            string today = "day" + todayInMonth;
            Logger.v(TAG, "today:" + today);
            if (!sqliteHelper.isTableExist(today))
            {
                sqliteHelper.createTable(today);
            }
        }

        private void prepareDir(string fp)
        {
            Logger.v(TAG, "cur db file:" + fp);
            if (File.Exists(fp))
            {
                Logger.v(TAG, "456");
            }
            else
            {
                Logger.v(TAG, "123");
                string parentPath = fp.Substring(0, fp.LastIndexOf('/')); //Exception? I don't care...
                if (!Directory.Exists(parentPath))
                {
                    Directory.CreateDirectory(parentPath); //Exception? I don't wanna care...
                }
            }
        }


    }
}
