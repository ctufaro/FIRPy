using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FIRPy.DomainObjects;

namespace FIRPy.DataAccess
{
    public static class DataAccessFactory
    {
        public static BaseDataAccess GetDatabase(string database, ConfigSettings settings)
        {
            
            switch(database)
            {
                case("SQLite"):
                    return new SQLite(settings.SQLiteDatabaseLocation);
                    break;
                default:
                    return null;
            }
        }

        public static SQLiteBulkInsert GetBulkDatabase(ConfigSettings settings, string database)
        {
            string dbConnectionString = string.Format("Data Source={0};Version=3;", settings.SQLiteDatabaseLocation);
            return new SQLiteBulkInsert(new System.Data.SQLite.SQLiteConnection(dbConnectionString), database);            
        }
    }
}
