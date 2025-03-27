using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionSeatTest_cSharp
{
    public static class GlobalVariables
    {
        public static float Pitch;
        public static float Roll;
        public static float Yaw;

        public static CancellationTokenSource? Cts;
        public static CancellationTokenSource? CtsDeditec;

        public static readonly string IpDeditec = "111.111.111.111";
        public static readonly UInt16 PortDeditec = 55555;
        public static readonly UInt16 TimeOut = 1000;
    }
}
