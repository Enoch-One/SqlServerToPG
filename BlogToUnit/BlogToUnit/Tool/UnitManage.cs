using BlogToUnit.Entity;
using Cnool.Common;
using NewsToUnit;
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
    public class UnitManage
    {
        private static string PostGreSqlConnectionString = "Server=61.153.18.239;Port=5432;User Id=postgres;Password=Zx5p(wFwDXSxaxEHA7yX;Database=db_unit;";
        public static int GetId()
        {
            string sql = "SELECT nextval('public_unit_id_seq')";
            int id = Convert.ToInt32(PostgreHelper.GetField(PostGreSqlConnectionString, sql));
            return id;
        }
        //查询是否已经存在单元
        //public static int getUnitCountByName(string loginName)
        //{
        //    string tblName="unit";
        //    string condition = "unit_name='" + loginName + "'@博客'";

        //    int count = PostgreHelper.GetCount(PostGreSqlConnectionString, tblName, condition);
        //    return count;
        //}

        //查询是否已经存在博客单元
        public static DataRow getUnitByBlogName(string loginName)
        {
            string sql = "select * from unit where unit_name='" + loginName + "@博客'";
            return PostgreHelper.FillDataRow(sql);
        }

        //查询是否已经存在单元
        public static DataRow getUnitByName(string loginName)
        {
            string sql = "select * from unit where username='" + loginName + "'";
            return PostgreHelper.FillDataRow(sql);
        }


        public static bool CreateUnit(Unit unit)
        {
            /**
             * 1.检测loginName+"@博客"单元是否存在
             *   存在:获取单元id
             *   不存在:执行2
             * 
             * 2.根据运营者名称查询单元
             *   无:将博客单元的运营者指定为 loginName  //username=loginName
             *   有:将博客单元的协助管理员指定为 loginName //asmanagers = [loginName]
             *   
             *   创建博客单元
             * */

            string sql = @"insert into unit ( id,unit_name, forum_id, create_time, isvalid, creater, 
                                             industry_id, unit_type, unit_logo, status, 
                                             article_count, auditor, audit_time, last_publish_time, tags, 
                                             username, address, longitude, latitude, phone, linkman, wechat, 
                                             website, asmanagers, order_number,unit_domain)
                                                

                        values ( @id,@unit_name, @forum_id, @create_time, @isvalid, @creater, 
                                             @industry_id, @unit_type, @unit_logo, @status, 
                                             @article_count, @auditor, @audit_time, @last_publish_time, @tags, 
                                             @username, @address, @longitude, @latitude, @phone, @linkman, @wechat, 
                                             @website, @asmanagers, @order_number,@unit_domain)";

            DbParameter[] parms = {
                                        PostgreHelper.MakeInParam("@id", NpgsqlDbType.Integer, -1, unit.id),
                                        PostgreHelper.MakeInParam("@unit_name", NpgsqlDbType.Varchar, 20, unit.unit_name),  
                                        //PostgreHelper.MakeInParam("@unit_remark", NpgsqlDbType.Varchar, 500, unit.unit_remark),
                                        PostgreHelper.MakeInParam("@forum_id", NpgsqlDbType.Integer, -1, unit.forum_id),
                                        PostgreHelper.MakeInParam("@create_time", NpgsqlDbType.TimestampTZ, -1, unit.create_time),
                                        PostgreHelper.MakeInParam("@isvalid", NpgsqlDbType.Boolean, -1, unit.isvalid),
                                        PostgreHelper.MakeInParam("@creater", NpgsqlDbType.Varchar, 50, unit.creater),

                                        //PostgreHelper.MakeInParam("@modifier", NpgsqlDbType.Varchar, 50, unit.modifier),
                                        //PostgreHelper.MakeInParam("@update_time", NpgsqlDbType.TimestampTZ, -1, unit.update_time),
                                        PostgreHelper.MakeInParam("@industry_id", NpgsqlDbType.Integer, -1, unit.industry_id),
                                        PostgreHelper.MakeInParam("@unit_type", NpgsqlDbType.Integer, -1, unit.unit_type),
                                        PostgreHelper.MakeInParam("@unit_logo", NpgsqlDbType.Varchar, 200, unit.unit_logo),
                                        PostgreHelper.MakeInParam("@status", NpgsqlDbType.Integer, -1, unit.status),

                                        PostgreHelper.MakeInParam("@article_count", NpgsqlDbType.Integer, -1, unit.article_count),
                                        PostgreHelper.MakeInParam("@auditor", NpgsqlDbType.Varchar, 50, unit.auditor),
                                        PostgreHelper.MakeInParam("@audit_time", NpgsqlDbType.TimestampTZ, -1, unit.audit_time),
                                        PostgreHelper.MakeInParam("@last_publish_time", NpgsqlDbType.TimestampTZ, -1, unit.last_publish_time),
                                        PostgreHelper.MakeInParam("@tags", NpgsqlDbType.Array | NpgsqlDbType.Varchar, -1, unit.tags),

                                        PostgreHelper.MakeInParam("@username", NpgsqlDbType.Varchar, 50, unit.username),
                                        PostgreHelper.MakeInParam("@address", NpgsqlDbType.Varchar, 200, unit.address),
                                        PostgreHelper.MakeInParam("@longitude", NpgsqlDbType.Double, -1, unit.longitude),
                                        PostgreHelper.MakeInParam("@latitude", NpgsqlDbType.Double, -1, unit.latitude),
                                        PostgreHelper.MakeInParam("@phone", NpgsqlDbType.Varchar, 50, unit.phone),
                                        PostgreHelper.MakeInParam("@linkman", NpgsqlDbType.Varchar, 50, unit.linkman),
                                        PostgreHelper.MakeInParam("@wechat", NpgsqlDbType.Varchar, 200, unit.wechat),

                                        PostgreHelper.MakeInParam("@website", NpgsqlDbType.Varchar, 200, unit.website),
                                        PostgreHelper.MakeInParam("@asmanagers", NpgsqlDbType.Array | NpgsqlDbType.Varchar, 50, unit.asmanagers),
                                        PostgreHelper.MakeInParam("@order_number", NpgsqlDbType.Integer, -1, unit.order_number),
                                        PostgreHelper.MakeInParam("@unit_domain", NpgsqlDbType.Varchar, 50, unit.unit_domain),
                                    };
            int rlt = PostgreHelper.ExecuteNonQuery(PostGreSqlConnectionString, CommandType.Text, sql, parms);
            return rlt > 0;
        }

        public static bool InsertUnit(int memberId, string loginName, string face)
        {
            //string msg = string.Empty;
            bool result = false;
            string sql = string.Empty;
            string blogName = string.Empty;
            int blogId = 0;
            string blogTitle = string.Empty;
            string memberLogo = string.Empty;
            DateTime blogAddTime = DateTime.Now;
            DateTime blogUpdateTime = DateTime.Now;
            int postCount = 0;
            int blogStatus = 0;
            try
            {
                DataTable dt = new DataTable();
                DataConnection dc = new DataConnection();
                //获取Blog博客表的数据
                sql = "select * from ra_bgBlog where MemberID=" + memberId;
                //dt = dc.FillDataTable(sql);
                DataRow dr = dc.FillDataRow(sql);
                //foreach (DataRow dr in dt.Rows)//?
                //{
                    blogId = Convert.ToInt32(dr["ID"]);
                    blogStatus = Convert.ToInt32(dr["status"]);
                    blogName = Convert.ToString(dr["Name"]);
                    blogTitle = Convert.ToString(dr["Title"]);
                    blogAddTime = Convert.ToDateTime(dr["AddTime"]);
                    blogUpdateTime = Convert.ToDateTime(dr["UpdateTime"]);
                    postCount = Convert.ToInt32(dr["PostCount"]);//文章数量
                //}
                //if (blogStatus == 0)
                //{
                //  sql = "update ra_bgBlog set status=1";
                //   dc.ExeSql(sql);

                //获取Member表的数据
                //sql = "select * from ra_pbMember where LoginName='" + loginName + "'"; //?
                //dt = dc.FillDataTable(sql);
                //foreach (DataRow dr in dt.Rows)
                //{
                // memberId = Convert.ToInt32(dr["ID"]);
                // memberLogo = "http://blog.cnool.net" + Convert.ToString(dr["Face"]);
                //}

                var unitInfo = getUnitByBlogName(loginName);
                if (unitInfo != null)
                {
                    ArticleManage.InsertArticle(blogId, Convert.ToInt32(unitInfo["id"]), blogName, loginName);
                    result = true;
                }
                else
                {
                    Unit unit = new Unit();
                    unit.username = loginName;
                    unitInfo = getUnitByName(loginName);
                    if (unitInfo != null)
                    {
                        unit.asmanagers = Untils.ConvertToSearchkey(loginName);
                        unit.username = null;
                    }
                    unit.id = GetId();
                    unit.unit_name = loginName + "@博客";
                    unit.create_time = blogAddTime;
                    unit.isvalid = true;
                    unit.creater = "system";
                    unit.unit_type = 1;
                    //unit.modifier = "system";
                    //unit.update_time = blogUpdateTime;
                    if (string.IsNullOrEmpty(face))
                    {
                        unit.unit_logo = null;
                    }
                    else
                    {
                        unit.unit_logo =Untils.GetHtmlImageUrl(face);
                    }
                    //unit.unit_remark = blogTitle;
                    unit.forum_id = 0;
                    unit.industry_id = 10040;
                    unit.status = 0;
                    unit.article_count = postCount;
                    unit.unit_domain = blogName + ".blog2.cnool.net";
                    result = CreateUnit(unit);
                    if (result)
                    {
                        //导入Article表
                        ArticleManage.InsertArticle(blogId, unit.id, blogName, loginName);
                    }
                    else
                    {
                        Untils.WriteProgramLog(loginName + "  导入Blog数据失败!");
                    }
                }
            }
            catch (Exception e)
            {
                Untils.WriteProgramLog("something wrong in A method: " + e.Message);
                throw;
            }
            return result;
        }

    }
}
