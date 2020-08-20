using System;
using Newtonsoft.Json;

namespace CaptchaSolverServer.Models.CaptchaSolverServers.AntiCaptchaServer
{
    class AntiCaptchaGetTaskResultRequest
    {
        [JsonProperty("clientKey")] public string ClientKey { get; set; }
        [JsonProperty("taskId")] public Nullable<int> TaskId { get; set; }
    }
}