using KMS.src.tool;
using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace KMS.src.db
{
    class SQLiteHelper
    {
        private const string TAG = "SQLiteHelper";

        private SQLiteConnection sqliteConnection;
        private SQLiteTransaction sqliteTransaction;
        private SQLiteCommand detailCmd;
        private SQLiteCommand cmd;

        public bool openDatabase(string path)
        {
            if (path is null || path.Length == 0)
                return false;

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

                cmd = new SQLiteCommand(sqliteConnection);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        internal bool IsDbReady()
        {
            if (sqliteConnection is null)
                return false;

            return sqliteConnection.State is ConnectionState.Open;
        }

        internal bool IsTableExist(string name)
        {
            if (name is null || name.Length is 0 || sqliteConnection is null || sqliteConnection.State != ConnectionState.Open)
                return false;

            SQLiteCommand cmd = new SQLiteCommand(sqliteConnection);

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

        internal SQLiteDataReader Query(string sql)
        {
            SQLiteCommand cmd = new SQLiteCommand(sqliteConnection);
            cmd.CommandText = sql;
            return cmd.ExecuteReader();
        }

        internal void ExecuteSQL(string sql)
        {
            if (IsDbReady())
            {
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }

        internal SQLiteDataReader QueryWithSQL(string sql)
        {
            if (sql is null || sql.Length == 0)
                return null;

            SQLiteCommand cmd = new SQLiteCommand(sqliteConnection);
            cmd.CommandText = sql;
            return cmd.ExecuteReader();
        }
    }
}
