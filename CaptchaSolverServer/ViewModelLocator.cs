using CaptchaSolverServer.ViewModels;

namespace CaptchaSolverServer
{
    class ViewModelLocator
    {
        public MainViewModel MainViewModel => DIController.Resolve<MainViewModel>();
        public ImageProcessingPageViewModel ImageProcessingPageViewModel => DIController.Resolve<ImageProcessingPageViewModel>();
        public ServerSettingsPageViewModel ServerSettingsPageViewModel => DIController.Resolve<ServerSettingsPageViewModel>();
        public AboutPageViewModel AboutPageViewModel => DIController.Resolve<AboutPageViewModel>();
    }
}