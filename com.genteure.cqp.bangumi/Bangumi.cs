using FluentScheduler;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.genteure.cqp.bangumi
{
    public static class Bangumi
    {
        public const string AppID = "com.genteure.cqp.bangumi";
        public const int MasterQQ = 244827448;

        private static readonly SQLiteAsyncConnection db;
        private static readonly Registry registry;

        static Bangumi()
        {
            registry = new Registry();

        }
    }
}
