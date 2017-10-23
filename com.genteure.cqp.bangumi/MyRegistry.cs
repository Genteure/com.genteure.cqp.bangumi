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
        private const int TRY_INTERVAL_MINUTES = 5;

        public MyRegistry()
        {
            // 每天两次数据更新
            // UTC 7 点 / UTC+8 3 点
            Schedule(() => FetchAndUpdateBangumiData()).WithName(FETCH_AM).ToRunNow().AndEvery(1).Days().At(7, 3);
            Schedule(() => FetchAndUpdateBangumiData()).WithName(FETCH_PM).ToRunEvery(1).Days().At(7 + 12, 3);

            // Schedule an IJob to run at an interval
            // Schedule<MyJob>().ToRunNow().AndEvery(2).Seconds();

            // Schedule an IJob to run once, delayed by a specific time interval
            // Schedule<MyJob>().ToRunOnceIn(5).Seconds();

            // Schedule a simple job to run at a specific time
            // Schedule(() => Console.WriteLine("It's 9:15 PM now.")).ToRunEvery(1).Days().At(21, 15);

            // Schedule a more complex action to run immediately and on an monthly interval
            // Schedule<MyComplexJob>().ToRunNow().AndEvery(1).Months().OnTheFirst(DayOfWeek.Monday).At(3, 0);

            // Schedule a job using a factory method and pass parameters to the constructor.
            // Schedule(() => new MyComplexJob("Foo", DateTime.Now)).ToRunNow().AndEvery(2).Seconds();

            // Schedule multiple jobs to be run in a single schedule
            // Schedule<MyJob>().AndThen<MyOtherJob>().ToRunNow().AndEvery(5).Minutes();


        }

        /// <summary>
        /// 下载处理最近12小时的番剧数据
        /// </summary>
        private async void FetchAndUpdateBangumiData()
        {
            try
            {
                string strdata = await GetBangumiAsync();
                JObject jo = JObject.Parse(strdata);

                if (jo["code"].ToObject<int>() != 0 || jo["message"].ToObject<string>() != "success")
                    throw new Exception($"Bangumi Server Error: code{jo["code"].ToObject<int>()}:, message:{jo["message"].ToObject<string>()}");

                List<Bangumi> blist = new List<Bangumi>();

                foreach (JObject day in jo["result"] as JArray)
                    foreach (JObject bangumi in day["seasons"] as JArray)
                        blist.Add(new Bangumi(bangumi));

            }
            catch (Exception)
            {
                JobManager.RemoveJob(FETCH_RETRY);
                Schedule(() => FetchAndUpdateBangumiData()).WithName(FETCH_RETRY).ToRunOnceIn(TRY_INTERVAL_MINUTES).Minutes(); // Retry
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

        private readonly DateTime HALF_DAY = new DateTime(0, 0, 0, 12, 0, 0); // TODO: Change this

    }
}
