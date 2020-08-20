using System;
using System.Windows.Input;
using CaptchaSolverServer.Helpers;
using CaptchaSolverServer.Models;
using CaptchaSolverServer.Models.CaptchaSolverServers.AntiCaptchaServer;
using DevExpress.Mvvm;

namespace CaptchaSolverServer.ViewModels
{
    class ServerSettingsPageViewModel : BindableBase, IDisposable
    {

        public RecognizeConfig RecognizeConfig { get; }
        public AntiCaptchaServer Server { get; set; }

        public string Host { get; set; }
        public int Port { get; set; }

        public bool SSL { get; set; }

        public bool Work { get; set; }
        
        public ServerSettingsPageViewModel(RecognizeConfig recognizeConfig)
        {
            RecognizeConfig = recognizeConfig;
            Host = "api.anti-captcha.com";
            Port = 80;
        }

        public ICommand StartServerCommand => new DelegateCommand(() =>
        {
            Work = true;
            Server = new AntiCaptchaServer(Host,Port,SSL,ReflectionHelper.GetGuidApplication(),RecognizeConfig.RecognizeMethod, Environment.CurrentDirectory + @"\Resources\CaptchaSolverServer.crt");
            Server.Start();
        });

        public ICommand StopServerCommand => new DelegateCommand( () =>
        {
            Work = false;
            Server?.Dispose();
        });

        public void Dispose()
        {
            Server?.Dispose();
        }
    }
}
