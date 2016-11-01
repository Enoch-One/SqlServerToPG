using System;
using System.Data;
using System.Configuration;
using System.Web;

using System.Data.SqlClient;
namespace Cnool.Common
{
    ///// <summary>
    ///// DataConnection 的摘要说明
    ///// </summary>
    //public class DataConnection
    //{
    //    public DataConnection()
    //    {
    //        //
    //        // TODO: 在此处添加构造函数逻辑
    //        //
    //    }
    //}
    /// <summary>
    /// DataConnection 的摘要说明。
    /// </summary>

    public class DataConnection
    {

        public string curerror = string.Empty;
        public DataConnection()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        //public static string CONNECTIONSTR = "data source=61.153.17.105;initial catalog=news2006;password=news@))^;persist security info=True;user id=cnoolnews2006;packet size=4096";	//定义数据库连接字段
        public static string CONNECTIONSTR = "data source=61.174.68.161;initial catalog=db_blog2007;password=4rfv5tgb6yhn7ujm;persist security info=True;user id=ur_blog2007;packet size=4096";	//定义数据库连接字段
        //"Data Source=61.174.68.161;Initial Catalog=db_blog2007;User ID=ur_blog2007;Password=4rfv5tgb6yhn7ujm"
        SqlConnection connRead;
        SqlCommand cmdRead;


        public string GetConnectString()
        {
            return CONNECTIONSTR;
        }

        // 执行一条无返回值的SQL语句
        public bool ExeSql(SqlConnection conn, string sql)
        {

            bool be = true;
            //SqlConnection conn=null;
            SqlCommand cmd = null;
            try
            {
                //conn=new SqlConnection(CONNECTIONSTR);
                //conn.Open();

                cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                be = false;
                this.curerror = "execute sql error:" + e.Message + " " + sql + " time=" + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString();
                //MyGlobal.debug = this.curerror;

            }
            finally
            {

                //try{conn.Close();}
                //catch(Exception){}
                //try{conn.Dispose();}
                //catch(Exception){}
                try { cmd.Dispose(); }
                catch (Exception) { }
                //conn=null;
                cmd = null;
            }
            return be;
        }



