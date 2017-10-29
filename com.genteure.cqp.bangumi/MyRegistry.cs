using System;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentScheduler;
using Newtonsoft.Json.Linq;

namespace com.genteure.cqp.bangumi
{
    public class MyRegistry : Registry
    {
        private const string FETCH_AM = "FetchBangumiData-AM";
        private const string FETCH_PM = "FetchBangumiData-PM";
        private const string FETCH_RETRY = "FetchBangumiData-Retry";
        private const string BANGUMI_NAME = "Bangumi";
        private const int TRY_INTERVAL_MINUTES = 5;

        public MyRegistry()
        {
            // 每天两次数据更新
            // UTC 7 点 / UTC+8 3 点
            Schedule(() => FetchAndUpdateBangumiData()).WithName(FETCH_AM).ToRunNow().AndEvery(1).Days().At(7, 3);
            Schedule(() => FetchAndUpdateBangumiData()).WithName(FETCH_PM).ToRunEvery(1).Days().At(7 + 12, 3);
        }

        /// <summary>
        /// 下载处理番剧数据
        /// </summary>
        private async void FetchAndUpdateBangumiData()
        {
            try
            {
                // 下载解析数据
                string strdata = await GetBangumiAsync();
                JObject jo = JObject.Parse(strdata);

                // 检查是否成功
                if (jo["code"].ToObject<int>() != 0 || jo["message"].ToObject<string>() != "success")
                    throw new Exception($"Bangumi Server Error: code{jo["code"].ToObject<int>()}:, message:{jo["message"].ToObject<string>()}");

                // 将所有番剧数据收集起来
                List<Bangumi> blist = new List<Bangumi>();
                foreach (JObject day in jo["result"] as JArray)
                    foreach (JObject bangumi in day["seasons"] as JArray)
                        blist.Add(new Bangumi(bangumi));

                // 保存获取到的所有番名
                blist.ForEach(x => Main.BangumiName[x.season_id] = x.title);

                // 删除所有旧的番剧定时任务
                JobManager.RemoveJob(BANGUMI_NAME);

                // 过滤出 (还未发布) 并且 (发布时间距离当前时间不到13小时) 的番剧
                DateTime before = DateTime.UtcNow + new TimeSpan(13, 0, 0);
                blist.Where(x => x.is_published == 0 && x.pub_ts_datetime < before).ToList().ForEach(x =>
                {
                    // 计划定时任务
                    JobManager.AddJob(x, z => z.WithName(BANGUMI_NAME).ToRunOnceAt(x.pub_ts_datetime));
                });
            }
            catch (Exception)
            {
                JobManager.RemoveJob(FETCH_RETRY);
                JobManager.AddJob(() => FetchAndUpdateBangumiData(), x => x.WithName(FETCH_RETRY).ToRunOnceIn(TRY_INTERVAL_MINUTES).Minutes()); // Retry
                throw;
            }
        }

        private static async Task<string> GetBangumiAsync()
        {
            var request = (HttpWebRequest)WebRequest.Create("http://bangumi.bilibili.com/web_api/timeline_global");

            request.Timeout = 10000;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36";
            request.Referer = "http://bangumi.bilibili.com/anime/timeline";

            var response = (HttpWebResponse)(await request.GetResponseAsync());
            var responseString = new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
            return responseString;
        }

    }
}
