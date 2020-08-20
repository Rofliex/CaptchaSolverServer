using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using CaptchaSolverServer.Helpers;
using Newtonsoft.Json;
using WatsonWebserver;

namespace CaptchaSolverServer.Models.CaptchaSolverServers.AntiCaptchaServer
{
    public delegate Task<string> RecognizeMethod(Bitmap captcha);


    class AntiCaptchaServer : IDisposable
    {
        enum FileEditMode
        {
            Add,
            Remove
        }
        public StatisticService ServerStatistic { get; set; }

        private readonly RecognizeMethod _recognizeMethod;
        private Server _server;
        private readonly string _guidApp;
        private readonly string _host;
        private readonly string _pathCertFile;
        private readonly int _port;
        private readonly bool _sslMode;
        private readonly ConcurrentBag<string> _clientKeys;
        private readonly ConcurrentDictionary<int, Captcha> _captchaDictionary;


        public AntiCaptchaServer(string Host, int Port, bool SslMode, string GuidAppString, RecognizeMethod RecognizeMethod, string PathCertFile)
        {
            if (Port <= 0 || Port > 65535 || string.IsNullOrEmpty(GuidAppString) || RecognizeMethod == null)
                throw new ArgumentException("Arguments corrupted.");
            _host = Host;
            _port = Port;
            _sslMode = SslMode;
            _guidApp = GuidAppString;
            _clientKeys = new ConcurrentBag<string>();
            _captchaDictionary = new ConcurrentDictionary<int, Captcha>();
            _recognizeMethod = RecognizeMethod;
            _pathCertFile = PathCertFile;
            ServerStatistic = new StatisticService();
        }


        public void Start()
        {
            if (_sslMode && !SetSSLCert())
                throw new SystemException("Can't set\\bind ssl certificate.");
            if (_host != "127.0.0.1")
                ModifyHostsFile(FileEditMode.Add);
            _server = new Server("127.0.0.1", _port, _sslMode, DefaultRouteAsync);
            _server.StaticRoutes.Add(HttpMethod.POST, "/createTask/", PostCreateTaskRouteAsync);
            _server.StaticRoutes.Add(HttpMethod.POST, "/createTask", PostCreateTaskRouteAsync);
            _server.StaticRoutes.Add(HttpMethod.POST, "/getTaskResult/", PostGetTaskResultRouteAsync);
            _server.StaticRoutes.Add(HttpMethod.POST, "/getTaskResult", PostGetTaskResultRouteAsync);
            _server.StaticRoutes.Add(HttpMethod.POST, "/getBalance", PostGetBalanceRouteAsync);
            _server.StaticRoutes.Add(HttpMethod.POST, "/getBalance/", PostGetBalanceRouteAsync);
        }

        public void Dispose()
        {
            _server?.Dispose();
            ModifyHostsFile(FileEditMode.Remove);
        }

        #region Routes

        async Task PostCreateTaskRouteAsync(HttpContext ctx)
        {
            ServerStatistic.AddCreateTaskRequest();
            string dataReq = await GetRequestDataStringAsync(ctx.Request.Data).ConfigureAwait(false);
            AntiCaptchaCreateTaskRequest requestDeserialized = JsonConvert.DeserializeObject<AntiCaptchaCreateTaskRequest>(dataReq);
            AntiCaptchaCreateTaskResponse responce = ProcessCreateTaskRequest(requestDeserialized);
            ctx.Response.StatusCode = 200;
            ctx.Response.ContentType = "application/json";
            await ctx.Response.Send(JsonConvert.SerializeObject(responce, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })).ConfigureAwait(false);
        }

