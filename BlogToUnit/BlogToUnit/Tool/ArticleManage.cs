using BlogToUnit.Entity;
using BlogToUnit.Tool;
using Cnool.Common;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace NewsToUnit
{
    public class ArticleManage
    {
        private static string PostGreSqlConnectionString = "Server=61.153.18.239;Port=5432;User Id=postgres;Password=Zx5p(wFwDXSxaxEHA7yX;Database=db_unit;";
        public static long GetId()
        {
            string sql = "SELECT nextval('public_article_id_seq')";
            long id = Convert.ToInt64(PostgreHelper.GetField(PostGreSqlConnectionString, sql));
            return id;
        }
        public static int SelectArticle(int blogId)
        {
            int count = PostgreHelper.GetCount(PostGreSqlConnectionString, "article", "source_art_id=" + blogId);
            return count;
        }
        public static void DelArticle(int blogId)
        {
            string sql = "delete from article where source_art_id=" + blogId;
            PostgreHelper.ExeSql(sql, PostGreSqlConnectionString);
        }

        public static bool CreateArticle(Article info, List<Comment> comment)
        {
            int rlt = 0;
            DataTable dt = new DataTable();
            NpgsqlConnection cnn = new NpgsqlConnection(PostGreSqlConnectionString);
            NpgsqlCommand cm = new NpgsqlCommand();
            cm.Connection = cnn;
            cnn.Open();
            DbTransaction trans = cnn.BeginTransaction();
            try
            {
                string sql = @"insert into article (id, title, content, publish_time, 
                                                syn_forum, syn_weixin, forum_id, media_id, create_time, creater_id, 
                                                creater, hits, unit_id, status, last_modifier, modify_time,  
                                                industry_id, industry_name, summary, is_publish, has_picture, 
                                                comments, image_url,tags,search_tags,images,url)
                                                

                        values (@id,@title,@content,@publish_time,
                                @syn_forum, @syn_weixin, @forum_id, @media_id, @create_time, @creater_id,
                                @creater, @hits, @unit_id, @status, @last_modifier, @modify_time,
                                @industry_id, @industry_name, @summary, @is_publish, @has_picture,
                                @comments, @image_url,@tags,@search_tags,@images,@url )";
                //tags, source_cate_id  @tags, @source_cate_id
                DbParameter[] parms = {
                                        PostgreHelper.MakeInParam("@id", NpgsqlDbType.Bigint, -1, info.Id),
                                        //PostgreHelper.MakeInParam("@source_art_id", NpgsqlDbType.Integer, -1, info.Source_Art_Id),
                                        //PostgreHelper.MakeInParam("@source_name", NpgsqlDbType.Varchar, 50, info.Source_Name),  
                                        //PostgreHelper.MakeInParam("@source_url", NpgsqlDbType.Varchar, 500, info.Source_Url),
                                        PostgreHelper.MakeInParam("@title", NpgsqlDbType.Varchar, 255, info.Title),
                                        PostgreHelper.MakeInParam("@content", NpgsqlDbType.Text, -1, info.Content),
                                        PostgreHelper.MakeInParam("@publish_time", NpgsqlDbType.TimestampTZ, -1, info.Publish_Time),

                                        PostgreHelper.MakeInParam("@syn_forum", NpgsqlDbType.Boolean, -1, info.Syn_Forum),
                                        PostgreHelper.MakeInParam("@syn_weixin", NpgsqlDbType.Boolean, -1, info.Syn_Weixin),
                                        PostgreHelper.MakeInParam("@forum_id", NpgsqlDbType.Integer, -1, info.Forum_Id),
                                        PostgreHelper.MakeInParam("@media_id", NpgsqlDbType.Varchar, 80, info.Media_Id),
                                        PostgreHelper.MakeInParam("@create_time", NpgsqlDbType.TimestampTZ, -1, info.Create_Time),
                                        PostgreHelper.MakeInParam("@creater_id", NpgsqlDbType.Integer, -1, info.Creater_Id),

                                        PostgreHelper.MakeInParam("@creater", NpgsqlDbType.Varchar, 50, info.Creater),
                                        PostgreHelper.MakeInParam("@hits", NpgsqlDbType.Integer, -1, info.Hits),
                                        PostgreHelper.MakeInParam("@unit_id", NpgsqlDbType.Integer, -1, info.Unit_Id),
                                        PostgreHelper.MakeInParam("@status", NpgsqlDbType.Integer, -1, info.Status),
                                        PostgreHelper.MakeInParam("@last_modifier", NpgsqlDbType.Varchar, 50, info.Last_Modifier),
                                        PostgreHelper.MakeInParam("@modify_time", NpgsqlDbType.TimestampTZ, -1, info.Modify_Time),
                                        //PostgreHelper.MakeInParam("@is_del", NpgsqlDbType.Boolean, -1, info.Is_Del),
                                        
                                        //PostgreHelper.MakeInParam("@del_time", NpgsqlDbType.TimestampTZ, -1, info.Del_Time),
                                        PostgreHelper.MakeInParam("@industry_id", NpgsqlDbType.Integer, -1, info.Industry_Id),
                                        PostgreHelper.MakeInParam("@industry_name", NpgsqlDbType.Varchar, 20, info.Industry_Name),
                                        PostgreHelper.MakeInParam("@summary", NpgsqlDbType.Varchar, 500, info.Summary),
                                        PostgreHelper.MakeInParam("@is_publish", NpgsqlDbType.Boolean, -1, info.Is_Publish),
                                        PostgreHelper.MakeInParam("@has_picture", NpgsqlDbType.Boolean, -1, info.Has_Picture),

                                        PostgreHelper.MakeInParam("@comments", NpgsqlDbType.Integer, -1, info.Comments),
                                        PostgreHelper.MakeInParam("@image_url", NpgsqlDbType.Varchar, 500, info.Image_Url),
                                        PostgreHelper.MakeInParam("@tags", NpgsqlDbType.Array | NpgsqlDbType.Varchar, 20, info.Tags),
                                        PostgreHelper.MakeInParam("@search_tags", NpgsqlDbType.Array | NpgsqlDbType.Varchar , 20, info.Search_Tags),
                                        PostgreHelper.MakeInParam("@images", NpgsqlDbType.Array | NpgsqlDbType.Varchar , 500, info.Images),
                                        PostgreHelper.MakeInParam("@url", NpgsqlDbType.Varchar, 200, info.Url),
                                        //PostgreHelper.MakeInParam("@source_cate_id", NpgsqlDbType.Integer, -1,info.Source_cate_id),

                                        
                                    };
                rlt = PostgreHelper.ExecuteNonQuery(trans, CommandType.Text, sql, parms);
                // return rlt > 0;
                //PostgreHelper.ExecuteNonQuery(PostGreSqlConnectionString, CommandType.Text, sql, parms);


                foreach (var info_comm in comment)
                {
                    sql = @"insert into article_comment (article_id, comm_content, comm_username, comm_time, ip, auditor, 
                                                        audit_time, is_valid, shield_words, forum_topicid, forum_threadid, 
                                                        last_modifier, modify_time)
                                                

                        values (@article_id, @comm_content, @comm_username, @comm_time, @ip, @auditor, 
                                                        @audit_time, @is_valid, @shield_words, @forum_topicid, @forum_threadid, 
                                                        @last_modifier, @modify_time)";

                    DbParameter[] parms1 = {
                                        PostgreHelper.MakeInParam("@article_id", NpgsqlDbType.Bigint, -1, info_comm.article_id),
                                        PostgreHelper.MakeInParam("@comm_content", NpgsqlDbType.Varchar, 200, info_comm.comm_content),  
                                        PostgreHelper.MakeInParam("@comm_username", NpgsqlDbType.Varchar, 50, info_comm.comm_username),
                                        PostgreHelper.MakeInParam("@comm_time", NpgsqlDbType.TimestampTZ, -1, info_comm.comm_time),
                                        PostgreHelper.MakeInParam("@ip", NpgsqlDbType.Varchar, 15, info_comm.ip),
                                        PostgreHelper.MakeInParam("@auditor", NpgsqlDbType.Varchar, 50, info_comm.auditor),

                                        PostgreHelper.MakeInParam("@audit_time", NpgsqlDbType.TimestampTZ, -1, info_comm.audit_time),
                                        PostgreHelper.MakeInParam("@is_valid", NpgsqlDbType.Boolean, -1, info_comm.is_valid),
                                        PostgreHelper.MakeInParam("@shield_words", NpgsqlDbType.Varchar, 200, info_comm.shield_words),
                                        PostgreHelper.MakeInParam("@forum_topicid", NpgsqlDbType.Integer, -1, info_comm.forum_topicid),
                                        PostgreHelper.MakeInParam("@forum_threadid", NpgsqlDbType.Integer, -1, info_comm.forum_threadid),

                                        PostgreHelper.MakeInParam("@last_modifier", NpgsqlDbType.Varchar, 50, info_comm.last_modifier),
                                        PostgreHelper.MakeInParam("@modify_time", NpgsqlDbType.TimestampTZ, -1, info_comm.modify_time),
                                        
                                    };
                    rlt = PostgreHelper.ExecuteNonQuery(trans, CommandType.Text, sql, parms1);
                }
                trans.Commit();
            }
            catch(Exception ex)
            {
                throw;
                rlt = 0;
                trans.Rollback();
            }
            finally
            {
                cnn.Close();
                trans.Dispose();
                cnn.Dispose();
            }
            return rlt > 0;
        }


        public static void InsertArticle(int blogId, int parentId, string blogName, string loginName)
        {
            bool result = false;
            //int count = SelectArticle(parentId);
            //if (count > 0)
            //{
            //    DelArticle(parentId);
            //}
            DataTable dt = new DataTable();
            DataConnection dc = new DataConnection();
            string sql = "select EndArticleId from DataTransfer where UserName='" + loginName + "'";
            int endArticleId = Int32.Parse(dc.GetField(sql));
            try
            {
                sql = "select * from ra_bgArticle where BlogID=" + blogId + " and id>" + endArticleId + " order by ID";
                dt = dc.FillDataTable(sql);


                foreach (DataRow dr in dt.Rows)
                {
                    int articleId = Convert.ToInt32(dr["ID"]);
                    int bLogId = Convert.ToInt32(dr["BlogID"]);
                    string title = Convert.ToString(dr["Title"]);
                    string tags = Convert.ToString(dr["Tag"]);
                    string body = Convert.ToString(dr["Body"]);
                    int categoryId = Convert.ToInt32(dr["CategoryId"]);//分类
                    int channelId = Convert.ToInt32(dr["ChannelId"]);//频道 例如10006
                    string excerpt = Convert.ToString(dr["Excerpt"]);//摘要 summary 

                    string authorName = Convert.ToString(dr["AuthorName"]);
                    string sourceName = Convert.ToString(dr["SourceName"]);
                    string sourceUrl = Convert.ToString(dr["SourceUrl"]);
                    string coverUrl = Convert.ToString(dr["CoverUrl"]);
                    DateTime postTime = Convert.ToDateTime(dr["PostTime"]);
                    DateTime updateTime = Convert.ToDateTime(dr["UpdateTime"]);
                    string urlTime = updateTime.ToString("yyyy/MM/dd");


                    //将读取到的数据逐条插入postgresql数据库
                    Article article = new Article();//2147483729
                    //pgArticleId = ArticleManage.GetId();
                    article.Id = GetId();
                    //东方博客2012年度奖项名单
                    article.Title = title;
                    article.Content =Untils.GetHtml(body);
                    article.Publish_Time = postTime;
                    article.Create_Time = postTime;
                    //article.Source_Name = sourceName;

                    article.Modify_Time = updateTime;
                    if (Untils.GetHtmlImageUrlList(body).Length == 0)
                    {
                        article.Image_Url = Untils.GetHtmlImageUrl(coverUrl);
                    }
                    else
                    {
                        article.Image_Url = !string.IsNullOrEmpty(coverUrl) ? Untils.GetHtmlImageUrl(coverUrl) : Untils.GetHtmlImageUrlList(body)[0];
                    }
                    if (!string.IsNullOrEmpty(coverUrl) || Untils.GetHtmlImageUrlList(body).Length > 0)
                    {
                        article.Has_Picture = true;
                    }
                    else
                    {
                        article.Has_Picture = false;
                    }
                    article.Images = Untils.GetHtmlImageUrlList(body);
                    //article.Summary = Untils.NoHTML(body);
                    article.Summary = Untils.NoHTML(body).Length > 140 ? Untils.NoHTML(body).Substring(0, 140) : Untils.NoHTML(body);

                    //article.Source_Art_Id = parentId;
                    //article.Source_Url = "http://" + blogName + ".blog2.cnool.net/Article/" + urlTime + "/" + articleId + ".html";
                    article.Url = "http://blog.cnool.net/article/" + article.Id + ".html";

                    article.Industry_Id = 10040;
                    article.Industry_Name = "博客";

                    article.Unit_Id = parentId;
                    article.Comments = 0;
                    article.Status = 0;
                    article.Is_Publish = true;
                    article.Syn_Weixin = false;
                    article.Media_Id = null;
                    article.Creater_Id = 0;
                    article.Creater = authorName;

                    //article.Is_Del = false;
                    //article.Del_Time = DateTime.Parse("1970-1-1");
                    //article.Source_cate_id = 0;
                    string[] SearchkeyList = Untils.ConvertToSearchkey(tags); //每个tag不能超过20个字
                    article.Tags = SearchkeyList;
                    article.Search_Tags = SearchkeyList;
                    if (SearchkeyList != null)
                    {
                        //关键词转tag入库  0
                        foreach (string tag in SearchkeyList)
                        {
                            ArticleManage.CreateAticleTag(article.Id, tag, 0);
                        }
                    }
                    //获取Comment评论表的数据
                    sql = "select * from ra_pbComment where ParentID=" + articleId;
                    dt = dc.FillDataTable(sql);
                    if (dt!=null)
                    {
                        article.Comments = dt.Rows.Count;
                    }
                    List<Comment> list = new List<Comment>();
                    foreach (DataRow dr_comment in dt.Rows)
                    {
                        int parentID = Convert.ToInt32(dr_comment["ParentID"]);
                        string body_comment = Convert.ToString(dr_comment["Body"]);
                        string authorName_comment = Convert.ToString(dr_comment["AuthorName"]);
                        DateTime addTime = Convert.ToDateTime(dr_comment["AddTime"]);
                        string authorIP = Convert.ToString(dr_comment["AuthorIP"]);
                        DateTime updateTime_comment = Convert.ToDateTime(dr_comment["UpdateTime"]);

                        //将读取到的数据逐条插入postgresql数据库
                        Comment comment = new Comment();
                        comment.article_id = article.Id;
                        comment.comm_content = body_comment;
                        comment.comm_username = authorName_comment;
                        comment.comm_time = addTime;
                        comment.ip = authorIP;
                        comment.is_valid = true;
                        comment.forum_topicid = 0;
                        comment.forum_threadid = 0;
                        comment.modify_time = updateTime_comment;
                        list.Add(comment);
                    }
                    result = CreateArticle(article, list);
                    if (result)
                    {
                        //插入用户最后操作的Article的ID
                        sql = "update DataTransfer set EndArticleId=" + articleId + " where UserName='" + loginName + "'";
                        dc.ExeSql(sql);
                    }
                    else
                    {
                        Untils.WriteProgramLog(loginName + "  导入Article数据失败!");
                    }
                }
                //return result;
            }
            catch (Exception e)
            {
                Untils.WriteProgramLog("something wrong in A method: " + e.Message);
                throw;
            }

        }


        public static bool CreateAticleTag(long articleId, string tag, int type)
        {
            if (tag.Length > 16)
            {
                return true;
            }
            string sql = @"insert into article_tag (article_id,tag_name,tag_type,tag_property_id)
                        values (@article_id,@tag_name,@tag_type,@tag_property_id)
            ";
            DbParameter[] parms = {
                                       PostgreHelper.MakeInParam("@article_id", NpgsqlDbType.Bigint, -1, articleId),
                                        PostgreHelper.MakeInParam("@tag_name", NpgsqlDbType.Varchar, 20, tag),
                                        PostgreHelper.MakeInParam("@tag_type", NpgsqlDbType.Integer, -1, type),  
                                        PostgreHelper.MakeInParam("@tag_property_id", NpgsqlDbType.Integer, -1, 0),
                                    };
            int rlt = PostgreHelper.ExecuteNonQuery(PostGreSqlConnectionString, CommandType.Text, sql, parms);
            return rlt > 0;
        }


        public static bool CreateAticleImage(long id, string image)
        {
            string sql = @"insert into article_picture (article_id, image_url)
                            values (@id,@image_url)
            ";
            DbParameter[] parms = {
                                        PostgreHelper.MakeInParam("@id", NpgsqlDbType.Bigint, -1, id),
                                        PostgreHelper.MakeInParam("@image_url", NpgsqlDbType.Varchar, -1, image),
                                    };
            int rlt = PostgreHelper.ExecuteNonQuery(PostGreSqlConnectionString, CommandType.Text, sql, parms);
            return rlt > 0;
        }

        public static int SelectId(int minId, int maxId)
        {
            int rlt = 0;
            string sql = "select max(id) from article where id>" + minId + "and id<" + maxId;
            object obj = PostgreHelper.ExecuteScalar(PostGreSqlConnectionString, CommandType.Text, sql, null);
            if (Convert.IsDBNull(obj))
            {
                rlt = 0;
            }
            else
            {
                rlt = Convert.ToInt32(obj);
            }
            return rlt;
        }




        //        public static bool CreateAticleTag(long articleId, string tag)
        //        {
        //            string sql = @"insert into article_tag (article_id,tag_name,tag_type,tag_property_id)
        //                        values (@article_id,@tag_name,@tag_type,@tag_property_id)
        //            ";
        //            DbParameter[] parms = {
        //                                        PostgreHelper.MakeInParam("@article_id", NpgsqlDbType.Bigint, -1, articleId),
        //                                        PostgreHelper.MakeInParam("@tag_name", NpgsqlDbType.Varchar, 20, tag),
        //                                        PostgreHelper.MakeInParam("@tag_type", NpgsqlDbType.Integer, -1, 10),  
        //                                        PostgreHelper.MakeInParam("@tag_property_id", NpgsqlDbType.Integer, -1, 0),
        //                                    };
        //            int rlt = PostgreHelper.ExecuteNonQuery(PostGreSqlConnectionString, CommandType.Text, sql, parms);
        //            return rlt > 0;
        //        }
    }
}
