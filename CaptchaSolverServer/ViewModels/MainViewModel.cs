using System;
using System.Windows.Controls;
using System.Windows.Input;
using CaptchaSolverServer.Helpers;
using CaptchaSolverServer.Services;
using CaptchaSolverServer.Views;
using DevExpress.Mvvm;

namespace CaptchaSolverServer.ViewModels
{
    class MainViewModel : BindableBase
    {
        private readonly PageService _navigation;

        public Page CurrentPage { get; set; }
        public string ApplicationTitle { get; set; }
        

        public MainViewModel(PageService navigation)
        {
            _navigation = navigation;
            _navigation.OnPageChanged += (page) => CurrentPage = page;
            _navigation.Navigate(new AboutPage());

            ApplicationTitle = "Captcha Solver Server v"+ReflectionHelper.GetVersionApplication();
        }

        public ICommand CloseAppCommand => new DelegateCommand(() =>
        {
            DIController.Resolve<ServerSettingsPageViewModel>()?.Dispose();
            Environment.Exit(0);
        });

        public ICommand OpenImageProcessingPage => new DelegateCommand(() =>
        {
            _navigation.Navigate(new ImageProcessingPage());
        });
        public ICommand OpenServerSettingsPage => new DelegateCommand(() =>
        {
            _navigation.Navigate(new ServerSettingsPage());
        });
        public ICommand OpenAboutPage => new DelegateCommand(() =>
        {
            _navigation.Navigate(new AboutPage());
        });
    }
}