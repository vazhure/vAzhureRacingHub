using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using vAzhureRacingAPI;

// OpenXR Motion Compensation guides:
// https://www.simracingstudio.com/forum/motion-profiles-1/srs-openxr-motion-compensation-guide
// https://manual.simhubdash.com/motion-addon/openxr-motion-compensation#cor-cor-cor

namespace MotionPlatform3
{
    public class MotionRigPose
    {
        public static string IniFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OpenXR-MotionCompensation", "OpenXR-MotionCompensation.ini");

        public static readonly string cSharedMemLocalMotionRig = @"Local\motionRigPose";
        /// <summary>
        /// OpenVR rig pose map name
        /// </summary>
        public static readonly string cSharedSHOVRMotionRigPose = @"Local\SHOVRMotionRigPose";
        /// <summary>
        /// OpenXR Motion Compensation map name
        /// </summary>
        public static readonly string cSharedMemCOR = @"Local\OXRMC_Telemetry";
        /// <summary>
        /// OpenXR Motion Compensation Status map name
        /// </summary>
        public static readonly string cSharedOXRMC_Status = @"Local\OXRMC_Status";
        /// <summary>
        /// OpenVR Motion Compensation Status memory map file
        /// </summary>
        public static readonly string cSharedOVRMC_MMFv1 = @"Local\OVRMC_MMFv1";

        public static readonly int cMemSize = Marshal.SizeOf(typeof(MMFstruct_Mover_v1));

        private readonly MemoryMappedFile memMotionRig;

        private readonly PosFilter pitchFilter;
        private readonly PosFilter rollFilter;
        private readonly PosFilter heaveFilter;

        /// <summary>
        /// MotionRigPose class constructor
        /// </summary>
        /// <param name="pitchRate">Degree per second</param>
        /// <param name="rollRate">Degree per second</param>
        /// <param name="heaveRate">Millimeter per second</param>
        public MotionRigPose(double pitchRate = 30, double rollRate = 30, double heaveRate = 100)
        {
            pitchFilter = new PosFilter(0, pitchRate);
            rollFilter = new PosFilter(0, rollRate);
            heaveFilter = new PosFilter(0, heaveRate);

            try
            {
                memMotionRig = MemoryMappedFile.CreateNew(cSharedMemLocalMotionRig, cMemSize, MemoryMappedFileAccess.ReadWrite);
            }
            catch
            {
                memMotionRig = null;
            }
        }

        /// <summary>
        /// Check OpenXR Compensation configuration file
        /// </summary>
        public static bool IsOpenXRCompensationConfigured
        {
            get
            {
                if (File.Exists(IniFilePath))
                {
                    try
                    {
                        IniFile iniFile = new IniFile(IniFilePath);
                        if (iniFile.KeyExists("startup", "physical_enabled"))
                        {
                            if (iniFile.ReadINI("startup", "physical_enabled") != "0")
                                return false;
                        }
                        else return false;

                        if (iniFile.KeyExists("tracker", "type"))
                        {
                            if (iniFile.ReadINI("tracker", "type") != "flypt")
                                return false;
                        }
                        else return false;

                        if (iniFile.KeyExists("translation_filter", "strength"))
                        {
                            if (iniFile.ReadINI("translation_filter", "strength") != "0")
                                return false;
                        }
                        else return false;


                        if (iniFile.KeyExists("rotation_filter", "strength"))
                        {
                            if (iniFile.ReadINI("rotation_filter", "strength") != "0")
                                return false;
                        }
                        else return false;

                        return true;
                    }
                    catch
                    {

                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Path OpenXR Configuration file
        /// </summary>
        /// <returns></returns>
        public static bool PatchOpenXRConfig()
        {
            if (File.Exists(IniFilePath))
            {
                try
                {
                    // Make a backup
                    File.Copy(IniFilePath, IniFilePath + ".bak");
                }
                catch { }
                
                try
                {
                    IniFile iniFile = new IniFile(IniFilePath);
                    iniFile.Write("startup", "physical_enabled", "0");
                    iniFile.Write("tracker", "type", "flypt");
                    iniFile.Write("translation_filter", "strength", "0");
                    iniFile.Write("rotation_filter", "strength", "0");

                    return IsOpenXRCompensationConfigured;
                }
                catch { }
            }

            return false;
        }

        /// <summary>
        /// Is OpenXR available
        /// </summary>
        public static bool IsOpenXR
        {
            get
            {
                try
                {
                    using (MemoryMappedFile.OpenExisting(cSharedOXRMC_Status, MemoryMappedFileRights.Read))
                    {
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Is OpenVR available
        /// </summary>
        public static bool IsOpenVR
        {
            get
            {
                try
                {
                    using (MemoryMappedFile.OpenExisting(cSharedOVRMC_MMFv1, MemoryMappedFileRights.Read))
                    {
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        public void SetPitchRate(double rate) => pitchFilter.SetSpeed(rate);
        public void SetRollRate(double rate) => rollFilter.SetSpeed(rate);
        public void SetHeaveRate(double rate) => heaveFilter.SetSpeed(rate);

        /// <summary>
        /// data buffer
        /// </summary>
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

    /// <summary>
    /// Position filter
    /// </summary>
    public class PosFilter
    {
        double _position = 0;
        double _target = 0;
        private double _speed;
        DateTime _prevTime = DateTime.MinValue;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="initialTarget">initial target</param>
        /// <param name="speed">Max value change Units per second </param>
        public PosFilter(double initialTarget, double speed)
        {
            Initialize(initialTarget, speed);
        }

        public void Initialize(double initialTarget, double speed)
        {
            _position = _target = initialTarget;
            _speed = speed;
            _prevTime = DateTime.MinValue;
        }

        /// <summary>
        /// Set speed
        /// </summary>
        /// <param name="speed">Units per second</param>
        /// <exception cref="ArgumentOutOfRangeException">Speed should be more than zero</exception>
        public void SetSpeed(double speed)
        {
            if (speed <= 0)
                throw new ArgumentOutOfRangeException(nameof(speed));

            _speed = speed;
        }

        /// <summary>
        /// Update target position
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public double UpdatePosition(double target)
        {
            TimeSpan ts = DateTime.Now - _prevTime;
            _prevTime = DateTime.Now;

            if (Math.Abs(_target - _position) < double.Epsilon)
            {
                _target = target;
                return _position;
            }
            else
            {
                double delta = (_target - _position);

                double elapsed = ts.TotalSeconds;
                double maxTime = Math.Abs(delta) / _speed;

                maxTime = Math.Max(maxTime, elapsed);

                if (maxTime < elapsed)
                    _position = _target;
                else
                    _position += delta * (elapsed / maxTime);

                _target = target;

                return _position;
            }
        }
#if DEBUG
        public void Test(string filename)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("#;target;pos;");
            for (int i = 0; i < 100; i++)
            {
                double target = Math.Sin(i * 0.5) * 10;
                double pos = UpdatePosition(target);
                stringBuilder.AppendLine($"{i};{target};{pos};");
                Thread.Sleep(50);
            }
            System.IO.File.WriteAllText(filename, stringBuilder.ToString());
        }
#endif
    }
}