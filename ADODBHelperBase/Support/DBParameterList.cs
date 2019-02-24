using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;

namespace ADODBHelperBase
{
    public class DBParameterList<T> where T : DbParameter, new()
    {
        private List<T> _innerList = new List<T>();

        public void AddParam(string name, ParameterDirection direction, object value, object nullValue)
        {
            T newParam = new T();
            newParam.Value = ((value == null) || (value == nullValue)) ? DBNull.Value : value;
            newParam.Direction = direction;
            newParam.ParameterName = name;

            _innerList.Add(newParam);
        }

        public void AddInputParam(string name, object value, object nullValue)
        {
            AddParam(name, ParameterDirection.Input, value, nullValue);
        }

        public void AddInputParam(string name, object value)
        {
            AddParam(name, ParameterDirection.Input, value, null);
        }

        public void AddOutputParam(string name, object value, object nullValue)
        {
            AddParam(name, ParameterDirection.Output, value, nullValue);
        }

        public void AddOutputParam(string name, object value)
        {
            AddParam(name, ParameterDirection.Output, value, null);
        }

        public List<T> Items
        {
            get
            {
                return _innerList;
            }
        }
    }
}
