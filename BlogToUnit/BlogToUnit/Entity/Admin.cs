using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogToUnit.Entity
{
    public class Admin
    {
        public int id { get; set; }

        public string username { get; set; }

        public string openid { get; set; }

        public string realname { get; set; }

        public DateTime create_time { get; set; }

        public DateTime? last_login_time { get; set; }

        public string last_login_ip { get; set; }

        public string roles_name { get; set; }

        public int[] role_list { get; set; }

        public string[] role_name_list { get; set; }

        public bool isvalid { get; set; }

        public string face { get; set; }
    }
}
