using FluentScheduler;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace com.genteure.cqp.bangumi
{
    internal static class Main
    {
        internal const string APP_ID = "com.genteure.cqp.bangumi";
        internal const int MASTER_QQ = 244827448;
        internal const string DATABASE_FILE_NAME = "db.db";

        internal const string COMMAND_NAME1 = ".追番";
        internal const string COMMAND_NAME2 = ".bangumi";

        internal const string NO_BANGUMI_ID = "你没写番剧ID！";
        internal const string BANGUMI_ID_NOT_NUMBER = "番剧ID是整数数字！";

        internal static SQLiteAsyncConnection db;



        /// <summary>
        /// 启动插件初始化
        /// </summary>
        /// <returns></returns>
        [DllExport("_eventStartup", CallingConvention.StdCall)]
        internal static CoolQApi.Event Startup()
        {
            JobManager.UseUtcTime();
            JobManager.JobException += x => CoolQApi.SendPrivateMsg(MASTER_QQ, x.Name + x.Exception.ToString());

            db = new SQLiteAsyncConnection(CoolQApi.GetAppDirectory() + DATABASE_FILE_NAME,
                SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex);
            db.CreateTableAsync<Subscriber>();

            JobManager.Initialize(new MyRegistry());
            return CoolQApi.Event.Ignore;
        }

        /// <summary>
        /// 插件停止退出
        /// </summary>
        /// <returns></returns>
        [DllExport("_eventExit", CallingConvention.StdCall)]
        internal static CoolQApi.Event Exit()
        {
            JobManager.StopAndBlock();
            //TODO: 停止计时器系统
            return CoolQApi.Event.Ignore;
        }


        /// <summary>
        /// 处理群聊消息
        /// </summary>
        /// <param name="subType">？？？？？</param>
        /// <param name="sendTime">发送时间</param>
        /// <param name="fromGroup">群号</param>
        /// <param name="fromQQ">发送人QQ号</param>
        /// <param name="fromAnonymous">匿名ID</param>
        /// <param name="msg">消息内容</param>
        /// <param name="font">字体ID？</param>
        /// <returns></returns>
        [DllExport("_eventGroupMsg", CallingConvention.StdCall)]
        internal static async System.Threading.Tasks.Task<CoolQApi.Event> ProcessGroupMessageAsync(int subType, int sendTime, long fromGroup,
            long fromQQ, string fromAnonymous, string msg, int font)
        {
            if (fromQQ == 80000000 || fromAnonymous != string.Empty) return CoolQApi.Event.Ignore; // 发送人为匿名

            var cmd = new List<string>(msg.Split(' '));
            string reply = string.Empty;
            // 非追番命令
            if (cmd[0] != COMMAND_NAME1 && cmd[0] != COMMAND_NAME2)
                return CoolQApi.Event.Ignore;
            // 没有子命令
            if (cmd.Count < 2)
                cmd.Add("help");
            int bid = cmd.Count >= 3 ? int.TryParse(cmd[2], out int tmp) ? tmp : 0 : -1;
            switch (cmd[1].ToLower())
            {
                case "订阅":
                case "add":
                case "sub":
                    switch (bid)
                    {
                        case -1:
                            reply = NO_BANGUMI_ID;
                            break;
                        case 0:
                            reply = BANGUMI_ID_NOT_NUMBER;
                            break;
                        default:
                            if (await db.Table<Subscriber>().Where(x => x.QQID == fromQQ).Where(x => x.GroupID == x.GroupID).Where(x => x.BangumiID == bid).CountAsync() != 0)
                                reply = "你已经订阅过这部番了！";
                            else
                            {
                                await db.InsertAsync(new Subscriber() { BangumiID = bid, GroupID = fromGroup, QQID = fromQQ });
                                reply = $"订阅 {bid} 成功！";
                            }
                            break;
                    }
                    break;
                case "取消订阅":
                case "remove":
                case "unsub":
                    switch (bid)
                    {
                        case -1:
                            reply = NO_BANGUMI_ID;
                            break;
                        case 0:
                            reply = BANGUMI_ID_NOT_NUMBER;
                            break;
                        default:
                            Subscriber sub = await db.Table<Subscriber>().Where(x => x.QQID == fromQQ).Where(x => x.GroupID == x.GroupID).Where(x => x.BangumiID == bid).FirstOrDefaultAsync();
                            if (sub != null)
                            {
                                await db.DeleteAsync(sub);
                                reply = "取消订阅成功！";
                            }
                            else
                                reply = "你本来就没订阅这部番！";
                            break;
                    }
                    break;
                case "我的订阅":
                case "list":
                    var list = await db.Table<Subscriber>().Where(x => x.QQID == fromQQ).Where(x => x.GroupID == fromGroup).ToListAsync();
                    reply = $"你在此群订阅了 {list.Count} 部番";
                    list.ForEach(x => reply += "\n" + x.BangumiID);
                    break;
                default:
                case "帮助":
                case "help":
                    reply = helpmsg;
                    break;
                case "debug.task":
                    if (fromQQ != MASTER_QQ)
                    { reply = "没有权限"; }
                    else
                    {
                        reply = "[调试信息]当前排队任务：";
                        JobManager.AllSchedules.ToList().ForEach(x => reply += "\n[" + x.Name + "]下次运行" + x.NextRun.ToLocalTime().ToString());
                    }
                    break;
            }
            if (reply != string.Empty)
                CoolQApi.SendGroupMsg(fromGroup, CoolQApi.CQC_At(fromQQ) + "\n" + reply);
            return CoolQApi.Event.Block;
        }
        private const string helpmsg = "追番系统\n\n主命令 " + COMMAND_NAME1 + " 或 " + COMMAND_NAME2 +
            "\n\n订阅/add/sub 番剧ID\n  订阅番剧\n取消订阅/remove/unsub 番剧ID\n  取消订阅番剧\n" +
            "我的订阅/list\n  列出已经订阅的番剧\n帮助/help\n  获取此帮助信息";

        /// <summary>
        /// 成员退群
        /// </summary>
        /// <param name="subType">1/群员离开 2/群员被踢 3/自己(即登录号)被踢</param>
        /// <param name="sendTime">发送时间</param>
        /// <param name="fromGroup">群号</param>
        /// <param name="fromQQ">操作者QQ(仅subType为2、3时存在)</param>
        /// <param name="target">被操作QQ</param>
        /// <returns></returns>
        [DllExport("_eventMemberQuit", CallingConvention.StdCall)]
        internal static CoolQApi.Event ProcessMemberQuit(int subType, int sendTime, long fromGroup, long fromQQ, long target)
        {
            return CoolQApi.Event.Ignore;
        }


    }
}
