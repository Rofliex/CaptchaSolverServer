using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading.Tasks;
using CaptchaSolverServer.Helpers;
using CaptchaSolverServer.Models.CaptchaSolverServers.AntiCaptchaServer;

namespace CaptchaSolverServer.Models
{
    public class RecognizeConfig
    {
        public ObservableCollection<MethodInfo> ImageProcessingMethods { get; }
        public string Language { get; set; }


        public RecognizeMethod RecognizeMethod => (b) => Task.Factory.StartNew(() =>
        {
            var img = BitmapHelper.ApplyImageProcessingMethods(b, ImageProcessingMethods);
            return OCR.Recognize(img, Language);
        });
        public RecognizeConfig()
        {
            ImageProcessingMethods = new ObservableCollection<MethodInfo>();
            Language = "eng";
        }
    }
}
