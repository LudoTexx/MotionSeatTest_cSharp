using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;


namespace MotionSeatTest_cSharp.SimRacingStudio
{
    static class SimRacingStudioAPI
    {
        static readonly string SRS_COMPUTER_IP = "localhost";  //to broadcast to all computers in the network use ""
        static readonly int SRS_COMPUTER_PORT = 33001;  //this port can be changed on the config.ini of srs app
        static readonly char[] API_MODE = "api".ToCharArray();  //constant to identify the package
        static readonly uint API_VERSION = 102;  //constant of the current version of the api

        static char[] game = "Project Cars 2".PadRight(50).ToCharArray();  //constant to identify the package
        static char[] vehicle = "Lamborghini Huracan".PadRight(50).ToCharArray();  //constant to identify the package
        static char[] location = "Circuit Gilles-Villeneuve".PadRight(50).ToCharArray();  //constant to identify the package



        static float maxRpm = 8000;
        static double speed = 0;
        static float rpm = 0;
        static int gear = 0;

        static float lateralVelocity = 0;
        static float lateralAcceleration = 0;
        static float verticalAcceleration = 0;
        static float longitudinalAcceleration = 0;
        static float suspensionTravelFrontLeft = 0;
        static float suspensionTravelFrontRight = 0;
        static float suspensionTravelRearLeft = 0;
        static float suspensionTravelRearRight = 0;
        static uint wheelTerrainFrontLeft = 0;
        static uint wheelTerrainFrontRight = 0;
        static uint wheelTerrainRearLeft = 0;
        static uint wheelTerrainRearRight = 0;


        static double speedIncrement = 0.2;

        public static async Task StartMotionSystem(CancellationToken ct)
        {
            try
            {
                using var udpClient = new UdpClient();

                while(!ct.IsCancellationRequested)
                {
                    if (speed > 200)
                    {
                        speed = 200;
                        speedIncrement = -0.2;
                    }

                    // when speed hits 0, starts acceleration again
                    if (speed < 0)
                    {
                        speed = 0;
                        speedIncrement = 0.2;
                    }

                    // reset the rpm
                    if (rpm > maxRpm) rpm = 0;

                    // reset gear to 0
                    if (gear > 9) gear = 0;

                    telemetryPacket tp = new telemetryPacket(API_MODE,
                                                             API_VERSION,
                                                             game,
                                                             vehicle,
                                                             location,
                                                             (float)speed,
                                                             rpm,
                                                             maxRpm,
                                                             gear,
                                                             GlobalVariables.pitch,
                                                             GlobalVariables.roll,
                                                             GlobalVariables.yaw,
                                                             lateralVelocity,
                                                             lateralAcceleration,
                                                             verticalAcceleration,
                                                             longitudinalAcceleration,
                                                             suspensionTravelFrontLeft,
                                                             suspensionTravelFrontRight,
                                                             suspensionTravelRearLeft,
                                                             suspensionTravelRearRight,
                                                             wheelTerrainFrontLeft,
                                                             wheelTerrainFrontRight,
                                                             wheelTerrainRearLeft,
                                                             wheelTerrainRearRight);

                    int size = Marshal.SizeOf(tp);
                    byte[] packet = new byte[size];

                    IntPtr ptr = Marshal.AllocHGlobal(size);

                    try
                    {
                        Marshal.StructureToPtr(tp, ptr, true);
                        Marshal.Copy(ptr, packet, 0, size);
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(ptr);
                    }

                    Debug.WriteLine("sending... " + tp.apiMode + " " + tp.version + " " + tp.rpm + " " + tp.speed);
                    udpClient.Send(packet, packet.Length, SRS_COMPUTER_IP, SRS_COMPUTER_PORT);

                    // increment variables
                    gear = gear + 1;
                    rpm = rpm + 50;
                    speed = speed + speedIncrement;

                    await Task.Delay(100, ct);
                }
            }
            catch (Exception ex) 
            {
                Debug.Write(ex);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct telemetryPacket
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public char[] apiMode;
        public uint version;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public char[] game;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public char[] vehicleName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public char[] location;
        public float speed;
        public float rpm;
        public float maxRpm;
        public int gear;
        public float pitch;
        public float roll;
        public float yaw;
        public float lateralVelocity;
        public float lateralAcceleration;
        public float verticalAcceleration;
        public float longitudinalAcceleration;
        public float suspensionTravelFrontLeft;
        public float suspensionTravelFrontRight;
        public float suspensionTravelRearLeft;
        public float suspensionTravelRearRight;
        public uint wheelTerrainFrontLeft;
        public uint wheelTerrainFrontRight;
        public uint wheelTerrainRearLeft;
        public uint wheelTerrainRearRight;

        public telemetryPacket(char[] papiMode, uint pversion, char[] pgame, char[] pvehicleName, char[] plocation, float pspeed, float prpm, float pmaxRpm, int pgear, float ppitch, float proll, float pyaw, float plateralVelocity, float plateralAcceleration, float pverticalAcceleration, float plongitudinalAcceleration, float psuspensionTravelFrontLeft, float psuspensionTravelFrontRight, float psuspensionTravelRearLeft, float psuspensionTravelRearRight, uint pwheelTerrainFrontLeft, uint pwheelTerrainFrontRight, uint pwheelTerrainRearLeft, uint pwheelTerrainRearRight)
        {
            apiMode = papiMode;
            version = pversion;
            game = pgame;
            vehicleName = pvehicleName;
            location = plocation;
            speed = pspeed;
            rpm = prpm;
            maxRpm = pmaxRpm;
            gear = pgear;
            pitch = ppitch;
            roll = proll;
            yaw = pyaw;
            lateralVelocity = plateralVelocity;
            lateralAcceleration = plateralAcceleration;
            verticalAcceleration = pverticalAcceleration;
            longitudinalAcceleration = plongitudinalAcceleration;
            suspensionTravelFrontLeft = psuspensionTravelFrontLeft;
            suspensionTravelFrontRight = psuspensionTravelFrontRight;
            suspensionTravelRearLeft = psuspensionTravelRearLeft;
            suspensionTravelRearRight = psuspensionTravelRearRight;
            wheelTerrainFrontLeft = pwheelTerrainFrontLeft;
            wheelTerrainFrontRight = pwheelTerrainFrontRight;
            wheelTerrainRearLeft = pwheelTerrainRearLeft;
            wheelTerrainRearRight = pwheelTerrainRearRight;
        }
    }
}
