﻿using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace com.genteure.cqp.bangumi
{
    internal static class CoolQApi
    {
        const string AppID = "com.genteure.cqp.bangumi";
        private static int ac;



        private static class NativeMethods
        {
            [DllExport("AppInfo")]
            public static string AppInfo() => "9," + AppID;

            [DllExport("Initialize", CallingConvention.StdCall)]
            public static int Initialize(int i) => ac = i;


            [DllImport("CQP.DLL")]
            public static extern int CQ_sendPrivateMsg(int AuthCode, long QQID, string Message);

            [DllImport("CQP.DLL")]
            public static extern int CQ_sendGroupMsg(int AuthCode, long GroupID, string Message);

            [DllImport("CQP.DLL")]
            public static extern int CQ_sendDiscussMsg(int AuthCode, long DiscussID, string Message);

            [DllImport("CQP.DLL")]
            public static extern int CQ_sendLike(int AuthCode, long QQID);

            [DllImport("CQP.DLL")]
            public static extern int CQ_setGroupKick(int AuthCode, long GroupID, long QQID, bool NeverAllowAgain = false);

            [DllImport("CQP.DLL")]
            public static extern int CQ_setGroupBan(int AuthCode, long GroupID, long QQID, long Seconds);

            [DllImport("CQP.DLL")]
            public static extern int CQ_setGroupAdmin(int AuthCode, long GroupID, long QQID, bool isAdmin);

            [DllImport("CQP.DLL")]
            public static extern int CQ_setGroupWholeBan(int AuthCode, long GroupID, bool isBan);

            [DllImport("CQP.DLL")]
            public static extern int CQ_setGroupAnonymousBan(int AuthCode, long GroupID, string AnomymousID, long Seconds);

            [DllImport("CQP.DLL")]
            public static extern int CQ_setGroupAnonymous(int AuthCode, long GroupID, bool isEnable);

            [DllImport("CQP.DLL")]
            public static extern int CQ_setGroupCard(int AuthCode, long GroupID, long QQID, string NewName);

            [DllImport("CQP.DLL")]
            public static extern int CQ_setGroupLeave(int AuthCode, long GroupID, bool isDisband);

            [DllImport("CQP.DLL")]
            public static extern int CQ_setGroupSpecialTitle(int AuthCode, long GroupID, long QQID, string Title, long Seconds);

            [DllImport("CQP.DLL")]
            public static extern int CQ_setDiscussLeave(int AuthCode, long DiscussID);

            [DllImport("CQP.DLL")]
            public static extern int CQ_setFriendAddRequest(int AuthCode, string ResponseFlag, Request Operation, string Remark);

            [DllImport("CQP.DLL")]
            public static extern int CQ_setGroupAddRequestV2(int AuthCode, string ResponseFlag, Request Type, Request Operation, string Reason);

            [DllImport("CQP.DLL")]
            public static extern string CQ_getGroupMemberInfoV2(int AuthCode, long GroupID, long QQID, bool NoCache);

            [DllImport("CQP.DLL")]
            public static extern string CQ_getStrangerInfo(int AuthCode, long QQID, bool NoCache);

            [DllImport("CQP.DLL")]
            public static extern int CQ_addLog(int AuthCode, CQLog Priority, string Category, string Content);

            [DllImport("CQP.DLL")]
            public static extern string CQ_getCookies(int AuthCode);

            [DllImport("CQP.DLL")]
            public static extern int CQ_getCsrfToken(int AuthCode);

            [DllImport("CQP.DLL")]
            public static extern int CQ_getLoginQQ(int AuthCode);

            [DllImport("CQP.DLL")]
            public static extern string CQ_getLoginNick(int AuthCode);

            [DllImport("CQP.DLL")]
            public static extern string CQ_getAppDirectory(int AuthCode);

            [DllImport("CQP.DLL")]
            public static extern int CQ_setFatal(int AuthCode, string ErrorInfo);

            [DllImport("CQP.DLL")]
            public static extern string CQ_getRecord(int AuthCode, string File, string Format);

            // [DllImport("CQP.DLL")]
            // public static extern int CQ______________(int AuthCode, long someshit);

        }

        /// <summary>
        /// 语音消息音频格式
        /// </summary>
        internal struct RecordFormat
        {
            const string MP3 = "mp3";
            const string AMR = "amr";
            const string WMA = "wma";
            const string M4A = "m4a";
            const string SPX = "spx";
            const string OGG = "ogg";
            const string WAV = "wav";
            const string FLAC = "flac";
        }

        /// <summary>
        /// 事件
        /// </summary>
        internal enum Event : Int32
        {
            /// <summary>
            /// 忽略
            /// </summary>
            Ignore = 0,
            /// <summary>
            /// 截拦
            /// </summary>
            Block = 1
        }

        /// <summary>
        /// 请求
        /// </summary>
        internal enum Request : Int32
        {
            /// <summary>
            /// 通过
            /// </summary>
            Allow = 1,
            /// <summary>
            /// 拒绝
            /// </summary>
            Deny = 2,
            /// <summary>
            /// 群添加
            /// </summary>
            GroupAdd = 1,
            /// <summary>
            /// 群邀请
            /// </summary>
            GourpInvite = 2
        }

        /// <summary>
        /// 酷Q日志记录等级
        /// </summary>
        internal enum CQLog : Int32
        {
            /// <summary>
            /// 调试 灰色
            /// </summary>
            Debug = 0,
            /// <summary>
            /// 信息 黑色
            /// </summary>
            Info = 10,
            /// <summary>
            /// 信息(成功) 紫色
            /// </summary>
            InfoSuccess = 11,
            /// <summary>
            /// 信息(接收) 蓝色
            /// </summary>
            InfoRecv = 12,
            /// <summary>
            /// 信息(发送) 绿色
            /// </summary>
            InfoSend = 13,
            /// <summary>
            /// 警告 橙色
            /// </summary>
            Warning = 20,
            /// <summary>
            /// 错误 红色
            /// </summary>
            Error = 30,
            /// <summary>
            /// 致命错误 深红
            /// </summary>
            Fatal = 40
        }


        private class Unpack
        {
            private readonly byte[] _source;
            private int _location = 0;
            public Unpack(byte[] source) => _source = source;
            public byte[] GetAll() => _source.SubArray(_location, _source.Length - _location);
            public int Len() => _source.Length - _location;
            public byte[] GetBin(int len) { if (len <= 0) return null; _location += len; return _source.SubArray(_location, len); }
            public byte GetByte() { _location += 1; return (byte)_source.SubArray(_location, 1).GetValue(0); }
            public int GetInt() { _location += 4; return _source.SubArray(_location, 4).ToInt(); }
            public long GetLong() { _location += 8; return _source.SubArray(_location, 8).ToLong(); }
            public short GetShort() { _location += 2; return _source.SubArray(_location, 2).ToShort(); }
            public string GetLenStr() { try { return Encoding.GetEncoding("GB2312").GetString(GetBin(GetShort())); } catch { return ""; } }
            public byte[] GetToken() { return GetBin(GetShort()); }
        }

        private static bool ConvertAnsiHexToGroupMemberInfo(byte[] source, ref GroupMemberInfo gm)
        {
            if (source == null || source.Length < 40)
                return false;
            var u = new Unpack(source);
            gm.GroupId = u.GetLong();
            gm.Number = u.GetLong();
            gm.NickName = u.GetLenStr();
            gm.InGroupName = u.GetLenStr();
            gm.Gender = u.GetInt() == 0 ? "男" : " 女";
            gm.Age = u.GetInt();
            gm.Area = u.GetLenStr();
            gm.JoinTime = new DateTime(1970, 1, 1, 0, 0, 0).ToLocalTime().AddSeconds(u.GetInt());
            gm.LastSpeakingTime = new DateTime(1970, 1, 1, 0, 0, 0).ToLocalTime().AddSeconds(u.GetInt());
            gm.Level = u.GetLenStr();
            var manager = u.GetInt();
            gm.Authority = manager == 3 ? "群主" : (manager == 2 ? "管理员" : "成员");
            gm.HasBadRecord = (u.GetInt() == 1);
            gm.Title = u.GetLenStr();
            gm.TitleExpirationTime = u.GetInt();
            gm.CanModifyInGroupName = (u.GetInt() == 1);
            return true;
        }

        /// <summary>
        /// 表示一个群成员的信息。
        /// </summary>
        public sealed class GroupMemberInfo
        {
            /// <summary>
            /// 此群成员在其个人资料上所填写的年龄。
            /// </summary>
            /// <returns></returns>
            public int Age { get; set; }

            /// <summary>
            /// 此群成员在其个人资料上所填写的区域。
            /// </summary>
            /// <returns></returns>
            public string Area { get; set; }

            /// <summary>
            /// 此群成员的身份。
            /// </summary>
            /// <returns></returns>
            public string Authority { get; set; }

            /// <summary>
            /// 指示此群成员是否能够修改所有群成员名片的值。
            /// </summary>
            /// <returns></returns>
            public bool CanModifyInGroupName { get; set; }

            /// <summary>
            /// 此群成员在其个人资料上所填写的性别。
            /// </summary>
            /// <returns></returns>
            public string Gender { get; set; }

            /// <summary>
            /// 此群成员的群名片。
            /// </summary>
            /// <returns></returns>
            public string InGroupName { get; set; }

            /// <summary>
            /// 此群成员的头衔。
            /// </summary>
            /// <returns></returns>
            public string Title { get; set; }

            /// <summary>
            /// 此群成员所在群号。
            /// </summary>
            /// <returns></returns>
            public long GroupId { get; set; }

            /// <summary>
            /// 指示此群成员是否有不良记录的值。
            /// </summary>
            /// <returns></returns>
            public bool HasBadRecord { get; set; }

            /// <summary>
            /// 头衔过期时间。
            /// </summary>
            /// <returns></returns>
            public int TitleExpirationTime { get; set; }

            /// <summary>
            /// 此群成员的入群时间。
            /// </summary>
            /// <returns></returns>
            public DateTime JoinTime { get; set; }

            /// <summary>
            /// 此群成员最后发言日期。
            /// </summary>
            /// <returns></returns>
            public DateTime LastSpeakingTime { get; set; }

            /// <summary>
            /// 此群成员的群内等级。
            /// </summary>
            /// <returns></returns>
            public string Level { get; set; }

            /// <summary>
            /// 此群成员的昵称。
            /// </summary>
            /// <returns></returns>
            public string NickName { get; set; }

            /// <summary>
            /// 此群成员的QQ号码。
            /// </summary>
            /// <returns></returns>
            public long Number { get; set; }
        }

        // -------------
    }

    internal static class DataConvertExtensions
    {
        public static long ToLong(this byte[] bytes) { Array.Reverse(bytes); return BitConverter.ToInt64(bytes, 0); }
        public static int ToInt(this byte[] bytes) { Array.Reverse(bytes); return BitConverter.ToInt32(bytes, 0); }
        public static short ToShort(this byte[] bytes) { Array.Reverse(bytes); return BitConverter.ToInt16(bytes, 0); }

        /// <summary>
        /// 从此实例检索子数组
        /// </summary>
        /// <param name="source">要检索的数组</param>
        /// <param name="startIndex">起始索引号</param>
        /// <param name="length">检索最大长度</param>
        /// <returns>与此实例中在 startIndex 处开头、长度为 length 的子数组等效的一个数组</returns>
        public static byte[] SubArray(this byte[] source, int startIndex, int length)
            => (startIndex < 0 || startIndex > source.Length || length < 0) ?
                throw new ArgumentOutOfRangeException(nameof(startIndex)) :
                startIndex + length <= source.Length ?
                    source.Skip(startIndex).Take(length).ToArray() :
                    source.Skip(startIndex).ToArray();
    }

}
