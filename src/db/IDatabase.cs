using System;
using System.Collections.Generic;
using System.Text;

namespace KMS.src.db
{
    interface IDatabase
    {
        bool openDatabase(string path);
        bool createTable(string name);
    }
}
