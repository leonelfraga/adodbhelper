using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADODBHelperBase;
using FirebirdSql.Data.FirebirdClient;

namespace FirebirdADODbHelper
{
    public class FBAdoDBHelper : ADODbHelperBase<FbConnection,FbCommand,FbDataAdapter,FbParameter>
    {
        public FBAdoDBHelper(string connectionString) : base(connectionString) { }
    }
}
