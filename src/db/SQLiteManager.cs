using KMS.src.tool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KMS.src.db
{
    class SQLiteManager : IDatabase
    {
        private const string TAG = "SQLiteManager";

        private const string MAIN_DIR_NAME = "db";

        private static SQLiteManager instance;

        private SQLiteManager()
        {
            prepareDir();
            DateTime dateTime = DateTime.Now;
            string today = dateTime.ToShortDateString();
            Logger.v(TAG, "today:" + today + "," + dateTime.ToShortTimeString());

        }

        internal static IDatabase getInstance()
        {
            //if(instance == null)
                instance = new SQLiteManager();

            return instance;
        }

        private void prepareDir()
        {
            if (Directory.Exists(MAIN_DIR_NAME))
            {
                Logger.v(TAG, "exis");

            }
            else
            {
                Logger.v(TAG, "creating");
                if (File.Exists(MAIN_DIR_NAME))
                {
                    DateTime dt = DateTime.Now;
                    File.Move(MAIN_DIR_NAME, MAIN_DIR_NAME + "_" + dt.Year + dt.Month + dt.Day + dt.Hour + dt.Minute + dt.Second);
                }
                Directory.CreateDirectory(MAIN_DIR_NAME);
            }
        }
    }
}
