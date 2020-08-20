using Newtonsoft.Json;

namespace CaptchaSolverServer.Models.CaptchaSolverServers.AntiCaptchaServer
{
    class AntiCaptchaGetTaskResultResponse
    {
        [JsonProperty("errorId")] public int ErrorId { get; set; }

        [JsonProperty("errorCode")] public string ErrorCode { get; set; } = null;
        [JsonProperty("errorDescription")] public string ErrorDescription { get; set; } = null;
        [JsonProperty("status")] public string Status { get; set; } = null;
        [JsonProperty("solution")] public Solution SolutionInstance { get; set; } = null;
        public class Solution
        {
            [JsonProperty("text")] public string Text { get; set; } = null;
        }
    }
}
