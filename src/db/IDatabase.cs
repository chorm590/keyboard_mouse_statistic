
namespace KMS.src.db
{
    interface IDatabase
    {
        bool openDatabase(string path);
        bool createTable(string name);
        bool deleteTable(string name);
        void closeDababase();
    }
}
