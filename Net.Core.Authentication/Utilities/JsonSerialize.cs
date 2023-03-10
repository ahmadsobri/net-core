using System.Text.Json;

namespace Net.Core.Authentication.Utilities
{
    public class JsonSerialize<T>
    {
        //JsonSerializerOptions options = new JsonSerializerOptions()
        //{
        //    NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
        //};
        public string ToString(T t)
        {
            return JsonSerializer.Serialize(t);
        }

        public T ToObject(string s)
        {
            return JsonSerializer.Deserialize<T>(s);
        }
    }
}
