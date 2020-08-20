using Newtonsoft.Json;

namespace CaptchaSolverServer.Models.CaptchaSolverServers.AntiCaptchaServer
{
    class AntiCaptchaCreateTaskRequest
    {
        [JsonProperty("clientKey")] public string ClientKey { get; set; }
        [JsonProperty("task")] public AnticaptchaCreateTaskRequestParams Task { get; set; }

        public class AnticaptchaCreateTaskRequestParams
        {
            [JsonProperty("type")] public string Type { get; set; }
            [JsonProperty("body")] public string Body { get; set; }
            [JsonProperty("phrase")] public bool Phrase { get; set; }
            [JsonProperty("numeric")] public bool Numeric { get; set; }
            [JsonProperty("minLength")] public int MinLength { get; set; }
            [JsonProperty("maxLength")] public int MaxLength { get; set; }
        }
    }
}