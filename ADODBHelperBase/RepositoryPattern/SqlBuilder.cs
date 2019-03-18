using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace ADODBHelperBase.RepositoryPattern
{
    /// <summary>
    /// Generate SQL instructions from DataAnnotations
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SqlBuilder<T> where T : class
    {
        private string GetTableName()
        {
            var attributes = Attribute.GetCustomAttributes(typeof(T));

            foreach(var attrib in attributes)
            {
                if(attrib is TableAttribute)
                {
                    TableAttribute table = (TableAttribute)attrib;
                    return table.Name;
                }
            }

            return String.Empty;
        }

        private string[] GetFields()
        {
            List<string> ret = new List<string>();

            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach(var prop in properties)
            {
                var attributes = Attribute.GetCustomAttributes(prop);
                foreach(var attrib in attributes)
                {
                    if(attrib is ColumnAttribute)
                    {
                        ColumnAttribute cln = (ColumnAttribute)attrib;
                        ret.Add(cln.Name);
                    }
                }
            }

            return ret.ToArray();
        }

        private string[] GetKeyFields()
        {
            List<string> ret = new List<string>();

            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                var attributes = Attribute.GetCustomAttributes(prop);
                bool hasKeyAttr = false;

                foreach (var attrib in attributes)
                {
                    if (attrib is KeyAttribute)
                    {
                        hasKeyAttr = true;
                        break;
                    }
                }

                if(hasKeyAttr)
                {
                    foreach (var attrib in attributes)
                    {
                        if (attrib is ColumnAttribute)
                        {
                            ColumnAttribute cln = (ColumnAttribute)attrib;
                            ret.Add(cln.Name);
                        }
                    }
                }
            }

            return ret.ToArray();
        }

        public string SelectSqlByKey()
        {
            StringBuilder sqlResult = new StringBuilder("select ");

            string fromTable = GetTableName();
            string[] fields = GetFields();
            string[] keys = GetKeyFields();

            for(int i = 0; i < fields.Length;i++)
            {
                sqlResult.AppendLine(String.Format("    {0}{1}", fields[i], (i == (fields.Length - 1) ? "" : ",")));
            }

            sqlResult.AppendLine("from ");
            sqlResult.AppendLine(String.Format("    {0} ", fromTable));
            sqlResult.AppendLine("where ");

            for (int i = 0; i < keys.Length; i++)
            {
                sqlResult.AppendLine(String.Format("    {0}{1} = @{1} ", (i == 0) ? "" : "and ", keys[i]));
            }

            return sqlResult.ToString();
        }

        public string SelectSql()
        {
            StringBuilder sqlResult = new StringBuilder("select ");

            string fromTable = GetTableName();
            string[] fields = GetFields();
            string[] keys = GetKeyFields();

            for (int i = 0; i < fields.Length; i++)
            {
                sqlResult.AppendLine(String.Format("    {0}{1}", fields[i], (i == (fields.Length - 1) ? "" : ",")));
            }

            sqlResult.AppendLine("from ");
            sqlResult.AppendLine(String.Format("    {0} ", fromTable));

            return sqlResult.ToString();
        }

        public string DeleteSql()
        {
            string fromTable = GetTableName();
            string[] keys = GetKeyFields();

            StringBuilder sqlResult = new StringBuilder(String.Format("delete from {0} where ", fromTable));

            for (int i = 0; i < keys.Length; i++)
            {
                sqlResult.AppendLine(String.Format("    {0}{1} = @{1} ", (i == 0) ? "" : "and ", keys[i]));
            }

            return sqlResult.ToString();
        }

        public string InsertSql()
        {
            string fromTable = GetTableName();
            string[] fields = GetFields();

            StringBuilder sqlResult = new StringBuilder(String.Format("insert into {0} ", fromTable));
            StringBuilder insertFields = new StringBuilder();
            StringBuilder insertValues = new StringBuilder();

            for(int i = 0; i < fields.Length; i++)
            {
                insertFields.AppendLine(String.Format("    {0}{1}", fields[i], (i == (fields.Length - 1) ? "" : ",")));
                insertValues.AppendLine(String.Format("    @{0}{1}", fields[i], (i == (fields.Length - 1) ? "" : ",")));
            }

            sqlResult.Append("(");
            sqlResult.AppendLine(insertFields.ToString());
            sqlResult.Append(")");
            sqlResult.AppendLine(" values ");
            sqlResult.Append("(");
            sqlResult.Append(insertValues.ToString());
            sqlResult.Append(")");

            return sqlResult.ToString();
        }

        public string UpdateSql()
        {
            string fromTable = GetTableName();
            string[] fields = GetFields();
            string[] keys = GetKeyFields();

            StringBuilder sqlResult = new StringBuilder(String.Format("update {0} set ", fromTable));

            for (int i = 0; i < fields.Length; i++)
            {
                if (!keys.Contains(fields[i]))
                {
                    sqlResult.AppendLine(String.Format("    {0} = @{0}{1}", fields[i], (i == (fields.Length - 1) ? "" : ",")));
                }
            }

            sqlResult.AppendLine("where ");

            for (int i = 0; i < keys.Length; i++)
            {
                sqlResult.AppendLine(String.Format("    {0}{1} = @{1} ", (i == 0) ? "" : "and ", keys[i]));
            }

            return sqlResult.ToString();
        }
    }
}
