using KMS.src.tool;
using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace KMS.src.db
{
    class SQLiteHelper : IDatabase
    {
        private const string TAG = "SQLiteHelper";

        private SQLiteConnection sqliteConnection;
        private SQLiteTransaction sqliteTransaction;
        private SQLiteCommand detailCmd;

        public bool openDatabase(string path)
        {
            try
            {
                sqliteConnection = new SQLiteConnection("data source=" + path);
                sqliteConnection.Open();

                //SQLite在打开文件时不会检测其是否合法，此处通过执行SQL语句来检测库文件合法性。
                //当库文件不合法时，将其重命名为KMS不识别的格式以避免后续继续打开此文件而浪费计算资源。
                //chorm, 2021-01-12 22:55
                try
                {
                    SQLiteCommand cmd = new SQLiteCommand(sqliteConnection);
                    cmd.CommandText = "SELECT name from sqlite_master";
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    reader.Close();
                }
                catch (SQLiteException)
                {
                    sqliteConnection.Close();
                    File.Move(path, path + "_invalid");
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        internal bool createDetailTable(string name)
        {
            if (name == null || name.Length == 0 || sqliteConnection == null || sqliteConnection.State != ConnectionState.Open)
            {
                return false;
            }

            SQLiteCommand cmd = new SQLiteCommand(sqliteConnection);
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS " + name + "(year SMALLINT NOT NULL, month TINYINT NOT NULL, day TINYINT NOT NULL" +
                ",hour TINYINT NOT NULL, minute TINYINT NOT NULL, second TINYINT NOT NULL" +
                ",type SMALLINT NOT NULL,fkey TINYINT DEFAULT 0,value MEDIUMINT DEFAULT 0)";
            cmd.ExecuteNonQuery();

            return true;
        }

        internal bool isTableExist(string name)
        {
            if (name == null || name.Length == 0 || sqliteConnection == null || sqliteConnection.State != ConnectionState.Open)
            {
                return false;
            }

            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = sqliteConnection;

            cmd.CommandText = "SELECT name FROM sqlite_master where type='table' AND name='" + name + "'";
            SQLiteDataReader reader = cmd.ExecuteReader();
            bool hasRow = reader.HasRows;
            reader.Close();
            
            return hasRow;
        }

        public void closeDababase()
        {
            if (sqliteConnection != null)
                sqliteConnection.Close();
        }

        internal bool BeginTransaction()
        {
            sqliteTransaction = sqliteConnection.BeginTransaction();
            return sqliteTransaction != null;
        }

        internal bool CommitTransaction()
        {
            if (sqliteTransaction is null)
                return false;

            sqliteTransaction.Commit();
            return true;
        }

        internal void InsertDetail(string sql)
        {
            if(detailCmd is null)
                detailCmd = new SQLiteCommand();

            detailCmd.Connection = sqliteConnection;
            detailCmd.CommandText = sql;
            detailCmd.ExecuteNonQuery();
        }

        public bool deleteTable(string name)
        {
            SQLiteCommand cmd = new SQLiteCommand(sqliteConnection);
            cmd.CommandText = "DROP TABLE IF EXISTS " + name;
            cmd.ExecuteNonQuery();

            return true;
        }

        internal bool createDaySummaryTable(string name)
        {
            SQLiteCommand cmd = new SQLiteCommand(sqliteConnection);
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS " + name
                + "(_id INTEGER PRIMARY KEY AUTOINCREMENT,hour TINYINT NOT NULL,type SMALLINT NOT NULL,value MEDIUMINT DEFAULT 0)";
            cmd.ExecuteNonQuery();
            
            return true;
        }

        internal SQLiteDataReader QueryTable(string name)
        {
            SQLiteCommand cmd = new SQLiteCommand(sqliteConnection);
            cmd.CommandText = "SELECT * FROM " + name;
            return cmd.ExecuteReader();
        }
    }
}
