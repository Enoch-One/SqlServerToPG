using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cnool.Common;
using System.Data;
using System.Text.RegularExpressions;
using NewsToUnit;
using BlogToUnit.Tool;
using BlogToUnit.Entity;

namespace BlogToUnit
{
    class Program
    {
        static void Main(string[] args)
        {
           //string a="<img src=\"/uploads/2016editor/5/31/11/4b1a48f0-8608-49c6-86be-6a7aaeb6c67f.jpg\" alt=\"\" /></span></p><p><span style=\"font-size:16px;\"><img src=\"/uploads/2016editor/5/31/11/cac9b691-0ffc-4cc9-a030-f2273d4c6884.jpg\" alt=\"\" />";

            //delete();
           //Untils.GetHtml(a);
            //return;
            //selectMember();
            //string a = string.Empty; ;

            //Untils.GetHtmlImageUrl(a);
            //delete(10593);

            //string a = "测试啊<p><img src='/dadt/yndt/201103/W020110322380977845026.jpg'><img src='http://blog2.cnool.net/dadt/yndt/201103/W020110322380977845026.jpg'></p><img src='/dadt/yndt/201103/W020110322380977845026.jpg'>";
            //Untils.GetHtml(a);
            //string[] b = new string[] { "/dadt/yndt/201103/w020110322380977845026.jpg", "/dadt/yndt/201103.jpg" };
            //string a = "<p><img src='/dadt/W020110322380977845026.jpg' alt='' /></p><p><img src='/dadt/yndt/201103/W020110322380977845026.jpg'/></p><img src='http://cnool.net/dadt/yndt/201103/W0201103223809778450262.jpg'/>";
            //string sw = Untils.GetHtml(a);

            string username = string.Empty;
            string blogName = string.Empty;
            string sql = string.Empty;
            int status = 0;
            string loginName = string.Empty;
            int userId = 0;
            try
            {
                DataTable dt = new DataTable();
                DataConnection dc = new DataConnection();

                sql = "select * from DataTransfer where Status=0 ";
                dt = dc.FillDataTable(sql);
                if (dt == null || dt.Rows.Count == 0)
                {
                    Console.WriteLine("没有记录");
                    return;
                }
                else
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        username = Convert.ToString(dr["UserName"]);
                        string realName = Convert.ToString(dr["RealName"]);
                        string ip = Convert.ToString(dr["IP"]);
                        status = Convert.ToInt32(dr["Status"]);
                        //获取Member用户表的数据
                        sql = "select * from ra_pbMember  where LoginName= '" + username + "'";
                        //dt = dc.FillDataTable(sql);
                        DataRow drMember = dc.FillDataRow(sql);
                        userId = Convert.ToInt32(drMember["ID"]);
                        loginName = Convert.ToString(drMember["LoginName"]);
                        string face = Convert.ToString(drMember["Face"]);

                        Admin admin = new Admin();
                        admin.username = loginName;
                        admin.realname = realName;
                        admin.create_time = DateTime.Now;
                        admin.last_login_ip = ip;
                        admin.roles_name = "单元运营者";
                        string[] SearchkeyList = Untils.ConvertToSearchkey("单元运营者");
                        admin.role_name_list = SearchkeyList;
                        admin.isvalid = true;
                        admin.role_list = Array.ConvertAll<string, int>(Untils.ConvertToSearchkey(Convert.ToString(Convert.ToString(10002))),s=>int.Parse(s));//string[]转化int[]

                        var adminInfo = AdminManage.getAdminByName(loginName);
                        bool result = false;
                        if (adminInfo == null)
                        {
                            result = AdminManage.CreateAdmin(admin);
                        }
                        else
                        {
                            Untils.WriteProgramLog(loginName + "  已存在!");
                            result = true;
                        }
                        if (result)
                        {
                            result = UnitManage.InsertUnit(userId, loginName, face);
                            if (result)
                            {
                                sql = "update DataTransfer set Status=1 where UserName='" + username + "'";
                                dc.ExeSql(sql);
                                Untils.WriteProgramLog(loginName + "  导入数据成功!");
                            }
                            else
                            {
                                Untils.WriteProgramLog(loginName + "  导入数据失败!");
                            }
                        }
                        else
                        {
                            Untils.WriteProgramLog(loginName + "  导入Admin数据失败!");
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Untils.WriteProgramLog("something wrong in A method: " + e.Message);
                throw;
            }
        }

        //public static void selectMember()
        //{
        //    string[] member=new string[]{"山水清澈" ,"会飞的佳佳猫","龟山隐真","清风扶金","家有小儿","陈美英的博客","xiaobai1659","最初的地方","他的记忆",
        //        "qazwsx9642","幽谷百合","静静的长椅","煜榕","基度山人","别梦依依","haya01","中浪淘沙","猪猪2002","狂笑三声半","月下荷塘","鹏啸九天","高山临风","宁静致远丫头","缪卡妮",
        //        "喵小姐的博客","静忆","菜叶青虫","子烟小秘","蓝色的千纸鹤","中国青年","聊聊心情","三言两拍","山野农夫","悠然冰心","晨雪新语","四明水人",
        //        "竹影流动","不拘的风","晚礼服","rxxx103","六月的一天","wfhsyy","张老师的博客","点点滴滴的快乐","李凝桦","水吟吟","悠然冰心","力挥之亘","龟山隐真","梧桐树三更雨","慧宁"
        //        ,"妍尘","益达浪子","西门子","灵溪春晓"};
        //    DataConnection dc = new DataConnection();
        //    foreach (string name in member)
        //    {

        //    //获取Member用户表的数据
        //        string sql = "select * from ra_pbMember  where LoginName= '" + name + "'";
        //    //dt = dc.FillDataTable(sql);
        //    DataRow drMember = dc.FillDataRow(sql);
        //    string realName = Convert.ToString(drMember["RealName"]);
        //   // string loginName = Convert.ToString(drMember["LoginName"]);
        //    string ip = Convert.ToString(drMember["LastLoginIP"]);
        //    string face = Convert.ToString(drMember["Face"]);

        //    sql = "INSERT INTO DataTransfer (UserName,PostTime,RealName,IP) VALUES ('" + name + "','" + DateTime.Now + "','" + realName + "','" + ip + "')";
        //    dc.ExeSql(sql);
        //    }
        //}


        public static void delete()
        {
            int[] list = new int[] {10608,10607,10606,10605,10604,10603,10602,10601,10600,10599,10598,10597,10596,10595,10594 };
            foreach(var deleteId in  list){
            DataConnection dc = new DataConnection();

            string sql1 = "select * from article where unit_id=" + deleteId;
            DataTable dt = new DataTable();
            dt = PostgreHelper.FillDataTable(sql1);
            foreach (DataRow dr in dt.Rows)
            {
                long article_id = Convert.ToInt64(dr["id"]);
                string sql2 = "delete from article_comment where article_id=" + article_id;
                PostgreHelper.ExeSql(sql2, "Server=61.153.18.239;Port=5432;User Id=postgres;Password=Zx5p(wFwDXSxaxEHA7yX;Database=db_unit;");
            }
            string sql3 = "delete from article where unit_id=" + deleteId;
            PostgreHelper.ExeSql(sql3, "Server=61.153.18.239;Port=5432;User Id=postgres;Password=Zx5p(wFwDXSxaxEHA7yX;Database=db_unit;");
        }
        }


    }
}


