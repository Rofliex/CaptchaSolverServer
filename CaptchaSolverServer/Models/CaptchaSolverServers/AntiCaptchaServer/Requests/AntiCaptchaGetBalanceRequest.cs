using Newtonsoft.Json;

namespace CaptchaSolverServer.Models.CaptchaSolverServers.AntiCaptchaServer
{
    class AntiCaptchaGetBalanceRequest
    {
        [JsonProperty("clientKey")] public string ClientKey { get; set; }
    }
}
