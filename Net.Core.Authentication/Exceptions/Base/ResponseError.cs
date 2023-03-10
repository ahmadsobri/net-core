using System.Text.Json;
using System.Text.Json.Serialization;

namespace Net.Core.Authentication.Exceptions.Base
{
    public class ResponseError
    {
        [JsonPropertyName("Status")]
        public string Status { get; set; }
        [JsonPropertyName("Message")]
        public string Message { get; set; }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
