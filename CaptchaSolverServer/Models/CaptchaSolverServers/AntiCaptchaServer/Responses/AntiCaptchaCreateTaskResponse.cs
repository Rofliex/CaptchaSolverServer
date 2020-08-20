using Newtonsoft.Json;

namespace CaptchaSolverServer.Models.CaptchaSolverServers.AntiCaptchaServer
{
    class AntiCaptchaCreateTaskResponse
    {
        [JsonProperty("errorId")] public int? ErrorId { get; set; } = null;
        [JsonProperty("taskId")] public int? TaskId { get; set; }
        [JsonProperty("errorCode")] public string ErrorCode { get; set; }
        [JsonProperty("errorDescription")] public string ErrorDescription { get; set; }
    }
}
