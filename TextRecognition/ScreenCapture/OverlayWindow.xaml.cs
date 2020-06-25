using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace TextRecognition
{
    /// <summary>
    /// Interaction logic for OverlayWindow.xaml
    /// </summary>
    public partial class OverlayWindow : Window
    {
        private System.Windows.Point _rectStart;

        public OverlayWindow(BitmapImage background)
        {
            InitializeComponent();
            Width = SystemParameters.VirtualScreenWidth;
            Height = SystemParameters.VirtualScreenHeight;
            Top = SystemParameters.VirtualScreenTop;
            Left = SystemParameters.VirtualScreenLeft;
            BackgroundImage.Source = background;
        }

        public Rectangle GetSelection()
        {
            var selection = SelectionRectangle.Rect;
            return new Rectangle(Convert.ToInt32(selection.X), Convert.ToInt32(selection.Y), Convert.ToInt32(selection.Width), Convert.ToInt32(selection.Height));
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
                return;
            }

            base.OnKeyDown(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            // Start the snip on mouse down
            _rectStart = e.GetPosition(OverlayGrid);
            SelectionRectangle.Rect = new Rect(_rectStart, new System.Windows.Size(0, 0));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_rectStart == default)
            {
                return;
            }

            var position = e.GetPosition(OverlayGrid);
            SelectionRectangle.Rect = new Rect(_rectStart, position);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            var position = e.GetPosition(OverlayGrid);          
            SelectionRectangle.Rect = new Rect(_rectStart, position);
            _rectStart = default;
            DialogResult = true;
            Close();
        }
    }
}
