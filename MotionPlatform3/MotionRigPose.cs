using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using vAzhureRacingAPI;

namespace MotionPlatform3
{
 
    public class MotionRigPose
    {
        // public static readonly string cSharedMemMotionRigPose = @"Local\motionRigPose";
        public static readonly string cSharedMemCOR = @"Local\OXRMC_Telemetry";

        public static readonly int cMemSize = Marshal.SizeOf(typeof(MMFstruct_Mover_v1));

        private readonly MemoryMappedFile memMotionRig;
        public MotionRigPose()
        {
            try
            {
                memMotionRig = MemoryMappedFile.CreateNew(cSharedMemCOR, cMemSize, MemoryMappedFileAccess.ReadWrite);
            }
            catch
            {
                memMotionRig = null;
            }
        }

        byte[] data = new byte[cMemSize];
        
        public bool SetPose(double pitch, double roll, double heaveMM)
        {
            try
            {
                using (var viewPluginInfo = memMotionRig.CreateViewStream(0L, cMemSize, MemoryMappedFileAccess.ReadWrite))
                {
                    MMFstruct_Mover_v1 rigpos = new MMFstruct_Mover_v1() { rigPitch = pitch * 180.0 / Math.PI, rigRoll = roll * 180.0 / Math.PI, rigHeave = heaveMM };
                    Marshalizable<MMFstruct_Mover_v1>.ToBytes(rigpos, ref data);
                    viewPluginInfo.WriteAsync(data, 0, data.Length);
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        ~MotionRigPose()
        {
            memMotionRig?.Dispose();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MMFstruct_Mover_v1
    {
        public double rigSway;
        public double rigSurge;
        public double rigHeave;
        public double rigYaw;
        public double rigRoll;
        public double rigPitch;
    }
}