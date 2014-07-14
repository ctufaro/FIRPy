using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FIRPy.DataAccess
{
    public abstract class BaseDataAccess
    {
        public abstract DataTable GetDataTable(string sql);
        public abstract int ExecuteNonQuery(string sql);
        public abstract string ExecuteScalar(string sql);
        public abstract bool Update(String tableName, Dictionary<String, String> data, String where);
        public abstract bool Delete(String tableName, String where);
        public abstract bool Insert(String tableName, Dictionary<String, String> data);
        public abstract bool ClearDB();
        public abstract bool ClearTable(String table);
    }
}
