using System;
using System.Drawing;
using System.Windows;

namespace TextRecognition.ScreenCapture
{
    static class ScreenShot
    {
        public static Bitmap CaptureDesktop()
        {
            var rectangle = new Rectangle(Convert.ToInt32(SystemParameters.VirtualScreenTop), Convert.ToInt32(SystemParameters.VirtualScreenLeft),
                Convert.ToInt32(SystemParameters.VirtualScreenWidth), Convert.ToInt32(SystemParameters.VirtualScreenHeight));
            return CaptureInternal(rectangle);
        }

        private static Bitmap CaptureInternal(Rectangle rect)
        {
            var bmp = new Bitmap(rect.Width, rect.Height);

            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(rect.Location, System.Drawing.Point.Empty, rect.Size, CopyPixelOperation.SourceCopy);
                g.Flush();
            }

            return bmp;
        }
    }
}
