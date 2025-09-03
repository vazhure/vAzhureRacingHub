using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WreckfestPlugin
{
    public class Math2
    {
        public static Vector3 GetPYRFromQuaternion(Quaternion r)
        {
            float yaw = (float)Math.Atan2(2.0f * (r.Y * r.W + r.X * r.Z), 1.0f - 2.0f * (r.X * r.X + r.Y * r.Y));
            float pitch = (float)Math.Asin(2.0f * (r.X * r.W - r.Y * r.Z));
            float roll = (float)Math.Atan2(2.0f * (r.X * r.Y + r.Z * r.W), 1.0f - 2.0f * (r.X * r.X + r.Z * r.Z));

            return new Vector3(pitch, yaw, roll);
        }

        public static float LoopAngleRad(float angle, float minMag)
        {
            float absAngle = Math.Abs(angle);

            if (absAngle <= minMag)
            {
                return angle;
            }

            float direction = angle / absAngle;

            float loopedAngle = ((float)Math.PI * direction) - angle;

            return loopedAngle;
        }
    }
}
