
namespace KMS.src.db
{
    interface IDatabase
    {
        bool openDatabase(string path);
        void ExecuteSQL(string sql);
        bool deleteTable(string name);
        void closeDababase();
    }
}
