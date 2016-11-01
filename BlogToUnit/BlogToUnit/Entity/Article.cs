using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsToUnit
{
    public class Article
    {
        #region Properties

        public long Id { get; set; }

        public int Source_Art_Id { get; set; }

        public string Source_Name { get; set; }

        public string Source_Url { get; set; }

        public int Industry_Id { get; set; }

        public string Industry_Name { get; set; }

        public int Unit_Id { get; set; }

        public string Title { get; set; }

        public string Summary { get; set; }

        public string Image_Url { get; set; }

        public string[] Images { get; set; }

        public string Content { get; set; }

        public int Hits { get; set; }

        public int Comments { get; set; }

        public int Status { get; set; }

        public bool Is_Publish { get; set; }

        public DateTime? Publish_Time { get; set; }

        public bool Has_Picture { get; set; }

        public bool Syn_Forum { get; set; }

        public bool Syn_Weixin { get; set; }

        public int Forum_Id { get; set; }

        public string Media_Id { get; set; }

        public int Creater_Id { get; set; }

        public string Creater { get; set; }

        public DateTime Create_Time { get; set; }

        public string Last_Modifier { get; set; }

        public DateTime? Modify_Time { get; set; }

        public bool Is_Del { get; set; }

        public DateTime? Del_Time { get; set; }

        public string[] Tags { get; set; }

        public int Source_cate_id { get; set; }

        public string[] Search_Tags { get; set; }

        public string Url { get; set; }

        #endregion
    }
}
