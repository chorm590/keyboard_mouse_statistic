
namespace KMS.src.db
{
    interface IDatabase
    {
        bool openDatabase(string path);
        bool deleteTable(string name);
        void closeDababase();
    }
}
