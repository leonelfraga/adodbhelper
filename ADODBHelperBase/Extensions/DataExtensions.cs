using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ADODBHelperBase.Extensions
{
    public static class DataExtensions
    {
        public static T getValue<T>(this IDataRecord reader, string columnName)
        {
            if (reader[columnName] == DBNull.Value)
            {
                return default(T);
            }
            else
            {
                Type type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
                return (T)Convert.ChangeType(reader[columnName], type);
            }
        }

        public static T getValue<T>(this DataRow row, string columnName)
        {
            if (row[columnName] == DBNull.Value)
            {
                return default(T);
            }
            else
            {
                Type type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
                return (T)Convert.ChangeType(row[columnName], type);
            }
        }

        public static bool IsNull(this IDataReader reader, string columnName)
        {
            return (reader[columnName] == DBNull.Value);
        }
    }
}
