using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Windows.Input;
using System.Reflection;
using System.Windows.Media;
using CaptchaSolverServer.Helpers;
using CaptchaSolverServer.Models;
using DevExpress.Mvvm;
using DevExpress.Mvvm.UI;


namespace CaptchaSolverServer.ViewModels
{
    class ImageProcessingPageViewModel : BindableBase
    {
        public RecognizeConfig RecognizeConfig { get; }
        public ObservableCollection<MethodInfo> ImageProcessingMethods { get; set; }
        public ImageSource CurrentImage { get; set; }

        public bool ImageProcessingEnabled { get; set; }
        public string RecognizedText { get; set; }
        public string ImageStretchStyle { get; set; }



        private Bitmap _loadedImage;

        public ImageProcessingPageViewModel(RecognizeConfig recognizeConfig)
        {
            RecognizeConfig = recognizeConfig;
            ImageProcessingMethods =
                new ObservableCollection<MethodInfo>(
                    ReflectionHelper.FindMethods<BitmapFilters>(typeof(Bitmap), new[] { typeof(Bitmap) }));
            ImageStretchStyle = "None";
        }


        public ICommand AddImageProcessingMethod => new DelegateCommand<MethodInfo>(method =>
        {
            RecognizeConfig.ImageProcessingMethods.Add(method);
        });

        public ICommand RemoveImageProcessingMethod => new DelegateCommand<MethodInfo>(method =>
        {
            RecognizeConfig.ImageProcessingMethods.Remove(method);
        });

        public ICommand ClearImageProcessingMethodsCommand => new DelegateCommand(() =>
        {
            RecognizeConfig.ImageProcessingMethods.Clear();
        });

        public ICommand LoadImageCommand => new DelegateCommand(() =>
        {
            var ofd = new OpenFileDialogService()
            {
                Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG;*.ICO)|*.BMP;*.JPG;*.GIF;*.PNG;*.ICO",
                Title = "Load Image"
            };
            if (ofd.ShowDialog())
            {
                var pathImage = ofd.GetFullFileName();
                _loadedImage = new Bitmap(new FileStream(pathImage, FileMode.Open, FileAccess.Read));
                ImageStretchStyle = _loadedImage.Width > 570 ? "Uniform" : "None";
                RestoreCurrentImage();
            }
        });

        public ICommand RestoreCurrentImageCommand => new DelegateCommand(RestoreCurrentImage);

        public ICommand ApplyImageProcessingMethodsCommand => new DelegateCommand(() =>
       {
           var result = BitmapHelper.ApplyImageProcessingMethods(_loadedImage, RecognizeConfig.ImageProcessingMethods);
           CurrentImage = result.ToBitmapSource();
       });
        public ICommand RecognizeTextCommand => new AsyncCommand(async () =>
            {
                RecognizedText = await RecognizeConfig.RecognizeMethod(_loadedImage).ConfigureAwait(false);
            });
        private void RestoreCurrentImage() => CurrentImage = _loadedImage.ToBitmapSource();

    }
}
