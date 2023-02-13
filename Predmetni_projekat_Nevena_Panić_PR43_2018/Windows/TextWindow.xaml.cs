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
using System.Windows.Shapes;

namespace Predmetni_projekat_Nevena_Panić_PR43_2018.Windows
{
    /// <summary>
    /// Interaction logic for TextWindow.xaml
    /// </summary>
    public partial class TextWindow : Window
    {
        string Error { get; set; }
        public TextWindow()
        {
            Error = String.Empty;
            InitializeComponent();

            color_cb.ItemsSource = DataStorage.allColors;
            color_cb.SelectedItem = "Black";
        }

        private void add_btn_Click(object sender, RoutedEventArgs e)
        {
            // get text
            DataStorage.Text = text_tb.Text;

            // get font
            int fontSize;
            double opacity = 100;

            if (!String.IsNullOrEmpty(size_tb.Text))
            {
                if (Int32.TryParse(size_tb.Text, out fontSize) && fontSize > 0)
                    DataStorage.FontSize = fontSize;
                else
                    Error = "Font must be positive number! Please try again.";
            }

            if (!String.IsNullOrEmpty(opacity_tb.Text))
            {
                if (double.TryParse(opacity_tb.Text, out opacity) && opacity > 0 && opacity <= 100)
                    DataStorage.TextOpacity = opacity / 100F;
                else
                    Error = "Opacity must be positive number! Please try again.";
            }

            // get color
            BrushConverter converter = new BrushConverter();
            DataStorage.TextColor = converter.ConvertFromString(color_cb.SelectedItem.ToString()) as SolidColorBrush;

            // check errors
            if (String.IsNullOrEmpty(Error))
            {
                // all ok
                this.Close();
            }
            else
            {
                MessageBox.Show(Error, "Error message: ", MessageBoxButton.OK, MessageBoxImage.Error);
                Error = String.Empty;
            }
        }
        private void quit_btn_Click(object sender, RoutedEventArgs e)
        {
            DataStorage.Text = String.Empty;
            this.Close();
        }
    }
}
