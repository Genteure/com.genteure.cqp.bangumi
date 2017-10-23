using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.genteure.cqp.bangumi
{
    public static class Bangumi
    {
        private static Registry registry;

        static Bangumi()
        {
            registry = new Registry();

        }
    }
}
