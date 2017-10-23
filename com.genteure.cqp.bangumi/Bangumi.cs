using Newtonsoft.Json.Linq;
using FluentScheduler;
using System;

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
            throw new NotImplementedException();
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
