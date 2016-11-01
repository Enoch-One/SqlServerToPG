using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data;
using System.Data.Common;
using Npgsql;
using NpgsqlTypes;

namespace Cnool.Common
{
    /// <summary>
    /// 数据库操作基类(for PostgreSQL)
    /// </summary>
    public class PostgreHelper
    {
        public static string curerror = string.Empty;

        public static string CONNECTIONSTR = "Server=61.153.18.239;Port=5432;User Id=postgres;Password=Zx5p(wFwDXSxaxEHA7yX;Database=db_unit;";//定义数据库连接字段
        public string GetConnectString()
        {
            return CONNECTIONSTR;
        }

        /// <summary>
        /// 获取执行SQL后的结果，多用于单个字段的值
        /// </summary>

        public static string GetField(string connectionString, string sql)
        {

            //SqlConnection conn = new SqlConnection(CONNECTIONSTR);
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            conn.Open();
            //SqlCommand cmd = new SqlCommand(sql, conn);
            NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);
            string re = string.Empty;
            try { re = cmd.ExecuteScalar().ToString().Trim(); }
            catch { re = null; }
            cmd.Dispose();
            conn.Close();
            conn.Dispose();
            conn = null;
            return re;
        }


        // 执行一条无返回值的SQL语句
        public static bool ExeSql(string sql, string connectionString)
        {
            bool be = true;
            NpgsqlConnection conn = null;
            NpgsqlCommand cmd = null;
            try
            {
                conn = new NpgsqlConnection(connectionString);
                conn.Open();
                cmd = new NpgsqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                be = false;
                string curerror = "execute sql error:" + e.Message + " " + sql + " time=" + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString();
                //MyGlobal.debug = this.curerror;


            }
            finally
            {

                try { conn.Close(); }
                catch (Exception) { }
                try { conn.Dispose(); }
                catch (Exception) { }
                try { cmd.Dispose(); }
                catch (Exception) { }
                conn = null;
                cmd = null;
            }
            return be;
        }

            //获取执行SQL后的结果，多用于单个字段的值


            //public string GetField(SqlConnection conn,string sql)		
            //{
			
            //    //SqlConnection conn=new SqlConnection(CONNECTIONSTR);
            //    //conn.Open();
            //    SqlCommand cmd=new SqlCommand(sql,conn);			
            //    string re=string.Empty;
            //    try{re=cmd.ExecuteScalar().ToString().Trim();}
            //    catch(Exception e){
            //        MyGlobal.debug=e.Message+" sql="+sql;
            //        re=null;}
			
            //    try{cmd.Dispose();}
            //    catch(Exception){}
            //    //conn.Close();	
            //    //conn.Dispose();
            //    //conn=null;
            //    return re;
            //}


        /// <summary>
        /// 得到数据条数
        /// </summary>
        public static int GetCount(string connectionString, string tblName, string condition)
        {
            StringBuilder sql = new StringBuilder("select count(*) from " + tblName);
            if (!string.IsNullOrEmpty(condition))
                sql.Append(" where " + condition);

            object count = ExecuteScalar(connectionString, CommandType.Text, sql.ToString(), null);
            return int.Parse(count.ToString());
        }

