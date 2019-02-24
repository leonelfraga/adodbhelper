using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADODBHelperBase.Extensions;
using FirebirdADODbHelper;
using System.Configuration;
using System.Data;

namespace FBAdoDbHelperTestApp
{
    class Program
    {
        

        static void Main(string[] args)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["cnx"].ConnectionString;

            //TestInserts(connectionString);
            TestSelect(connectionString);

            Console.WriteLine("");
            Console.WriteLine("Type any key to continue...");
            Console.ReadKey();
        }

        public static void TestInserts(string cnx)
        {
            using (FBAdoDBHelper dbHelper = new FBAdoDBHelper(cnx))
            {
                string sql = "insert into SAMPLE_TABLE (FIELD_1,FIELD_2,FIELD_3) values (@FIELD_1,@FIELD_2,@FIELD_3)";
                FbParameterList sqlParams = new FbParameterList();
                try
                {
                    dbHelper.BeginTransaction();

                    for (int i = 1; i <= 1000; i++)
                    {
                        try
                        {
                            sqlParams.Items.Clear();

                            sqlParams.AddInputParam("@FIELD_1", i);
                            sqlParams.AddInputParam("@FIELD_2", String.Format("SAMPLE DATA #{0}", i));
                            sqlParams.AddInputParam("@FIELD_3", DateTime.Now);

                            dbHelper.ExecuteNonQuery(sql, sqlParams.Items.ToArray());
                            Console.WriteLine(String.Format("Success: {0}", i));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(String.Format("Error: {0}", i));
                            throw ex;
                        }
                    }

                    dbHelper.CommitTransaction();
                }
                catch(Exception exDB)
                {
                    Console.WriteLine("Error: " + exDB.Message);
                    dbHelper.RollbackTransaction();
                }
            }
        }

        public static void TestSelect(string cnx)
        {
            string sql = "select FIELD_1,FIELD_2,FIELD_3 from SAMPLE_TABLE";

            try
            {
                using (FBAdoDBHelper dbHelper = new FBAdoDBHelper(cnx))
                {
                    DataTable table = dbHelper.GetTable(sql);

                    foreach(DataRow r in table.Rows)
                    {
                        Console.WriteLine(String.Format("1: {0} / 2: {1} / 3: {2:dd/MM/yyyy HH:mm}",
                            r.getValue<int>("FIELD_1"),
                            r.getValue<string>("FIELD_2"),
                            r.getValue<DateTime>("FIELD_3")));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
