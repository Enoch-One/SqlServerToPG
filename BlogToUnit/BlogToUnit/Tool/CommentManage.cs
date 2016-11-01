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
    public class CommentManage
    {
        private static string PostGreSqlConnectionString = "Server=61.153.18.239;Port=5432;User Id=postgres;Password=Zx5p(wFwDXSxaxEHA7yX;Database=db_unit;";
        public static long GetId()
        {
            string sql = "SELECT nextval('public_article_comment_id_seq')";
            long id = Convert.ToInt64(PostgreHelper.GetField(PostGreSqlConnectionString, sql));
            return id;
        }
        public static int SelectComment(long articleId)
        {
            int count = PostgreHelper.GetCount(PostGreSqlConnectionString, "article_comment", "article_id=" + articleId);
            return count;
        }
        public static void DelComment(long articleId)
        {
            string sql = "delete from article_comment where article_id=" + articleId;
            PostgreHelper.ExeSql(sql, PostGreSqlConnectionString);
        }
        public static bool Createcomment(Comment info)
        {
            string sql = @"insert into article_comment (article_id, comm_content, comm_username, comm_time, ip, auditor, 
                                                        audit_time, is_valid, shield_words, forum_topicid, forum_threadid, 
                                                        last_modifier, modify_time)
                                                

                        values (@article_id, @comm_content, @comm_username, @comm_time, @ip, @auditor, 
                                                        @audit_time, @is_valid, @shield_words, @forum_topicid, @forum_threadid, 
                                                        @last_modifier, @modify_time)";

            DbParameter[] parms = {
                                        PostgreHelper.MakeInParam("@article_id", NpgsqlDbType.Bigint, -1, info.article_id),
                                        PostgreHelper.MakeInParam("@comm_content", NpgsqlDbType.Text, -1, info.comm_content),  
                                        PostgreHelper.MakeInParam("@comm_username", NpgsqlDbType.Varchar, 50, info.comm_username),
                                        PostgreHelper.MakeInParam("@comm_time", NpgsqlDbType.TimestampTZ, -1, info.comm_time),
                                        PostgreHelper.MakeInParam("@ip", NpgsqlDbType.Varchar, 15, info.ip),
                                        PostgreHelper.MakeInParam("@auditor", NpgsqlDbType.Varchar, 50, info.auditor),

                                        PostgreHelper.MakeInParam("@audit_time", NpgsqlDbType.TimestampTZ, -1, info.audit_time),
                                        PostgreHelper.MakeInParam("@is_valid", NpgsqlDbType.Boolean, -1, info.is_valid),
                                        PostgreHelper.MakeInParam("@shield_words", NpgsqlDbType.Varchar, 200, info.shield_words),
                                        PostgreHelper.MakeInParam("@forum_topicid", NpgsqlDbType.Integer, -1, info.forum_topicid),
                                        PostgreHelper.MakeInParam("@forum_threadid", NpgsqlDbType.Integer, -1, info.forum_threadid),

                                        PostgreHelper.MakeInParam("@last_modifier", NpgsqlDbType.Varchar, 50, info.last_modifier),
                                        PostgreHelper.MakeInParam("@modify_time", NpgsqlDbType.TimestampTZ, -1, info.modify_time),
                                        
                                    };
            int rlt = PostgreHelper.ExecuteNonQuery(PostGreSqlConnectionString, CommandType.Text, sql, parms);
            return rlt > 0;
        }

        public static void InsertComment(long articleId, long article_id)
        {
            //int count = SelectComment(article_id);
            //if (count > 0)
            //{
            //    DelComment(article_id);
            //}
            try
            {
                DataTable dt = new DataTable();
                DataConnection dc = new DataConnection();
                //获取Comment评论表的数据
                string sql = "select * from ra_pbComment where ParentID=" + articleId;
                dt = dc.FillDataTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    int parentID = Convert.ToInt32(dr["ParentID"]);
                    string body = Convert.ToString(dr["Body"]);
                    string authorName = Convert.ToString(dr["AuthorName"]);
                    DateTime addTime = Convert.ToDateTime(dr["AddTime"]);
                    string authorIP = Convert.ToString(dr["AuthorIP"]);
                    DateTime updateTime = Convert.ToDateTime(dr["UpdateTime"]);

                    //将读取到的数据逐条插入postgresql数据库
                    Comment comment = new Comment();
                    comment.article_id = article_id;
                    comment.comm_content = body;
                    comment.comm_username = authorName;
                    comment.comm_time = addTime;
                    comment.ip = authorIP;
                    comment.is_valid = true;
                    comment.forum_topicid = 0;
                    comment.forum_threadid = 0;
                    comment.modify_time = updateTime;
                    Createcomment(comment);
                }
                //return result;
            }
            catch (Exception e)
            {
                Untils.WriteProgramLog("something wrong in A method: " + e.Message);
                throw;
            }
        }
    }
}
