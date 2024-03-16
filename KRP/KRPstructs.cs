using System;
using System.Runtime.InteropServices;

namespace KartRacingPro
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct SPluginsKartEvent
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string m_szDriverName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string m_szKartID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string m_szKartName;
        /// <summary>
        /// 0 = direct; 1 = clutch; 2 = shifter
        /// </summary>
        public int m_iDriveType;
        public int m_iNumberOfGears;
        public int m_iMaxRPM;
        public int m_iLimiter;
        public int m_iShiftRPM;
        /// <summary>
        /// 0 = aircooled; 1 = watercooled
        /// </summary>
        public int m_iEngineCooling;
        /// <summary>
        /// degrees Celsius
        /// </summary>
        public float m_fEngineOptTemperature;
        /// <summary>
        /// Degrees Celsius. Lower and upper limits
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] m_afEngineTemperatureAlarm;
        /// <summary>
        /// liters
        /// </summary>
        public float m_fMaxFuel;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string m_szCategory;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string m_szDash;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string m_szTrackID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string m_szTrackName;
        /// <summary>
        /// centerline length. meters
        /// </summary>
        public float m_fTrackLength;
        /// <summary>
        /// 1 = testing; 2 = race; 4 = challenge
        /// </summary>
        public int m_iType;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct SPluginsKartSession
    {
        /// <summary>
        /// testing: always 0. Race: 1 = practice; 2 = qualify; 3 = warmup; 4 = qualify heat; 5 = second chance heat; 6 = prefinal; 7 = final. Challenge: 0 = waiting; 1 = practice; 2 = race
        /// </summary>
        public int m_iSession;
        public int m_iSessionSeries;
        /// <summary>
        /// 0 = sunny; 1 = cloudy; 2 = rainy
        /// </summary>
        public int m_iConditions;
        /// <summary>
        /// degrees Celsius
        /// </summary>
        public float m_fAirTemperature;
        /// <summary>
        /// degrees Celsius
        /// </summary>
        public float m_fTrackTemperature;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string m_szSetupFileName;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct SPluginsKartData
    {
        /// <summary>
        /// engine rpm 
        /// </summary>
        public int m_iRPM;
        /// <summary>
        /// degrees Celsius
        /// </summary>
        public float m_fCylinderHeadTemperature;
        /// <summary>
        /// degrees Celsius
        /// </summary>
        public float m_fWaterTemperature;
        /// <summary>
        /// 0 = Neutral
        /// </summary>
        public int m_iGear;
        /// <summary>
        /// liters
        /// </summary>
        public float m_fFuel;
        /// <summary>
        /// meters/second
        /// </summary>
        public float m_fSpeedometer;
        /// <summary>
        /// world position of a reference point attached to chassis ( not CG )
        /// </summary>
        public float m_fPosX, m_fPosY, m_fPosZ;
        /// <summary>
        /// velocity of CG in world coordinates. meters/second
        /// </summary>
        public float m_fVelocityX, m_fVelocityY, m_fVelocityZ;
        /// <summary>
        /// acceleration of CG local to chassis rotation, expressed in G ( 9.81 m/s2 ) and averaged over the latest 10ms
        /// </summary>
        public float m_fAccelerationX, m_fAccelerationY, m_fAccelerationZ;
        /// <summary>
        /// rotation matrix of the chassis
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
        public float[] m_aafRot;
        /// <summary>
        /// degrees, -180 to 180
        /// </summary>
        public float m_fYaw, m_fPitch, m_fRoll;
        /// <summary>
        /// degrees / second
        /// </summary>
        public float m_fYawVelocity, m_fPitchVelocity, m_fRollVelocity;
        /// <summary>
        /// degrees. Negative = left
        /// </summary>
        public float m_fInputSteer;
        /// <summary>
        /// 0 to 1
        /// </summary>
        public float m_fInputThrottle;
        /// <summary>
        /// 0 to 1
        /// </summary>
        public float m_fInputBrake;
        /// <summary>
        /// 0 to 1
        /// </summary>
        public float m_fInputFrontBrakes;
        /// <summary>
        /// 0 to 1, 0 = Fully engaged
        /// </summary>
        public float m_fInputClutch;
        /// <summary>
        /// meters/second. 0 = front-left; 1 = front-right; 2 = rear-left; 3 = rear-right
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] m_afWheelSpeed;
        /// <summary>
        /// material index. 0 = not in contact
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public int[] m_aiWheelMaterial;
        /// <summary>
        /// Nm
        /// </summary>
        public float m_fSteerTorque;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct SPluginsKartLap
    {
        /// <summary>
        /// lap index
        /// </summary>
        public int m_iLapNum;
        /// <summary>
        /// 
        /// </summary>
        public int m_iInvalid;
        /// <summary>
        /// milliseconds
        /// </summary>
        public int m_iLapTime;
        /// <summary>
        /// 1 = best lap 
        /// </summary>
        public int m_iPos;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct SPluginsKartSplit
    {
        /// <summary>
        /// split index
        /// </summary>
        public int m_iSplit;
        /// <summary>
        /// milliseconds
        /// </summary>
        public int m_iSplitTime;
        /// <summary>
        /// milliseconds. Difference with best lap
        /// </summary>
        public int m_iBestDiff;
    };

    /******************************************************************************
	structures and functions to draw
	******************************************************************************/

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct SPluginQuad
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public float[] m_aafPos;           /* 0,0 -> top left. 1,1 -> bottom right. counter-clockwise */
        public int m_iSprite;                  /* 1 based index in SpriteName buffer. 0 = fill with m_ulColor */
        public uint m_ulColor;        /* ABGR */
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct SPluginString
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string m_szString;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] m_afPos;               /* 0,0 -> top left. 1,1 -> bottom right */
        public int m_iFont;                    /* 1 based index in FontName buffer */
        public float m_fSize;
        public int m_iJustify;                 /* 0 = left; 1 = center; 2 = right */
        public uint m_ulColor;        /* ABGR */
    }

    /******************************************************************************
	structures to receive the track center line
	******************************************************************************/
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct SPluginsTrackSegment
    {
        public int m_iType;                    /* 0 = straight; 1 = curve */
        public float m_fLength;                /* meters */
        public float m_fRadius;                /* curve radius in meters. < 0 for left curves; 0 for straights */
        public float m_fAngle;                 /* start angle in degrees. 0 = north */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] m_afStart;             /* start position in meters */
        public float m_fHeight;                /* start height in meters */
    }

    public enum RaceType : int
    {
        Replay  = -1,
        Testing = 1,
        Race = 2,
        Challenge = 4
    }

    /******************************************************************************
	structures to receive the race data
	******************************************************************************/
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SPluginsRaceEvent
    {
        /// <summary>
        /// -1 = replay; 1 = testing; 2 = race; 4 = challenge
        /// </summary>
        public RaceType m_iType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string m_szName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string m_szTrackName;
        /// <summary>
        /// meters
        /// </summary>
        public float m_fTrackLength;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct SPluginsRaceAddEntry
    {
        public int m_iRaceNum;                                     /* unique race number */
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string m_szName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string m_szKartName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string m_szKartShortName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string m_szCategory;
        public int m_iUnactive;                                    /* if set to 1, the driver left the event and the following fields are not set */
        public int m_iNumberOfGears;
        public int m_iMaxRPM;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct SPluginsRaceRemoveEntry
    {
        public int m_iRaceNum;                                     /* race number */
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct SPluginsRaceSession
    {
        /// <summary>
        /// testing: always 0. 
        /// Race: 
        /// 1 = practice; 
        /// 2 = qualify; 
        /// 3 = warmup; 
        /// 4 = qualify heat; 
        /// 5 = second chance heat; 
        /// 6 = prefinal; 
        /// 7 = final. 
        /// Challenge: 
        /// 0 = waiting; 
        /// 1 = practice; 
        /// 2 = race */
        /// </summary>
        public int m_iSession;                                     /* testing: always 0. Race: 1 = practice; 2 = qualify; 3 = warmup; 4 = qualify heat; 5 = second chance heat; 6 = prefinal; 7 = final. Challenge: 0 = waiting; 1 = practice; 2 = race */
        public int m_iSessionSeries;
        public int m_iGroup1, m_iGroup2;                           /* 0 = A, 1 = B, 2 = C, ... Only used for Qualify Heats */
        /// <summary>
        /// testing / waiting: always 0. practice / qualify / warmup: 
        /// 16 = in progress; 
        /// 32 = completed. 
        /// qualify heat / second chance heat / prefinal / final: 
        /// 16 = in progress; 
        /// 32 = semaphore; 
        /// 64 = sighting lap; 
        /// 128 = warmup lap; 
        /// 256 = pre-start; 
        /// 512 = race over; 
        /// 1024 = completed; 
        /// 2048 = rolling start */
        /// </summary>
        public int m_iSessionState;                                
        public int m_iSessionLength;                               /* milliseconds. 0 = no limit */
        public int m_iSessionNumLaps;
        public int m_iNumEntries;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public int[] m_aiEntries;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 50)]
        public int[] m_aiGrid;
        public int m_iConditions;                                  /* 0 = sunny; 1 = cloudy; 2 = rainy */
        public float m_fAirTemperature;                            /* degrees Celsius */
        public float m_fTrackTemperature;                          /* degrees Celsius */
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct SPluginsRaceSessionState
    {
        /// <summary>
        /// testing: always 0. Race: 1 = practice; 2 = qualify; 3 = warmup; 4 = qualify heat; 5 = second chance heat; 6 = prefinal; 7 = final. Challenge: 0 = waiting; 1 = practice; 2 = race
        /// </summary>
        public int m_iSession;                                     /* testing: always 0. Race: 1 = practice; 2 = qualify; 3 = warmup; 4 = qualify heat; 5 = second chance heat; 6 = prefinal; 7 = final. Challenge: 0 = waiting; 1 = practice; 2 = race */
        public int m_iSessionSeries;
        public int m_iSessionState;                                /* testing / waiting: always 0. practice / qualify / warmup: 16 = in progress; 32 = completed. qualify heat / second chance heat / prefinal / final: 16 = in progress; 32 = semaphore; 64 = sighting lap; 128 = warmup lap; 256 = pre-start; 512 = race over; 1024 = completed; 2048 = rolling start */
        public int m_iSessionLength;                               /* milliseconds. 0 = no limit */
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct SPluginsRaceLap
    {
        public int m_iSession;                                     /* testing: always 0. Race: 1 = practice; 2 = qualify; 3 = warmup; 4 = qualify heat; 5 = second chance heat; 6 = prefinal; 7 = final. Challenge: 0 = waiting; 1 = practice; 2 = race */
        public int m_iSessionSeries;
        public int m_iRaceNum;                                     /* race number */
        public int m_iLapNum;                                      /* lap index */
        public int m_iInvalid;
        public int m_iLapTime;                                     /* milliseconds */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] m_aiSplit;                                   /* milliseconds */
        public float m_fSpeed;                                     /* meters/second */
        public int m_iBest;                                        /* 1 = personal best lap; 2 = overall best lap */
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct SPluginsRaceSplit
    {
        public int m_iSession;                                     /* testing: always 0. Race: 1 = practice; 2 = qualify; 3 = warmup; 4 = qualify heat; 5 = second chance heat; 6 = prefinal; 7 = final. Challenge: 0 = waiting; 1 = practice; 2 = race */
        public int m_iSessionSeries;
        public int m_iRaceNum;                                     /* race number */
        public int m_iLapNum;                                      /* lap index */
        public int m_iSplit;                                       /* split index */
        public int m_iSplitTime;                                   /* milliseconds */
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct SPluginsRaceSpeed
    {
        public int m_iSession;                                     /* testing: always 0. Race: 1 = practice; 2 = qualify; 3 = warmup; 4 = qualify heat; 5 = second chance heat; 6 = prefinal; 7 = final. Challenge: 0 = waiting; 1 = practice; 2 = race */
        public int m_iSessionSeries;
        public int m_iRaceNum;                                     /* race number */
        public int m_iLapNum;                                      /* lap index */
        public float m_fSpeed;                                     /* meters/second */
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct SPluginsRaceCommunication
    {
        public int m_iSession;                                     /* testing: always 0. Race: 1 = practice; 2 = qualify; 3 = warmup; 4 = qualify heat; 5 = second chance heat; 6 = prefinal; 7 = final. Challenge: 0 = waiting; 1 = practice; 2 = race */
        public int m_iSessionSeries;
        public int m_iRaceNum;                                     /* race number */
        public int m_iCommunication;                               /* 1 = change state; 2 = penalty */
        public int m_iState;                                       /* 1 = DNS; 2 = retired; 3 = DSQ */
        public int m_iReason;                                      /* Reason for DSQ. 0 = jump start; 1 = too many offences; 3 = rolling start speeding; 4 = rolling start too slow; 5 = rolling start corridor crossing; 6 = rolling start overtaking; 7 = director */
        public int m_iOffence;                                     /* 1 = jump start; 3 = cutting; 4 = rolling start speeding; 5 = rolling start too slow; 6 = rolling start corridor crossing; 7 = rolling start overtaking */
        public int m_iLap;                                         /* lap index */
        public int m_iType;                                        /* 1 = time penalty; 2 = position penalty */
        public int m_iTime;                                        /* milliseconds. Penalty time */
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct SPluginsRaceClassification
    {
        public int m_iSession;                                     /* testing: always 0. Race: 1 = practice; 2 = qualify; 3 = warmup; 4 = qualify heat; 5 = second chance heat; 6 = prefinal; 7 = final. Challenge: 0 = waiting; 1 = practice; 2 = race */
        public int m_iSessionSeries;
        public int m_iSessionState;                                /* testing / waiting: always 0. practice / qualify / warmup: 16 = in progress; 32 = completed. qualify heat / second chance heat / prefinal / final: 16 = in progress; 32 = semaphore; 64 = sighting lap; 128 = warmup lap; 256 = pre-start; 512 = race over; 1024 = completed; 2048 = rolling start */
        public int m_iSessionTime;                                 /* milliseconds. Current session time */
        public int m_iNumEntries;                                  /* number of entries */
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct SPluginsRaceClassificationEntry
    {
        public int m_iRaceNum;                                     /* race number */
        public int m_iState;                                       /* 1 = DNS; 2 = retired; 3 = DSQ */
        public int m_iBestLap;                                     /* milliseconds */
        public float m_fBestSpeed;                                 /* meters/second */
        public int m_iBestLapNum;                                  /* best lap index */
        public int m_iNumLaps;                                     /* number of laps */
        public int m_iGap;                                         /* milliseconds */
        public int m_iGapLaps;
        public int m_iPenalty;                                     /* milliseconds */
        public int m_iPit;                                         /* 0 = on track; 1 = in the pits */
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct SPluginsRaceTrackPosition
    {
        public int m_iRaceNum;                                     /* race number */
        public float m_fPosX, m_fPosY, m_fPosZ;                        /* meters */
        public float m_fYaw;                                       /* angle from north. degrees */
        public float m_fTrackPos;                                  /* position on the centerline, from 0 to 1 */
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct SPluginsRaceVehicleData
    {
        /// <summary>
        /// race number
        /// </summary>
        public int m_iRaceNum;
        /// <summary>
        /// if set to 0, the vehicle is not active and the following fields are not set
        /// </summary>
        public int m_iActive;
        /// <summary>
        /// engine RPM
        /// </summary>
        public int m_iRPM;
        /// <summary>
        /// 0 = Neutral
        /// </summary>
        public int m_iGear;
        /// <summary>
        /// meters/second
        /// </summary>
        public float m_fSpeedometer;
        /// <summary>
        /// -1 ( left ) to 1 ( right )
        /// </summary>
        public float m_fSteer;
        /// <summary>
        /// 0 to 1
        /// </summary>
        public float m_fThrottle;
        /// <summary>
        /// 0 to 1 
        /// </summary>
        public float m_fBrake;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct SPluginsSpectateVehicle
    {
        public int m_iRaceNum;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string m_szName;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct PluginInfo
    {
        /// <summary>
        /// message id
        /// </summary>
        public int m_id;
        /// <summary>
        /// 0: software running; 1: on-track, simulation paused; 2: on-track, simulation running
        /// </summary>
        public int m_iState;
        /// <summary>
        /// 0 = 100hz; 1 = 50hz; 2 = 20hz; 3 = 10hz; -1 = disable
        /// </summary>
        public int m_PluginRate;
        public int m_PluginVersion;
    }

    // Kart Events

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct KartEventInfo
    {
        /// <summary>
        /// message id
        /// </summary>
        public int m_id;
        public SPluginsKartEvent m_KartEvent;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct KartSessionInfo
    {
        /// <summary>
        /// message id
        /// </summary>
        public int m_id;
        public SPluginsKartSession m_KartSession;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct KartLapInfo
    {
        /// <summary>
        /// message id
        /// </summary>
        public int m_id;
        public SPluginsKartLap m_KartLap;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct KartSplitInfo
    {
        /// <summary>
        /// message id
        /// </summary>
        public int m_id;
        public SPluginsKartSplit m_KartSplit;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct KartTelemetryInfo
    {
        public int m_id;
        public float _fTime;
        public float _fPos;
        public SPluginsKartData m_KartData;
    }

    /// <summary>
    /// Track Centerline
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct TrackSegmentInfo
    {
        /// <summary>
        /// message id
        /// </summary>
        public int m_id;
        public int _iNumSegments;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 200)]
        public SPluginsTrackSegment[] m_TrackSegments;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] m_RaceData;
    }

    /// <summary>
    /// Race Events
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct RaceEventInfo
    {
        /// <summary>
        /// message id
        /// </summary>
        public int m_id;
        public SPluginsRaceEvent m_RaceEvent;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public struct RaceSessionInfo
    {
        /// <summary>
        /// message id
        /// </summary>
        public int m_id;                                                       
        public SPluginsRaceSession m_RaceSession;
    }

    public struct Marshalizable<T> where T : new()
    {
        /// <summary>
        /// Convert bytes to object T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        internal static T FromBytes(byte[] bytes)
        {
            try
            {
                GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
                T stuff = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
                handle.Free();
                return stuff;
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// Convert object T to bytes
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="bytes"></param>
        internal static void ToBytes(T obj, ref byte[] bytes)
        {
            GCHandle h = default;
            try
            {
                h = GCHandle.Alloc(bytes, GCHandleType.Pinned);
                Marshal.StructureToPtr<T>(obj, h.AddrOfPinnedObject(), false);
            }
            catch
            {
            }
            finally
            {
                if (h.IsAllocated)
                {
                    h.Free();
                }
            }
        }
    }
}