using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace EHD_Miner
{
    class DBHelper
    {
        private readonly static string connStr = "Data Source=" + Application.StartupPath + "\\database.sqlite";

        //获取 connection 对象
        public static IDbConnection CreateConnection()
        {
            IDbConnection conn = new SqliteConnection(connStr);//MySqlConnection //SqlConnection
            conn.Open();
            return conn;
        }

        //执行非查询语句
        public static int ExecuteNonQuery(IDbConnection conn, string sql, Dictionary<string, object> parameters)
        {
            using (IDbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                foreach (KeyValuePair<string, object> keyValuePair in parameters)
                {
                    IDbDataParameter parameter = cmd.CreateParameter();
                    parameter.ParameterName = keyValuePair.Key;
                    parameter.Value = keyValuePair.Value;
                    cmd.Parameters.Add(parameter);
                }
                return cmd.ExecuteNonQuery();
            }
        }

        //执行非查询语句-独立连接
        public static int ExecuteNonQuery(string sql, Dictionary<string, object> parameters)
        {
            using (IDbConnection conn = CreateConnection())
            {
                return ExecuteNonQuery(conn, sql, parameters);
            }
        }

        //查询首行首列
        public static object ExecuteScalar(IDbConnection conn, string sql, Dictionary<string, object> parameters)
        {
            using (IDbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                foreach (KeyValuePair<string, object> keyValuePair in parameters)
                {
                    IDbDataParameter parameter = cmd.CreateParameter();
                    parameter.ParameterName = keyValuePair.Key;
                    parameter.Value = keyValuePair.Value;
                    cmd.Parameters.Add(parameter);
                }
                return cmd.ExecuteScalar();
            }
        }

        //查询首行首列-独立连接
        public static object ExecuteScalar(string sql, Dictionary<string, object> parameters)
        {
            using (IDbConnection conn = CreateConnection())
            {
                return ExecuteScalar(conn, sql, parameters);
            }
        }

        //查询表
        public static DataTable ExecuteQuery(IDbConnection conn, string sql, Dictionary<string, object> parameters)
        {
            DataTable dt = new DataTable();
            using (IDbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                foreach (KeyValuePair<string, object> keyValuePair in parameters)
                {
                    IDbDataParameter parameter = cmd.CreateParameter();
                    parameter.ParameterName = keyValuePair.Key;
                    parameter.Value = keyValuePair.Value;
                    cmd.Parameters.Add(parameter);
                }
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    dt.Load(reader);
                }
            }

            return dt;
        }

        //查询表--独立连接
        public static DataTable ExecuteQuery(string sql, Dictionary<string, object> parameters)
        {
            using (IDbConnection conn = CreateConnection())
            {
                return ExecuteQuery(conn, sql, parameters);
            }
        }
    }
}
