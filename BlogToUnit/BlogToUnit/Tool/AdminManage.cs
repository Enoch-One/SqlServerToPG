using BlogToUnit.Entity;
using Cnool.Common;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogToUnit.Tool
{
    public class AdminManage
    {
        private static string PostGreSqlConnectionString = "Server=61.153.18.239;Port=5432;User Id=postgres;Password=Zx5p(wFwDXSxaxEHA7yX;Database=db_unit;";
        public static long GetId()
        {
            string sql = "SELECT nextval('public_sys_admin_id_seq')";
            long id = Convert.ToInt64(PostgreHelper.GetField(PostGreSqlConnectionString, sql));
            return id;
        }

        //查询是否已经存在用户
        public static DataRow getAdminByName(string loginName)
        {
            string sql = "select * from sys_admin where username='" + loginName + "'";
            return PostgreHelper.FillDataRow(sql);
        }
        public static bool CreateAdmin(Admin info)
        {
            string sql = @"insert into sys_admin (username, openid, realname, create_time, last_login_time, 
                                                  last_login_ip, roles_name, role_list, role_name_list, isvalid)
                                                

                        values (@username, @openid, @realname, @create_time, @last_login_time, 
                                @last_login_ip, @roles_name, @role_list, @role_name_list, @isvalid)";

            DbParameter[] parms = {
                                        PostgreHelper.MakeInParam("@username", NpgsqlDbType.Varchar, 50, info.username),
                                        PostgreHelper.MakeInParam("@openid", NpgsqlDbType.Varchar, 50, info.openid),  
                                        PostgreHelper.MakeInParam("@realname", NpgsqlDbType.Varchar, 20, info.realname),
                                        PostgreHelper.MakeInParam("@create_time", NpgsqlDbType.TimestampTZ, -1, info.create_time),
                                        PostgreHelper.MakeInParam("@last_login_time", NpgsqlDbType.TimestampTZ, -1, info.last_login_time),

                                        PostgreHelper.MakeInParam("@last_login_ip", NpgsqlDbType.Varchar, 15, info.last_login_ip),
                                        PostgreHelper.MakeInParam("@roles_name", NpgsqlDbType.Varchar, 200, info.roles_name),
                                        PostgreHelper.MakeInParam("@role_list", NpgsqlDbType.Array | NpgsqlDbType.Integer, 200, info.role_list),
                                        PostgreHelper.MakeInParam("@role_name_list", NpgsqlDbType.Array | NpgsqlDbType.Varchar, 200, info.role_name_list),
                                        PostgreHelper.MakeInParam("@isvalid", NpgsqlDbType.Boolean, -1, info.isvalid),
                                        
                                    };
            int rlt = PostgreHelper.ExecuteNonQuery(PostGreSqlConnectionString, CommandType.Text, sql, parms);
            return rlt > 0;
        }

        public static string InsertUnit(int memberId, string loginName)
        {
            string msg = string.Empty;

            return msg;
        }




    }
}
