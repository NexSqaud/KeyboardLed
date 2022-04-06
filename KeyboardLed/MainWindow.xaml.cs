using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KeyboardLed
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private KeyboardController _controller = new();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void KeyMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!PixelColorPicker.SelectedColor.HasValue)
            {
                return;
            }

            var rectangle = (Rectangle)sender;
            var lineIndex = int.Parse(rectangle.Name[1..2]);
            var pixelIndex = int.Parse(rectangle.Name[3..]);

            var color = PixelColorPicker.SelectedColor.Value;
            rectangle.Fill = new SolidColorBrush(color);

            _controller.SetPixelColor(lineIndex, pixelIndex, new Color()
            {
                Red = color.R,
                Green = color.G,
                Blue = color.B
            });
        }

        private void RowFillClick(object sender, RoutedEventArgs e)
        {
            if (!PixelColorPicker.SelectedColor.HasValue)
            {
                return;
            }

            var button = (Button)sender;
            var lineIndex = int.Parse(button.Name[1..]);
            var color = PixelColorPicker.SelectedColor.Value;

            int i = 0;
            while(true)
            {
                var key = FindName($"K{lineIndex}_{i}");
                if(key == null)
                {
                    break;
                }
                ((Rectangle)key).Fill = new SolidColorBrush(color);
                i++;
            }

            _controller.SetLineColor(lineIndex, new Color
            {
                Red = color.R,
                Green = color.G,
                Blue = color.B
            });
        }
        private void FillButton_Click(object sender, RoutedEventArgs e)
        {
            if (!PixelColorPicker.SelectedColor.HasValue)
            {
                return;
            }
            var color = PixelColorPicker.SelectedColor.Value;

            int i = 0;
            int j = 0;
            while(true)
            {
                var key = FindName($"K{i}_{j}");
                if(key == null)
                {
                    if(j == 0)
                    {
                        break;
                    }
                    else
                    {
                        i++;
                        j = 0;
                    }
                    continue;
                }
                ((Rectangle)key).Fill = new SolidColorBrush(color);
                j++;
            }

            _controller.Fill(new Color
            {
                Red = color.R,
                Green = color.G,
                Blue = color.B
            });
        }

        private void PushButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _controller.PushBuffer();
            }
            catch(Exception)
            {

            }
        }

    }
}
