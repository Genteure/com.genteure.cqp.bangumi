using Newtonsoft.Json.Linq;
using FluentScheduler;
using System;
using System.Text;

namespace com.genteure.cqp.bangumi
{
    internal class Bangumi : IJob
    {
        public Bangumi() { }
        public Bangumi(JObject j)
        {
            cover = j["cover"].ToObject<string>();
            delay = j["delay"].ToObject<int>();
            ep_id = j["ep_id"].ToObject<int>();
            favorites = j["favorites"].ToObject<int>();
            is_published = j["is_published"].ToObject<int>();
            pub_index = j["pub_index"].ToObject<string>();
            pub_time = j["pub_time"].ToObject<string>();
            pub_ts = j["pub_ts"].ToObject<int>();
            season_id = j["season_id"].ToObject<int>();
            season_status = j["season_status"].ToObject<int>();
            square_cover = j["square_cover"].ToObject<string>();
            title = j["title"].ToObject<string>();

            pub_ts_datetime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(pub_ts);
        }

        public void Execute()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("【番】【{0}】更新啦！\n", title);
            sb.AppendFormat("{0} 更新时间：{1}\n", pub_index, pub_time);
            sb.AppendFormat("https://bangumi.bilibili.com/anime/{0}/play#{1} \n", season_id, ep_id);
            sb.Append("\n注：这是一个还在开发中的半成品插件——宅急送队长");
            // CoolQApi.SendPrivateMsg(Main.MasterQQ, sb.ToString());
            long[] g = { 627565437, 95349372, 423768065, 549858724 };
            foreach (var q in g)
                CoolQApi.SendGroupMsg(q, sb.ToString());
        }

        public string cover { get; set; }
        public int delay { get; set; }
        public int ep_id { get; set; }
        public int favorites { get; set; }
        public int is_published { get; set; }
        public string pub_index { get; set; }
        public string pub_time { get; set; }
        public int pub_ts { get; set; }
        public DateTime pub_ts_datetime { get; set; }
        public int season_id { get; set; }
        public int season_status { get; set; }
        public string square_cover { get; set; }
        public string title { get; set; }

    }
}
