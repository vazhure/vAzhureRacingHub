using System;
using System.Reflection;
using vAzhureRacingAPI;

namespace vAzhureRacingHub
{
    public static class PluginManager
    {
        /// <summary>
        /// Загрузка плагина из dll
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static ICustomPlugin LoadFromFile(string filepath)
        {
            try
            {
                Assembly plugin = Assembly.LoadFrom(filepath);

                foreach (Type type in plugin.GetTypes())
                {
                    if (type.IsClass)
                    {
                        foreach (Type subtype in type.GetInterfaces())
                        {
                            if (subtype.Name == "ICustomPlugin")
                            {
                                Type[] types = new Type[0];
                                ConstructorInfo constructorInfoObj = type.GetConstructor(
                                    BindingFlags.Instance | BindingFlags.Public, null,
                                    CallingConventions.HasThis, types, null);

                                if (constructorInfoObj != null)
                                {
                                    var obj = constructorInfoObj.Invoke(null);
                                    if (obj is ICustomPlugin custom)
                                        return custom;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
               
            }
            return null;
        }
    }
}
