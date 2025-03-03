using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotionSeatTest_cSharp
{
    public static class GlobalVariables
    {
        public static float pitch;
        public static float roll;
        public static float yaw;

        public static CancellationTokenSource? cts;
    }
}
