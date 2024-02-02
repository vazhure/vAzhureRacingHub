using System.Web.Script.Serialization;

namespace vAzhureRacingAPI
{
    public static class ObjectSerializeHelper
    {
        static JavaScriptSerializer JsonSerializer => new JavaScriptSerializer();

        public static string GetJson(this object o)
        {
            return JsonSerializer.Serialize(o);
        }

        public static T DeserializeJson<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }

        public static T DeserializeJson<T>(string json, JavaScriptConverter[] converters)
        {
            JsonSerializer.RegisterConverters(converters);

            return JsonSerializer.Deserialize<T>(json);
        }

    }
}
