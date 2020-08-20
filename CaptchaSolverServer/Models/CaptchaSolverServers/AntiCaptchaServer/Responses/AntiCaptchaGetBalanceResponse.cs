using Newtonsoft.Json;

namespace CaptchaSolverServer.Models.CaptchaSolverServers.AntiCaptchaServer
{
    class AntiCaptchaGetBalanceResponse
    {
        [JsonProperty("errorId")] public int ErrorId { get; set; }

        [JsonProperty("balance")] public float? Balance { get; set; } = null;
        [JsonProperty("errorCode")] public string ErrorCode { get; set; } = null;
        [JsonProperty("errorDescription")] public string ErrorDescription { get; set; } = null;

    }
}
