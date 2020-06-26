using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SearchablePdfCreator.ScreenCapture
{
    class ScreenSource
    {
        public bool TryCapture(out Bitmap result)
        {
            var mainWindow = Application.Current.MainWindow;
            mainWindow.Hide();
            var fullScreenShot = ScreenShot.CaptureDesktop();
            var overlayWindow = new OverlayWindow(BitmapToImageSource(fullScreenShot));
            overlayWindow.Owner = mainWindow;
            var successful = overlayWindow.ShowDialog();
            mainWindow.Show();

            if (successful == false)
            {
                result = default;
                return false;
            }

            var rect = overlayWindow.GetSelection();
            result = fullScreenShot.Clone(rect, fullScreenShot.PixelFormat);
            return true;
        }

        private BitmapImage BitmapToImageSource(Image bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
