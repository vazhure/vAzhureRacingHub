using System.Runtime.InteropServices;

namespace vAzhureRacingAPI
{
    public struct Marshalizable<T> where T : new()
    {
        /// <summary>
        /// Convert bytes to object T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static T FromBytes(byte[] bytes)
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
        public static void ToBytes(T obj, ref byte[] bytes)
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
