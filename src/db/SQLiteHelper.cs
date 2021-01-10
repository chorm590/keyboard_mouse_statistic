using KMS.src.tool;
using System;
using System.Data;
using System.Data.SQLite;

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
            }
            catch (Exception e)
            {
                Logger.v(TAG, e.StackTrace);
                return false;
            }

            detailCmd = new SQLiteCommand();

            return true;
        }

        public bool createTable(string name)
        {
            if (name == null || name.Length == 0 || sqliteConnection == null || sqliteConnection.State != ConnectionState.Open)
            {
                return false;
            }

            SQLiteCommand cmd = new SQLiteCommand(sqliteConnection);
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS " + name + "(_id INTEGER PRIMARY KEY autoincrement, year SMALLINT NOT NULL, month TINYINT NOT NULL, day TINYINT NOT NULL" +
                ",hour TINYINT NOT NULL, minute TINYINT NOT NULL, second TINYINT NOT NULL, type SMALLINT NOT NULL, value MEDIUMINT NOT NULL)";
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
            bool hasRows = reader.HasRows;
            reader.Close();

            return hasRows;
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
            detailCmd.Connection = sqliteConnection;
            detailCmd.CommandText = sql;
            detailCmd.ExecuteNonQuery();
        }
    }
}
