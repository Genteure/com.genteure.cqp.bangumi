using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.genteure.cqp.bangumi
{
    public class Subscriber
    {
        /// <summary>
        /// ID
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// 番剧ID
        /// </summary>
        [Indexed]
        public int BangumiID { get; set; }

        /// <summary>
        /// 群号
        /// </summary>
        [Indexed]
        public long GroupID { get; set; }

        /// <summary>
        /// QQ号
        /// </summary>
        [Indexed]
        public long QQID { get; set; }
    }
}
