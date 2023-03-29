using System;

namespace vAzhureRacingAPI
{
    public static class Math2
    {
        public static T Clamp<T>(T v, T min, T max) where T : IComparable
        {
            return v.CompareTo(min) < 0 ? min : v.CompareTo(max) > 0 ? max : v;
        }

        public static float Mapf(float x, float in_min, float in_max, float out_min, float out_max)
        {
            try
            {
                return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
            }
            catch
            {
                return out_min;
            }
        }

        public static double Mapd(double x, double in_min, double in_max, double out_min, double out_max)
        {
            try
            {
                return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
            }
            catch
            {
                return out_min;
            }
        }

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
    }
}
