using CaptchaSolverServer.Models;
using CaptchaSolverServer.Services;
using CaptchaSolverServer.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CaptchaSolverServer
{
    public static class DIController
    {
        private static readonly ServiceProvider _provider;

        static DIController()
        {
            var services = new ServiceCollection();

            services.AddSingleton<PageService>();
            services.AddSingleton<RecognizeConfig>();

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<ImageProcessingPageViewModel>();
            services.AddSingleton<ServerSettingsPageViewModel>();
            services.AddSingleton<AboutPageViewModel>();

            _provider = services.BuildServiceProvider();
        }

        public static T Resolve<T>() => _provider.GetRequiredService<T>();
    }
}