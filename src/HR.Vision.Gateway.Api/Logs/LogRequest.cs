using Newtonsoft.Json;
using Serilog.Events;
using System;

namespace E.Shop.Gateway.Api.Logs
{
    public class LogRequest
    {
        [JsonProperty("level")]
        public LogEventLevel Level { get; set; }

        [JsonProperty("application")]
        public string Application { get; set; }

        [JsonProperty("user_id")]
        public Guid UserId { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
