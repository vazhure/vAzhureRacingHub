using System;

namespace vAzhureRacingAPI
{
    public static class Math2
    {
        public static T Clamp<T>(T v, T min, T max) where T : IComparable
        {
            return v.CompareTo(min) < 0 ? min : v.CompareTo(max) > 0 ? max : v;
        }

        public static float Mapf(float x, float in_min, float in_max, float out_min, float out_max, bool bClampInput = false, bool bClampOutput = false)
        {
            try
            {
                if (bClampInput)
                    x = Clamp(x, in_min, in_max);
                
                x = (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;

                if (bClampOutput)
                    x = Clamp(x, out_min, out_max);
                
                return x;
            }
            catch
            {
                return out_min;
            }
        }

        public static double Mapd(double x, double in_min, double in_max, double out_min, double out_max, bool bClampInput = false, bool bClampOutput = false)
        {
            try
            {
                if (bClampInput)
                    x = Clamp(x, in_min, in_max);

                x = (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;

                if (bClampOutput)
                    x = Clamp(x, out_min, out_max);

                return x;
            }
            catch
            {
                return out_min;
            }
        }

        public const double halfPI = 1.57;

        /// <summary>
        /// Расстояние между точками
        /// </summary>
        /// <param name="pt1">Координаты точки 1</param>
        /// <param name="pt2">Координаты точки 2</param>
        /// <returns></returns>
        internal static double Distance(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="forward"></param>
        /// <param name="up"></param>
        /// <returns>{yaw, pitch, roll} in radians</returns>
        public static float[] RadiansFromVectors(ref float[] forward, ref float[] up)
        {
            // Yaw is the bearing of the forward vector's shadow in the xy plane.
            float yaw = (float)Math.Atan2(forward[1], forward[0]);

            // Pitch is the altitude of the forward vector off the xy plane, toward the down direction.
            float pitch = (float)-Math.Asin(forward[2]);

            // Find the vector in the xy plane 90 degrees to the right of our bearing.
            float planeRightX = (float)Math.Sin(yaw);
            float planeRightY = (float)-Math.Cos(yaw);

            // Roll is the rightward lean of our up vector, computed here using a dot product.
            float roll = (float)Math.Asin(up[0] * planeRightX + up[1] * planeRightY);

            // If we're twisted upside-down, return a roll in the range +-(pi/2, pi)
            if (up[2] < 0)
                try
                {
                    roll = (float)(Math.Sign(roll) * Math.PI - roll);
                }
                catch { }

            // Convert radians to degrees.
            return new float[] { yaw, pitch, roll };
        }
    }
}
