using SQLite;

namespace com.genteure.cqp.bangumi
{
    public class Subscriber
    {
        /**
         *
         * 番剧ID为0时QQ号一定为0
         * 代表全群的提前预告推送
         * 
         * */

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
