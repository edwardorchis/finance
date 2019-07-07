using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Finance.Account.Controls.Commons
{
    public class NotifyAdorner : Adorner
    {
        private VisualCollection _visuals;
        private Canvas _grid;
        private Image _image;

        public NotifyAdorner(UIElement adornedElement, int level)
            : base(adornedElement)
        {
            _visuals = new VisualCollection(this);
            System.Resources.ResourceManager rm = Properties.Resources.ResourceManager;

            var msgLevel = (MessageLevel)level;
            var imageName = "notepad_icon";
            switch (msgLevel)
            {
                case MessageLevel.ERR:
                    break;
                case MessageLevel.INFO:
                    break;
                case MessageLevel.WARN:
                    break;
            }

            var image =Consts.BitmapToBitmapImage((System.Drawing.Bitmap)rm.GetObject(imageName));
            _image = new Image()
            {
                Source = image,            
                Width = 20,
                Height = 20
            };
           
            _image.MouseDown += (sender, e) => {
                adornedElement.RaiseEvent(new RoutedEventArgs { RoutedEvent = GotFocusEvent });
                KeyEventArgs args = new KeyEventArgs(System.Windows.Input.Keyboard.PrimaryDevice, System.Windows.Input.Keyboard.PrimaryDevice.ActiveSource, 0, Key.F8);
                args.RoutedEvent = KeyDownEvent;
                adornedElement.RaiseEvent(args);
            };
            _grid = new Canvas();
            _grid.Children.Add(_image);
            
            _visuals.Add(_grid);
        }

        public void ShowAdorner()
        {
            _image.Visibility = Visibility.Visible;
        }

        public void HideAdorner()
        {
            _image.Visibility = Visibility.Collapsed;
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return _visuals.Count;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return _visuals[index];
        }

        protected override Size MeasureOverride(Size constraint)
        {
            return base.MeasureOverride(constraint);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _grid.Arrange(new Rect(finalSize));

            _image.Margin = new Thickness(finalSize.Width - 28, finalSize.Height - 28, 0, 0);

            return base.ArrangeOverride(finalSize);
        }


    }
}