        async Task PostGetTaskResultRouteAsync(HttpContext ctx)
        {
            string dataReq = await GetRequestDataStringAsync(ctx.Request.Data).ConfigureAwait(false);
            AntiCaptchaGetTaskResultRequest requestDeserialized = JsonConvert.DeserializeObject<AntiCaptchaGetTaskResultRequest>(dataReq);
            AntiCaptchaGetTaskResultResponse responce = ProcessGetTaskResultRequest(requestDeserialized);
            ctx.Response.StatusCode = 200;
            await ctx.Response.Send(JsonConvert.SerializeObject(responce, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })).ConfigureAwait(false);
        }

        private async Task PostGetBalanceRouteAsync(HttpContext ctx)
        {
            ctx.Response.StatusCode = 200;
            string dataReq = await GetRequestDataStringAsync(ctx.Request.Data).ConfigureAwait(false);
            var requestDeserialized = JsonConvert.DeserializeObject<AntiCaptchaGetBalanceRequest>(dataReq);
            AntiCaptchaGetBalanceResponse responce;
            if (_clientKeys.Count > 0 && !_clientKeys.Contains(requestDeserialized.ClientKey))
                responce = new AntiCaptchaGetBalanceResponse()
                {
                    ErrorId = 1,
                    ErrorCode = "ERROR_KEY_DOES_NOT_EXIST",
                    ErrorDescription = "Account authorization key not found in the system"
                };
            else
                responce = new AntiCaptchaGetBalanceResponse()
                {
                    ErrorId = 0,
                    Balance = 1337
                };
            await ctx.Response.Send(JsonConvert.SerializeObject(responce, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })).ConfigureAwait(false);
        }
        private async Task DefaultRouteAsync(HttpContext ctx)
        {
            ctx.Response.StatusCode = 403;
            await ctx.Response.Send("What are you doing?)").ConfigureAwait(false);
        }
        #endregion

        #region TaskProcessors

        AntiCaptchaCreateTaskResponse ProcessCreateTaskRequest(AntiCaptchaCreateTaskRequest requestDeserialized)
        {
            if (_clientKeys.Count > 0 && (!string.IsNullOrEmpty(requestDeserialized.ClientKey) || !_clientKeys.Contains(requestDeserialized.ClientKey)))
            {
                return new AntiCaptchaCreateTaskResponse { ErrorId = 1, ErrorCode = "ERROR_KEY_DOES_NOT_EXIST", ErrorDescription = "Account authorization key not found in the system" };
            }

            if (string.IsNullOrEmpty(requestDeserialized.Task.Body) || !BitmapHelper.CheckImageBody(requestDeserialized.Task.Body))
            {
                ServerStatistic.AddImageRecognizeErrors();
                return new AntiCaptchaCreateTaskResponse
                {
                    ErrorId = 15,
                    ErrorCode = "ERROR_IMAGE_TYPE_NOT_SUPPORTED",
                    ErrorDescription = "Could not determine captcha file type by its exif header or image type is not supported. The only allowed formats are JPG, GIF, PNG"
                };
            }

            Captcha captcha = new Captcha(requestDeserialized.Task.Body) { Status = CaptchaStatus.Processing };

            var currentTaskId = _captchaDictionary.Count;
            bool isAdded = _captchaDictionary.TryAdd(currentTaskId, captcha);
            if (!isAdded)
            {
                return new AntiCaptchaCreateTaskResponse
                {
                    ErrorId = 45,
                    ErrorCode = "ERROR_FACTORY_PLATFORM_OPERATION_FAILED",
                    ErrorDescription = "Factory Platform general error code."
                };
            }
            Task.Run(async () =>
            {
                await StartRecognizeCaptcha(captcha).ConfigureAwait(false);
            });
            return new AntiCaptchaCreateTaskResponse { ErrorId = 0, TaskId = currentTaskId };
        }

        async Task StartRecognizeCaptcha(Captcha captcha)
        {
            captcha.Result = await _recognizeMethod(captcha.Image).ConfigureAwait(false);
            captcha.Status = CaptchaStatus.Ready;
        }


        AntiCaptchaGetTaskResultResponse ProcessGetTaskResultRequest(AntiCaptchaGetTaskResultRequest requestDeserialized)
        {
            if (_captchaDictionary.Count == 0 || (requestDeserialized.TaskId.HasValue && !_captchaDictionary.ContainsKey(requestDeserialized.TaskId.Value)))
            {
                return new AntiCaptchaGetTaskResultResponse
                {
                    ErrorId = 1,
                    ErrorCode = "ERROR_FACTORY_TASK_NOT_FOUND",
                    ErrorDescription = "Task not found or not available for this operation"
                };
            }

            Captcha captcha = _captchaDictionary[requestDeserialized.TaskId.Value];
            if (captcha.Status == CaptchaStatus.Processing)
                return new AntiCaptchaGetTaskResultResponse { ErrorId = 0, Status = "processing" };
            if (captcha.Status == CaptchaStatus.Ready && string.IsNullOrEmpty(captcha.Result))
            {
                ServerStatistic.AddImageRecognizeErrors();
                return new AntiCaptchaGetTaskResultResponse
                {
                    ErrorId = 12,
                    ErrorCode = "ERROR_CAPTCHA_UNSOLVABLE",
                    ErrorDescription = "Captcha could not be solved"
                };
            }

            ServerStatistic.AddImageRecognizeGood();

            AntiCaptchaGetTaskResultResponse getTaskResultResponce = new AntiCaptchaGetTaskResultResponse
            {
                ErrorId = 0,
                SolutionInstance = new AntiCaptchaGetTaskResultResponse.Solution { Text = captcha.Result },
                Status = "ready"
            };
            captcha.Image.Dispose();
            return getTaskResultResponce;
        }

        public static async Task<string> GetRequestDataStringAsync(Stream stream)
        {
            List<byte> data = new List<byte>();
            int countReadBytes;
            do
            {
                var buffer = new byte[1024];
                countReadBytes = await stream.ReadAsync(buffer, 0, buffer.Length);
                data.AddRange(buffer.Take(countReadBytes));
            } while (countReadBytes > 0);
            return Encoding.UTF8.GetString(data.ToArray());
        }

        public void AddClientKeys(params string[] keys)
        {
            if (keys != null && keys.Length > 0)
            {
                foreach (string key in keys)
                    _clientKeys.Add(key);
            }
        }

        #endregion

        #region SSLUtility

        bool SetSSLCert()
        {
            X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            X509Certificate2 cert = new X509Certificate2(X509Certificate2.CreateFromCertFile(_pathCertFile));
            store.Open(OpenFlags.ReadWrite);
            store.Add(cert);
            store.Close();

            _ = GetResultFromNetsh($"http add sslcert ipport=0.0.0.0:{_port} certhash={cert.Thumbprint} appid={{{_guidApp}}} certstorename=Root");

            string result = GetResultFromNetsh($"http show sslcert ip = 0.0.0.0:{_port}");
            if (!result.Contains(cert.Thumbprint.ToLower()) || !result.Contains(_guidApp.ToLower()) || !GetResultFromNetsh($"http add urlacl url=https://127.0.0.1:{_port}/ user=everyone listen=yes").Contains("183"))
            {
                return false;
            }
            return true;
        }
        static string GetResultFromNetsh(string arg)
        {
            Process proc = new Process
            {
                StartInfo =
                {
                    FileName = "netsh.exe",
                    Arguments = arg,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };
            proc.Start();
            return proc.StandardOutput.ReadToEnd();
        }


        #endregion

        void ModifyHostsFile(FileEditMode fileEditMode)
        {
            string hostfilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "system32\\drivers\\etc\\hosts");
            string host = $"127.0.0.1 {_host}";
            var hostsFileContent = File.ReadAllText(hostfilePath);
            switch (fileEditMode)
            {
                case FileEditMode.Add:
                    if (!hostsFileContent.Contains(host))
                        File.AppendAllText(hostfilePath, "\r\n" + host);
                    break;
                case FileEditMode.Remove:
                    File.WriteAllText(hostfilePath, hostsFileContent.Replace("\r\n" + host, ""));
                    break;
            }
        }

    }
}
