using System;
using System.Drawing;
using System.IO;

namespace WebApi.Utils
{
    public static class ImageHelper
    {
        public static Bitmap FromBase64StringToImage(this string base64String)
        {
            byte[] byteBuffer = Convert.FromBase64String(base64String);
            try
            {
                using var memoryStream = new MemoryStream(byteBuffer) {Position = 0};
                using var imgReturn = Image.FromStream(memoryStream);
                memoryStream.Close();
                return new Bitmap(imgReturn);
            }
            catch { return null; }
        }
    }
}