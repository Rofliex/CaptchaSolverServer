using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using AForge.Imaging.Filters;

namespace CaptchaSolverServer.Helpers
{
    public static class BitmapHelper
    {
        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr hObject);
        public static BitmapSource ToBitmapSource(this Bitmap source)
        {
            if (source == null)
                return null;
            BitmapSource bitSrc = null;

            var hBitmap = source.GetHbitmap();

            try
            {
                bitSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            catch (System.ComponentModel.Win32Exception)
            {
                bitSrc = null;
            }
            finally
            {
                DeleteObject(hBitmap);
            }

            return bitSrc;
        }

        public static bool CheckImageBody(string base64)
        {
            try
            {
                Image image = Base64ToImage(base64);
                if (image == null)
                    return false;
            }
            catch { return false; }
            return true;
        }
        public static Image Base64ToImage(string base64String)
        {
            // Convert base 64 string to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            // Convert byte[] to Image
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                Image image = Image.FromStream(ms, true);
                return image;
            }
        }

        public static Bitmap ApplyImageProcessingMethods(Bitmap target, IEnumerable<MethodInfo> methods)
        {
            if (target == null || methods.Count() == 0)
                return target;
            var filteredMethods = methods.Where(method =>
                method.ReturnType.Name == typeof(Bitmap).Name && method.GetParameters().Count() == 1 &&
                method.GetParameters().First().ParameterType.Name == typeof(Bitmap).Name);
            var image = (Bitmap)target.Clone();
            foreach (var method in filteredMethods)
            {
                image = (Bitmap)method.Invoke(null, new[] { image });
            }
            return image;
        }
    }
    public class BitmapFilters
    {
        #region  Filters AForge

        public static Bitmap Blur(Bitmap Image)
        {
            Blur filter = new Blur();
            Image = Image.Clone(new Rectangle(0, 0, Image.Width, Image.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Image = filter.Apply(Image);
            return Image;
        }

        public static Bitmap BrightnessCorrection(Bitmap Image)
        {
            BrightnessCorrection filter = new BrightnessCorrection();
            Image = Image.Clone(new Rectangle(0, 0, Image.Width, Image.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Image = filter.Apply(Image);
            return Image;
        }

        public static Bitmap ContrastCorrection(Bitmap Image)
        {
            ContrastCorrection filter = new ContrastCorrection();
            Image = Image.Clone(new Rectangle(0, 0, Image.Width, Image.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Image = filter.Apply(Image);
            return Image;
        }

        public static Bitmap SaturationCorrection(Bitmap Image)
        {
            SaturationCorrection filter = new SaturationCorrection();
            Image = Image.Clone(new Rectangle(0, 0, Image.Width, Image.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Image = filter.Apply(Image);
            return Image;
        }

        public static Bitmap Closing(Bitmap Image)
        {
            Closing filter = new Closing();
            Image = Image.Clone(new Rectangle(0, 0, Image.Width, Image.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Image = filter.Apply(Image);
            return Image;
        }

        public static Bitmap Dilatation(Bitmap Image)
        {
            Dilatation filter = new Dilatation();
            Image = Image.Clone(new Rectangle(0, 0, Image.Width, Image.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Image = filter.Apply(Image);
            return Image;
        }

        public static Bitmap Erosion(Bitmap Image)
        {
            Erosion filter = new Erosion();
            Image = Image.Clone(new Rectangle(0, 0, Image.Width, Image.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Image = filter.Apply(Image);
            return Image;
        }

        public static Bitmap BlobsFiltering(Bitmap Image)
        {
            BlobsFiltering filter = new BlobsFiltering();
            Image = Image.Clone(new Rectangle(0, 0, Image.Width, Image.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Image = filter.Apply(Image);
            return Image;
        }

        public static Bitmap Median2(Bitmap Image)
        {
            Median filter = new Median(2);
            Image = Image.Clone(new Rectangle(0, 0, Image.Width, Image.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Image = filter.Apply(Image);
            return Image;
        }

        public static Bitmap Opening(Bitmap Image)
        {
            Opening filter = new Opening();
            Image = Image.Clone(new Rectangle(0, 0, Image.Width, Image.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Image = filter.Apply(Image);
            return Image;
        }

        public static Bitmap Mean(Bitmap Image)
        {
            Mean filter = new Mean();
            Image = Image.Clone(new Rectangle(0, 0, Image.Width, Image.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Image = filter.Apply(Image);
            return Image;
        }

        public static Bitmap BilateralSmoothing(Bitmap Image)
        {
            BilateralSmoothing filter = new BilateralSmoothing();
            Image = Image.Clone(new Rectangle(0, 0, Image.Width, Image.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Image = filter.Apply(Image);
            return Image;
        }

        public static Bitmap Invert(Bitmap Image)
        {
            Invert filter = new Invert();
            Image = Image.Clone(new Rectangle(0, 0, Image.Width, Image.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Image = filter.Apply(Image);
            return Image;
        }

        public static Bitmap Convolution(Bitmap Image, int[,] kernel)
        {
            Convolution filter = new Convolution(kernel);
            Image = Image.Clone(new Rectangle(0, 0, Image.Width, Image.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Image = filter.Apply(Image);
            return Image;
        }

        public static Bitmap Convolution(Bitmap Image, int[,] kernel, int divisor)
        {
            Convolution filter = new Convolution(kernel, divisor);
            Image = Image.Clone(new Rectangle(0, 0, Image.Width, Image.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Image = filter.Apply(Image);
            return Image;
        }

        public static Bitmap GaussianSharpen(Bitmap Image)
        {
            GaussianSharpen filter = new GaussianSharpen();
            Image = Image.Clone(new Rectangle(0, 0, Image.Width, Image.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Image = filter.Apply(Image);
            return Image;
        }

        public static Bitmap Median(Bitmap Image)
        {
            Median filter = new Median();
            Image = Image.Clone(new Rectangle(0, 0, Image.Width, Image.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Image = filter.Apply(Image);
            return Image;
        }

        public static Bitmap Sharpen(Bitmap Image)
        {
            Sharpen filter = new Sharpen();
            Image = Image.Clone(new Rectangle(0, 0, Image.Width, Image.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Image = filter.Apply(Image);
            return Image;
        }

        public static Bitmap ResizeNearestNeighbor(Bitmap Image, int newWidth, int newHeight)
        {
            ResizeNearestNeighbor filter = new ResizeNearestNeighbor(newWidth, newHeight);
            Image = Image.Clone(new Rectangle(0, 0, Image.Width, Image.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Image = filter.Apply(Image);
            return Image;
        }

        public static Bitmap Shrink(Bitmap Image)
        {
            Shrink filter = new Shrink();
            Image = Image.Clone(new Rectangle(0, 0, Image.Width, Image.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Image = filter.Apply(Image);
            return Image;
        }

        public static Bitmap ColorFiltering(Bitmap Image)
        {
            ColorFiltering filter = new ColorFiltering
            {
                Blue = new AForge.IntRange(200, 255),
                Red = new AForge.IntRange(200, 255),
                Green = new AForge.IntRange(200, 255)
            };

            Image = Image.Clone(new Rectangle(0, 0, Image.Width, Image.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Image = filter.Apply(Image);

            return Image;
        }

        public static Bitmap SetPixelColor(Bitmap imgBmp, bool hasBeenCleared = true) //type 0 dafault, image has has been cleared.
        {
            var bgColor = Color.White;
            var textColor = Color.Black;
            for (var x = 0; x < imgBmp.Width; x++)
            {
                for (var y = 0; y < imgBmp.Height; y++)
                {
                    var pixel = imgBmp.GetPixel(x, y);
                    var isCloserToWhite = hasBeenCleared ? ((pixel.R + pixel.G + pixel.B) / 3) > 180 : ((pixel.R + pixel.G + pixel.B) / 3) > 120;
                    imgBmp.SetPixel(x, y, isCloserToWhite ? bgColor : textColor);
                }
            }

            return imgBmp;
        }

        public static Bitmap Scaled(Bitmap imgBmp) //type 0 dafault, image has has been cleared.
        {
            var scaleVal = 3f;

            var imgBmpScaled = new Bitmap(imgBmp, new Size((int)(imgBmp.Width * scaleVal), (int)(imgBmp.Height * scaleVal)));

            return imgBmpScaled;
        }

        #endregion

        #region Custom Filters

        public static Bitmap BinarizateBitmap(System.Drawing.Bitmap src)
        {
            double treshold = 0.6;

            Bitmap dst = new Bitmap(src.Width, src.Height);

            for (int i = 0; i < src.Width; i++)
            {
                for (int j = 0; j < src.Height; j++)
                {
                    dst.SetPixel(i, j, src.GetPixel(i, j).GetBrightness() < treshold ? System.Drawing.Color.Black : System.Drawing.Color.White);
                }
            }

            return dst;
        }

        public static Bitmap PintarPixelPretoDeBranco(Bitmap Image)
        {
            Image.Clone(new Rectangle(0, 0, Image.Width, Image.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            Color c = default;

            for (int X = 0; X <= (Image.Width) - 1; X++)
            {
                for (int Y = 0; Y <= (Image.Height) - 1; Y++)
                {
                    c = Image.GetPixel(X, Y);
                    if ((c.R + c.G + c.B > 210))
                    {
                        Image.SetPixel(X, Y, Color.FromArgb(c.A, 255, 255, 255));
                    }
                    else
                    {
                        Image.SetPixel(X, Y, Color.FromArgb(c.A, 0, 0, 0));
                    }
                }
            }

            return Image;
        }

        public static Bitmap TRT21Filter(Bitmap Image)
        {
            Bitmap temp = new Bitmap(Image);
            temp = PintarPixelPretoDeBranco(temp);
            temp = Opening(temp);
            return temp;
        }

        public static Bitmap Convolution(Bitmap Image)
        {
            int[,] kernel = {
                    { 2,-1, 2 },
                    {-1,10,-1 },
                    { 2,-1, 2 }
                };

            Bitmap temp = new Bitmap(Image);
            temp = Convolution(temp, kernel, 4);
            return temp;
        }

        #endregion
    }
}
