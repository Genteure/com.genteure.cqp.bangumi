using FluentScheduler;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace com.genteure.cqp.bangumi
{
    internal static class Main
    {
        public const string AppID = "com.genteure.cqp.bangumi";
        public const int MasterQQ = 244827448;

        private static SQLiteAsyncConnection db;


        /// <summary>
        /// 启动插件初始化
        /// </summary>
        /// <returns></returns>
        [DllExport("_eventStartup", CallingConvention.StdCall)]
        internal static CoolQApi.Event Startup()
        {
            JobManager.UseUtcTime();
            JobManager.Initialize(new MyRegistry());
            CoolQApi.AddLog(CoolQApi.LogLevel.Debug, "任务", "初始化");
            // db = new SQLiteAsyncConnection("TODO");
            // TODO: 初始化计时器系统 刷新番剧数据
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
        internal static CoolQApi.Event ProcessGroupMessage(int subType, int sendTime, long fromGroup,
            long fromQQ, string fromAnonymous, string msg, int font)
        {
            if (fromQQ == 80000000) return CoolQApi.Event.Ignore; // 发送人为匿名

            return CoolQApi.Event.Ignore;
        }

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
