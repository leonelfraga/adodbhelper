using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace ADODBHelperBase
{
    public abstract class ADODbHelperBase<CnxType, CmdType, AdpType, ParamType> : IDisposable, IADODBHelper
        where CnxType : DbConnection, new()
        where CmdType : DbCommand, new()
        where AdpType : DbDataAdapter, new()
        where ParamType : DbParameter, new()
    {
        #region Private (fields)
        private CnxType _connection;
        private DbTransaction _transaction;
        DBParameterList<ParamType> _outputParams;
        #endregion

        #region Constructors
        public ADODbHelperBase(string connectionString)
        {
            _connection = new CnxType();
            _connection.ConnectionString = connectionString;
            _connection.Open();
        }
        #endregion

        #region Private Methods
        private CmdType createCommand(string sql, CommandType cmdType, DbParameter[] sqlParams = null)
        {
            _outputParams = new DBParameterList<ParamType>();

            CmdType _sqlCommand = new CmdType();
            _sqlCommand.Connection = _connection;
            _sqlCommand.CommandText = sql;
            _sqlCommand.CommandType = cmdType;

            if(sqlParams != null)
            {
                for(int i = 0; i < sqlParams.Length; i++)
                {
                    _sqlCommand.Parameters.Add(sqlParams[i]);

                    if(sqlParams[i].Direction == ParameterDirection.Output)
                    {
                        _outputParams.AddOutputParam(sqlParams[i].ParameterName, sqlParams[i].Value);
                    }
                }
            }

            if(_transaction != null)
            {
                _sqlCommand.Transaction = _transaction;
            }

            return _sqlCommand;
        }
        #endregion

        #region Transaction Control
        public void BeginTransaction()
        {
            _transaction = _connection.BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Commit();
                _transaction.Dispose();
            }
        }

        public void RollbackTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction.Dispose();
            }
        }
        #endregion

        #region Output Parameters
        public T GetOutputParamValue<T>(string parameterName)
        {
            var ret = _outputParams.Items.FirstOrDefault(p => p.ParameterName == parameterName);

            if(ret == null)
            {
                return default(T);
            }
            else
            {
                if((ret.Value == null) || (ret.Value == DBNull.Value))
                {
                    return default(T);
                }
                else
                {
                    return (T)Convert.ChangeType(ret.Value, typeof(T));
                }
            }
        }
        #endregion

        #region SQL Execution
        public int ExecuteNonQuery(string sql, CommandType commandType, params DbParameter[] parameters)
        {
            using (CmdType _cmd = createCommand(sql, commandType, parameters))
            {
                return _cmd.ExecuteNonQuery();
            }
        }

        public int ExecuteNonQuery(string sql, params DbParameter[] parameters)
        {
            using (CmdType _cmd = createCommand(sql, CommandType.Text, parameters))
            {
                return _cmd.ExecuteNonQuery();
            }
        }

        public DataTable GetTable(string sql, CommandType commandType, params DbParameter[] parameters)
        {
            using (CmdType _cmd = createCommand(sql, commandType, parameters))
            {
                using (AdpType adapter = new AdpType())
                {
                    adapter.SelectCommand = _cmd;
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);

                    return ds.Tables[0];
                }
            }
        }

        public DataTable GetTable(string sql, params DbParameter[] parameters)
        {
            using (CmdType _cmd = createCommand(sql, CommandType.Text, parameters))
            {
                using (AdpType adapter = new AdpType())
                {
                    adapter.SelectCommand = _cmd;
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);

                    return ds.Tables[0];
                }
            }
        }

        public T ExecuteScalar<T>(string sql, CommandType commandType, params DbParameter[] parameters)
        {
            using (CmdType _cmd = createCommand(sql, commandType, parameters))
            {
                object ret = _cmd.ExecuteScalar();

                if(ret == null)
                {
                    return default(T);
                }
                else
                {
                    return (T)Convert.ChangeType(ret, typeof(T));
                }
            }
        }

        public T ExecuteScalar<T>(string sql, params DbParameter[] parameters)
        {
            using (CmdType _cmd = createCommand(sql, CommandType.Text, parameters))
            {
                object ret = _cmd.ExecuteScalar();

                if (ret == null)
                {
                    return default(T);
                }
                else
                {
                    return (T)Convert.ChangeType(ret, typeof(T));
                }
            }
        }

        public IDataReader ExecuteReader(string sql, CommandType commandType, params DbParameter[] parameters)
        {
            using (CmdType _cmd = createCommand(sql, commandType, parameters))
            {
                return _cmd.ExecuteReader();
            }
        }

        public IDataReader ExecuteReader(string sql, params DbParameter[] parameters)
        {
            using (CmdType _cmd = createCommand(sql, CommandType.Text, parameters))
            {
                return _cmd.ExecuteReader();
            }
        }
        #endregion

        #region Dispose
        public void Dispose()
        {
            if (_connection != null)
            {
                if (_connection.State == ConnectionState.Open)
                {
                    _connection.Close();
                }
                _connection.Dispose();
            }
        }
        #endregion
    }
}
