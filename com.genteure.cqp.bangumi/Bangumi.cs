using FluentScheduler;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text;

namespace com.genteure.cqp.bangumi
{
    internal class Bangumi : IJob
    {
        public Bangumi() { }
        public Bangumi(JObject j)
        {
            cover = j["cover"].ToObject<string>();
            title = j["title"].ToObject<string>();
            ep_id = j["ep_id"].ToObject<int>();
            favorites = j["favorites"].ToObject<int>();
            is_published = j["is_published"].ToObject<int>();
            index = j["pub_index"]?.ToObject<string>() ?? j["delay_index"]?.ToObject<string>() ?? string.Empty;
            pub_time = j["pub_time"].ToObject<string>();
            pub_ts = j["pub_ts"].ToObject<int>();
            delay = j["delay"].ToObject<int>();
            delay_reason = j["delay_reason"]?.ToObject<string>() ?? string.Empty;
            season_id = j["season_id"].ToObject<int>();
            season_status = j["season_status"].ToObject<int>();
            square_cover = j["square_cover"].ToObject<string>();


            pub_ts_datetime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(pub_ts);
        }

        async void IJob.Execute()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("【{0}】", title);
            if (delay == 0)
            {
                sb.AppendLine("更新啦！");
                sb.AppendFormat("{0} 更新时间：{1}\n", index, pub_time);
                sb.AppendFormat("https://bangumi.bilibili.com/anime/{0}/play#{1} ", season_id, ep_id);
            }
            else
            {
                sb.AppendLine(delay_reason);
            }

            var list = (await Main.db.Table<Subscriber>().Where(x => x.BangumiID == season_id).ToListAsync()).GroupBy(x => x.GroupID, x => x.QQID);
            foreach (var group in list)
            {
                string at = string.Empty;
                foreach (var qq in group)
                    at += CoolQApi.CQC_At(qq);
                CoolQApi.SendGroupMsg(group.Key, sb.ToString() + "\n\n" + at);
            }
        }

        public string cover { get; set; }
        public int delay { get; set; }
        public string delay_reason { get; set; }
        public int ep_id { get; set; }
        public int favorites { get; set; }
        public int is_published { get; set; }
        public string index { get; set; }
        public string pub_time { get; set; }
        public int pub_ts { get; set; }
        public DateTime pub_ts_datetime { get; set; }
        public int season_id { get; set; }
        public int season_status { get; set; }
        public string square_cover { get; set; }
        public string title { get; set; }

    }
}
