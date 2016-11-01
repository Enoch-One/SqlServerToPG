using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogToUnit.Tool
{
    public class Unit
    {
        public int id { get; set; }

        public string unit_name { get; set; }

        public string unit_remark { get; set; }

        public int forum_id { get; set; }

        public DateTime create_time { get; set; }

        public bool isvalid { get; set; }

        public string creater { get; set; }

        public string modifier { get; set; }

        public DateTime? update_time { get; set; }

        public int industry_id { get; set; }

        public int unit_type { get; set; }

        public string unit_logo { get; set; }

        public int status { get; set; }

        public int article_count { get; set; }

        public string auditor { get; set; }

        public DateTime? audit_time { get; set; }

        public DateTime? last_publish_time { get; set; }

        public string[] tags { get; set; }

        public string username { get; set; }

        public string address { get; set; }

        public double longitude { get; set; }

        public double latitude { get; set; }

        public string phone { get; set; }

        public string linkman { get; set; }

        public string wechat { get; set; }

        public string website { get; set; }

        public string[] asmanagers { get; set; }

        public int order_number { get; set;}

        public string unit_domain { get; set; }
    }
}
