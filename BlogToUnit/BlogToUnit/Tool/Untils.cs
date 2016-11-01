using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.IO;

namespace BlogToUnit.Tool
{
    public class Untils
    {
        //string转换为数组
        public static string[] ConvertToSearchkey(string searchkey)
        {
            List<string> list = new List<string>();
            string[] s = searchkey.Split(',');
            foreach (var item in s)
            {
                if (string.IsNullOrEmpty(item))
                {
                    continue;
                }
                if (item.Length > 16)
                {
                    continue;
                }
                list.Add(item);
            }
            if (list.Count == 0)
            {
                return null;
            }
            return list.ToArray();
        }

        static object locker = new object();
        /// <summary>
        /// 重要信息写入日志
        /// </summary>
        /// <param name="logs">日志列表，每条日志占一行</param>
        public static void WriteProgramLog(string log)
        {
            lock (locker)
            {
                string LogAddress = Environment.CurrentDirectory + "\\Log";
                if (!Directory.Exists(LogAddress + "\\PRG"))
                {
                    Directory.CreateDirectory(LogAddress + "\\PRG");
                }
                LogAddress = string.Concat(LogAddress, "\\PRG\\",
                 DateTime.Now.Year, '-', DateTime.Now.Month, '-',
                 DateTime.Now.Day, "_program.log");
                StreamWriter sw = new StreamWriter(LogAddress, true);
                sw.WriteLine(string.Format("[{0}] {1}", DateTime.Now.ToString(), log));
                sw.Close();
            }
        }

        public static string[] GetHtmlImageUrlList(string sHtmlText)
        {
            string imgSrc = "";
            // 定义正则表达式用来匹配 img 标签 
            //Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);
            //<img alt="查看更多精彩图片" onload="var image=new Image();image.src=this.src;if(image.width&gt;0 _fcksavedurl=" border="0" src="http://photo1.hexun.com/p/2006/0425/18694/b_8FB7A2DC2E0BA1D9.jpg" />

            CsQuery.CQ cq = sHtmlText;
            int i = 0;
            var imgs = cq["img"];
            string[] sUrlList = new string[imgs.Count()];
            foreach (var img in imgs)
            {
                try
                {
                    var src = img.GetAttribute("src");
                    if (!src.StartsWith("data:image/"))
                    {
                        if (!src.StartsWith("http://"))
                        {
                            imgSrc = "http://blog2.cnool.net" + src;
                        }
                        else
                        {
                            imgSrc = src;
                        }
                    }
                    //sUrlList[i++] = match.Groups["imgUrl"].Value;
                    sUrlList[i++] = imgSrc;
                }
                catch (Exception) {
                    continue; 
                }
            }

            // 搜索匹配的字符串 
            //MatchCollection matches = regImg.Matches(sHtmlText);
            //int i = 0;
            //string[] sUrlList = new string[matches.Count];

            //// 取得匹配项列表 
            //foreach (Match match in matches)
            //{
            //    string img = match.Groups["imgUrl"].Value;
            //    if (!img.StartsWith("data:image/"))
            //    {
            //        if (!img.StartsWith("http://"))
            //        {
            //            //imgSrc = img.ToLower().Replace(img, "http://blog2.cnool.net" + img);
            //            imgSrc = "http://blog2.cnool.net" + img;
            //        }
            //        else
            //        {
            //            imgSrc = img;
            //        }
            //    }
            //    //sUrlList[i++] = match.Groups["imgUrl"].Value;
            //    sUrlList[i++] = imgSrc;
            //}
            //imgSrc = String.Join(",", sUrlList);
            //return imgSrc;
            return sUrlList;
        }



        //将正文中没有 http:// 开头的img路径替换
        //public static string GetHtml(string sHtmlText)
        //{
        //    //string resultHtml = string.Empty; 
        //    // 定义正则表达式用来匹配 img 标签 
        //    Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>", RegexOptions.IgnoreCase);

        //    // 搜索匹配的字符串 
        //    MatchCollection matches = regImg.Matches(sHtmlText);
        //    //int i = 0;
        //    string[] sUrlList = new string[matches.Count];
        //    foreach (Match match in matches)
        //    {
        //        string img = match.Groups["imgUrl"].Value;
        //        //sUrlList[i++] = match.Groups["imgUrl"].Value;
        //        //foreach (var img in sUrlList)
        //        //{
        //        if (!img.StartsWith("data:image/"))
        //        {
        //            if (!img.Contains("this.src"))
        //            {
        //                if (!img.Contains("http://"))
        //                {
        //                    sHtmlText = Regex.Replace(sHtmlText, img, "http://blog2.cnool.net" + img);
        //                }
        //            }
        //        }
        //        //}
        //    }
        //    return sHtmlText;
        //}

        public static string GetHtml(string sHtmlText)
        {
            // 搜索匹配的字符串 
            CsQuery.CQ cq = sHtmlText;
            int i = 0;
            var imgs = cq["img"];
            string[] sUrlList = new string[imgs.Count()];
            foreach (var img in imgs)
            {
                try
                {
                    var src = img.GetAttribute("src");
                    if (!src.Contains("file://"))
                    {
                        if (!src.StartsWith("data:image/"))
                        {
                            if (!src.StartsWith("http://"))
                            {
                                sHtmlText = Regex.Replace(sHtmlText, src, "http://blog2.cnool.net" + src);
                            }
                        }
                    }
                }
                catch (Exception) {
                    continue; 
                }
            }
            return sHtmlText;
        }



        ////判断img是否 http:// 开头,否 则处理
        public static string GetHtmlImageUrl(string img)
        {
            string imgUrl = string.Empty;
            if (!string.IsNullOrEmpty(img))
            {
                if (!img.StartsWith("data:image/"))
                {
                    if (!img.StartsWith("http://"))
                    {
                        //imgUrl = img.ToLower().Replace(img, "http://blog2.cnool.net" + img);
                        imgUrl = "http://blog2.cnool.net" + img;
                    }
                    else
                    {
                        imgUrl = img;
                    }
                }
                else
                {
                    imgUrl = null;
                }
                return imgUrl;
            }
            else
            {
                return null;
            }

        }



        public static string NoHTML(string Htmlstring)
        {
            //删除脚本
            Htmlstring = Htmlstring.Replace("\r\n", "");
            Htmlstring = Regex.Replace(Htmlstring, @"<script.*?</script>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<style.*?</style>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<.*?>", "", RegexOptions.IgnoreCase);
            //删除HTML
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            Htmlstring = Htmlstring.Replace("<", "");
            Htmlstring = Htmlstring.Replace(">", "");
            Htmlstring = Htmlstring.Replace("\r\n", "");
            string endString = Htmlstring;
            if (Htmlstring.Contains("[cnool-page]"))
            {
                //Htmlstring.Replace("[cnool-page]", "");
                endString = Htmlstring.Replace("[cnool-page]", "");
            }
            return endString.Trim();
        }

    }
}
