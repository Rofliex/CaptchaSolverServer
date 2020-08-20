using System;
using System.Drawing;
using System.IO;

namespace CaptchaSolverServer.Models.CaptchaSolverServers
{
    public class Captcha
    {
        

        public CaptchaStatus Status { get; set; }
        public string Result { get; set; }

        public Bitmap Image { get; private set; }
        private Captcha()
        {

        }
        public Captcha(Bitmap image)
        {
            Status = CaptchaStatus.Processing;
            Result = null;
            Image = image;
        }
        public Captcha(string base64Image)
        {
            Status = CaptchaStatus.Processing;
            Result = null;
            Image = Base64ToBitmap(base64Image);
        }

        static Bitmap Base64ToBitmap(string base64String)
        {
            // Convert base 64 string to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            // Convert byte[] to Image
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                Image image = System.Drawing.Image.FromStream(ms, true);
                return (Bitmap)image;
            }
        }
    }
    public enum CaptchaStatus
    {
        Ready,
        Processing
    }
}
