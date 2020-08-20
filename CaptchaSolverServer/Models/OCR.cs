using System;
using System.Drawing;
using Tesseract;

namespace CaptchaSolverServer.Models
{
    class OCR
    {
        public static string Recognize(Bitmap b , string language = "eng")
        {
            string res = string.Empty;
            try
            {
                
                string path = $@"{Environment.CurrentDirectory}\tessdata\";
                using (var engine = new TesseractEngine(path, language,EngineMode.Default))
                {
                    string letters = "abcdefghijklmnopqrstuvwxyz";
                    string numbers = "0123456789";
                    engine.SetVariable("tessedit_char_whitelist", $"{numbers}{letters}{letters.ToUpper()}");
                    engine.SetVariable("tessedit_unrej_any_wd", true);
                    engine.SetVariable("tessedit_adapt_to_char_fragments", true);
                    engine.SetVariable("tessedit_redo_xheight", true);
                    engine.SetVariable("chop_enable", true);

                    using (var pix = PixConverter.ToPix(b))
                    using (var page = engine.Process(pix, PageSegMode.RawLine))
                        res = page.GetText().Replace(" ", "").Trim();
                }
            }
            catch { }
            return res;
        }
    }
}
