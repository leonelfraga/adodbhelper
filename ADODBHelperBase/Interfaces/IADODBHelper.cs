using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;

namespace ADODBHelperBase
{
    public interface IADODBHelper : IDisposable
    {
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();

        T GetOutputParamValue<T>(string parameterName);

        int ExecuteNonQuery(string sql, CommandType commandType, params DbParameter[] parameters);
        int ExecuteNonQuery(string sql, params DbParameter[] parameters);
        DataTable GetTable(string sql, CommandType commandType, params DbParameter[] parameters);
        DataTable GetTable(string sql, params DbParameter[] parameters);
        T ExecuteScalar<T>(string sql, CommandType commandType, params DbParameter[] parameters);
        T ExecuteScalar<T>(string sql, params DbParameter[] parameters);
        IDataReader ExecuteReader(string sql, CommandType commandType, params DbParameter[] parameters);
        IDataReader ExecuteReader(string sql, params DbParameter[] parameters);
    }
}