        /// <summary>
        /// 执行查询，返回DataSet
        /// </summary>
        public static DataSet ExecuteQuery(string connectionString, CommandType cmdType, string cmdText,
            params DbParameter[] cmdParms)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                using (NpgsqlCommand cmd = new NpgsqlCommand())
                {
                    PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                    using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                        return ds;
                    }
                }
            }
        }

        /// <summary>
        /// 在事务中执行查询，返回DataSet
        /// </summary>
        public static DataSet ExecuteQuery(DbTransaction trans, CommandType cmdType, string cmdText,
            params DbParameter[] cmdParms)
        {
            NpgsqlCommand cmd = new NpgsqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, cmdParms);
            NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds, "ds");
            cmd.Parameters.Clear();
            return ds;
        }

        /// <summary>
        /// 执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText,
            params DbParameter[] cmdParms)
        {
            NpgsqlCommand cmd = new NpgsqlCommand();

            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 在事务中执行 Transact-SQL 语句并返回受影响的行数。
        /// </summary>
        public static int ExecuteNonQuery(DbTransaction trans, CommandType cmdType, string cmdText,
            params DbParameter[] cmdParms)
        {
            NpgsqlCommand cmd = new NpgsqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, cmdParms);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 执行查询，返回DataReader
        /// </summary>
        public static DbDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText,
            params DbParameter[] cmdParms)
        {
            NpgsqlCommand cmd = new NpgsqlCommand();
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);

            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, cmdParms);
                NpgsqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }

        /// <summary>
        /// 在事务中执行查询，返回DataReader
        /// </summary>
        public static DbDataReader ExecuteReader(DbTransaction trans, CommandType cmdType, string cmdText,
            params DbParameter[] cmdParms)
        {
            NpgsqlCommand cmd = new NpgsqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, cmdParms);
            NpgsqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            cmd.Parameters.Clear();
            return rdr;
        }

        
        /// <summary>
        /// 得到datarow
        /// </summary>
        public static DataRow FillDataRow(string sql)
        {

            DataRowCollection rows = FillDataRows(sql);
            if (rows == null) return null;
            if (rows.Count == 0) return null;

            return rows[0];
        }
        //得到datarow
        public static DataRowCollection FillDataRows(string sql)
        {
            DataTable dt = FillDataTable(sql);

            if (dt == null) return null;
            return dt.Rows;

        }

        //得到table
        public static DataTable FillDataTable(string sql)
        {
            DataSet ds = new DataSet();
            bool bret = FillDataset(sql, ds, "table");
            if (!bret) return null;

            return ds.Tables[0];
        }
        // 根据SQL查询并用记录集填充DataSet
        public static bool FillDataset(string sql, DataSet ds, string tablename)
        {
            bool bret = false;
            NpgsqlConnection conn = null;
            NpgsqlDataAdapter da = null;
            try
            {
                conn = new NpgsqlConnection(CONNECTIONSTR);
                da = new NpgsqlDataAdapter(sql, conn);
                int iret = da.Fill(ds, tablename);
                if (iret >= 0) bret = true;
            }
            catch (Exception e)
            {

                curerror = "filldataset error:" + e.Message + " sql=" + sql;
                //MyGlobal.debug = this.curerror;
            }
            finally
            {
                da.Dispose();
                conn.Dispose();
                da = null;
                conn = null;
            }
            return bret;
        }



        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
        /// </summary>
        public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText,
            params DbParameter[] cmdParms)
        {
            NpgsqlCommand cmd = new NpgsqlCommand();

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, cmdParms);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 在事务中执行查询，并返回查询所返回的结果集中第一行的第一列。忽略其他列或行。
        /// </summary>
        public static object ExecuteScalar(DbTransaction trans, CommandType cmdType, string cmdText,
            params DbParameter[] cmdParms)
        {
            NpgsqlCommand cmd = new NpgsqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, cmdParms);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 生成要执行的命令
        /// </summary>
        /// <remarks>参数的格式：冒号+参数名</remarks>
        private static void PrepareCommand(DbCommand cmd, DbConnection conn, DbTransaction trans, CommandType cmdType,
            string cmdText, DbParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText.Replace("@", ":").Replace("?", ":").Replace("[", "\"").Replace("]", "\"");

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (NpgsqlParameter parm in cmdParms)
                {
                    parm.ParameterName = parm.ParameterName.Replace("@", ":").Replace("?", ":");

                    cmd.Parameters.Add(parm);
                }
            }
        }

        #region 生成参数

        public static DbParameter MakeInParam(string ParamName, NpgsqlDbType DbType, int Size, object Value)
        {
            return MakeParam(ParamName, DbType, Size, ParameterDirection.Input, Value);
        }

        public static DbParameter MakeOutParam(string ParamName, NpgsqlDbType DbType, int Size)
        {
            return MakeParam(ParamName, DbType, Size, ParameterDirection.Output, null);
        }

        public static DbParameter MakeParam(string ParamName, NpgsqlDbType DbType, Int32 Size, ParameterDirection Direction, object Value)
        {
            NpgsqlParameter praram = null;
            if (Size > 0)
                praram = new NpgsqlParameter(ParamName, DbType, Size);
            else
                praram = new NpgsqlParameter(ParamName, DbType);
            praram.Value = Value;
            praram.Direction = Direction;
            if (!(Direction == ParameterDirection.Output && Value == null))
                praram.Value = Value;
            return praram;
        }


        #endregion 生成参数结束
    }
}