        // 执行一条无返回值的SQL语句
        public bool ExeSql(string sql)
        {
            bool be = true;
            SqlConnection conn = null;
            SqlCommand cmd = null;
            try
            {
                conn = new SqlConnection(CONNECTIONSTR);
                conn.Open();
                cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                be = false;
                this.curerror = "execute sql error:" + e.Message + " " + sql + " time=" + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString();
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

        //得到table
        public DataTable FillDataTable(string sql)
        {
            DataSet ds = new DataSet();
            bool bret = this.FillDataset(sql, ds, "table");
            if (!bret) return null;

            return ds.Tables[0];
        }

        //得到table,事务中
        public DataTable FillDataTable1(string sql)
        {
            SqlConnection conn = new SqlConnection(CONNECTIONSTR);
            conn.Open();
            SqlTransaction transaction = conn.BeginTransaction();
            SqlCommand cmd = conn.CreateCommand();
            cmd.Transaction = transaction;
            try
            {
                DataSet ds = new DataSet();
                bool bret = this.FillDataset(sql, ds, "table");
                transaction.Commit();

                if (!bret) return null;
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return null;
            }
            finally
            {
                conn.Close();
            }
        }




        //得到datarow
        public DataRowCollection FillDataRows(string sql)
        {
            DataTable dt = this.FillDataTable(sql);

            if (dt == null) return null;
            return dt.Rows;

        }




        //得到datarow
        public DataRow FillDataRow(string sql)
        {

            DataRowCollection rows = this.FillDataRows(sql);
            if (rows == null) return null;
            if (rows.Count == 0) return null;

            return rows[0];
        }



        // 根据SQL查询并用记录集填充DataSet
        public bool FillDataset(string sql, DataSet ds, string tablename)
        {
            bool bret = false;
            SqlConnection conn = null;
            SqlDataAdapter da = null;
            try
            {
                conn = new SqlConnection(CONNECTIONSTR);
                da = new SqlDataAdapter(sql, conn);
                int iret = da.Fill(ds, tablename);
                if (iret >= 0) bret = true;
            }
            catch (Exception e)
            {

                this.curerror = "filldataset error:" + e.Message + " sql=" + sql;
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


        // 根据SQL查询并用记录集填充DataSet
        public void FillDataset(string sql, DataSet ds, int startRecord, int maxRecords, string tablename)
        {
            SqlConnection conn = new SqlConnection(CONNECTIONSTR);
            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            da.Fill(ds, startRecord, maxRecords, tablename);
            da.Dispose();
            conn.Dispose();
            da = null;
            conn = null;
        }
        // 执行SQL语句并取得返回结果
        public SqlDataReader GetReader(string sql)
        {
            connRead = new SqlConnection(CONNECTIONSTR);
            connRead.Open();
            cmdRead = new SqlCommand(sql, connRead);
            SqlDataReader rd = cmdRead.ExecuteReader();
            return rd;
        }
        // 关闭DataReader，释放资源
        public void GetReader_close()
        {
            cmdRead.Dispose();
            connRead.Close();
            connRead.Dispose();
            connRead = null;
        }

        //获取执行SQL后的结果，多用于单个字段的值
        /*		public string GetField(SqlConnection conn,string sql)		
                {
			
                    //SqlConnection conn=new SqlConnection(CONNECTIONSTR);
                    //conn.Open();
                    SqlCommand cmd=new SqlCommand(sql,conn);			
                    string re=string.Empty;
                    try{re=cmd.ExecuteScalar().ToString().Trim();}
                    catch(Exception e){
                        MyGlobal.debug=e.Message+" sql="+sql;
                        re=null;}
			
                    try{cmd.Dispose();}
                    catch(Exception){}
                    //conn.Close();	
                    //conn.Dispose();
                    //conn=null;
                    return re;
                }
        */


        //获取执行SQL后的结果，多用于单个字段的值
        public string GetField(string sql)
        {

            SqlConnection conn = new SqlConnection(CONNECTIONSTR);
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            string re = string.Empty;
            try { re = cmd.ExecuteScalar().ToString().Trim(); }
            catch { re = null; }
            cmd.Dispose();
            conn.Close();
            conn.Dispose();
            conn = null;
            return re;
        }

        //是否有效的sql
        public static bool isvalidSql(string sql)
        {
            if (DataConnection.getSelectNum(sql) >= 2) return false;//太多的select
            if (sql.IndexOf(";") < 0) return true;//无;号
            string[] sqls = sql.Split(';');

            for (int i = 0; i < sqls.Length; i++)
            {
                int inum = getYinhaoNum(sqls[i]);
                if (i == 0 || i == (sqls.Length - 1)) inum++;//第一个为偶数个


                if (inum % 2 == 1) return false;//偶数个

                //if(
            }

            return true;
        }

        //得到引号的数量
        private static int getYinhaoNum(String str)
        {
            if (str == null) return 0;
            if (str.Length == 0) return 0;

            int inum = 0;
            int ipos = str.IndexOf("'", 0);

            while (ipos >= 0)
            {
                inum++;
                ipos = str.IndexOf("'", ipos + 1);
            }

            return inum;
        }

        //得到引号的数量
        private static int getSelectNum(String str)
        {
            if (str == null) return 0;
            if (str.Length == 0) return 0;

            string curstr = str.ToLower();

            //int inum=0;
            int ipos = curstr.IndexOf("select", 0);
            if (ipos < 0) return 0;//无

            ipos = str.IndexOf("'", ipos + 1);
            if (ipos >= 0) return 2;//多于两个


            return 1;//只有1个
        }


    }
}