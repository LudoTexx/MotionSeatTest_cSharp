using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DeLib;

namespace MotionSeatTest_cSharp.Manage
{
    public static class Gpio
    {
        static uint deditecHandle;
        public async static Task SetUpDeditec(CancellationToken ct)
        {
            try
            {
                DeLibNET.DAPI_OPENMODULEEX_STRUCT deditecStruct = new DeLibNET.DAPI_OPENMODULEEX_STRUCT()
                {
                    address = GlobalVariables.IpDeditec,
                    portno = GlobalVariables.PortDeditec,
                    timeout = GlobalVariables.TimeOut,
                    encryption_type = 0
                };


                deditecHandle = DeLibNET.DapiOpenModuleEx((uint)DeLibNET.ModuleID.NET_ETH_LC, 0, deditecStruct, 0);

                DeLibNET.DapiSpecialCommand(deditecHandle, DeLibNET.DAPI_SPECIAL_CMD_TIMEOUT, DeLibNET.DAPI_SPECIAL_TIMEOUT_SET_VALUE_SEC, 2, 500);
                DeLibNET.DapiSpecialCommand(deditecHandle, DeLibNET.DAPI_SPECIAL_CMD_TIMEOUT, DeLibNET.DAPI_SPECIAL_TIMEOUT_ACTIVATE_AUTO_REACTIVATE, 0, 5);

                DeLibNET.DapiSpecialCommand(deditecHandle, DeLibNET.DAPI_SPECIAL_CMD_PWM, DeLibNET.DAPI_SPECIAL_PWM_FREQ_SET, 0, DeLibNET.DAPI_PWM_FREQUENCY_100HZ);

                while(!ct.IsCancellationRequested)
                {
                    try
                    {
                        int[] tmpValues = smoothSignals((int)DeLibNET.DapiADGetVolt(deditecHandle, 0), (int)DeLibNET.DapiADGetVolt(deditecHandle, 1));
                        GlobalVariables.Pitch = tmpValues[0];
                        GlobalVariables.Roll = tmpValues[1];


                        await Task.Delay(100, ct);
                    }
                    catch(OperationCanceledException)
                    {
                        throw;
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"Error reading sensor values: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static double alpha = 0.2; // Smoothing factor between 0 and 1 (closer to 0 = smoother)
        private static double filteredValueRoll = 0;
        private static double filteredValuePitch = 0;

        private static int[] smoothSignals(int roll, int pitch)
        {
            try
            {
                filteredValueRoll = alpha * roll + (1 - alpha) * filteredValueRoll;
                filteredValuePitch = alpha * pitch + (1 - alpha) * filteredValuePitch;

                return new int[2] { (int)Math.Round(filteredValueRoll), (int)Math.Round(filteredValuePitch) };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new int[2] { 0, 0 };
            }
        }
    }
}
