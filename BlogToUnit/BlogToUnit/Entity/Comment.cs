using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogToUnit.Entity
{
    public class Comment
    {
        public long id { get; set; }

        public long article_id { get; set; }

        public string comm_content { get; set; }

        public string comm_username { get; set; }

        public DateTime comm_time { get; set; }

        public string ip { get; set; }

        public string auditor { get; set; }

        public DateTime? audit_time { get; set; }

        public bool is_valid { get; set; }

        public string shield_words { get; set; }

        public int forum_topicid { get; set; }

        public int forum_threadid { get; set; }

        public string last_modifier { get; set; }

        public DateTime modify_time { get; set; }

    }
}
