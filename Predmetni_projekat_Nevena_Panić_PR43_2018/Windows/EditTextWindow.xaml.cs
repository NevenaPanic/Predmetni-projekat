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
    /// Interaction logic for EditTextWindow.xaml
    /// </summary>
    public partial class EditTextWindow : Window
    {
        BrushConverter converter = new BrushConverter();
        string Error { get; set; }
        Label ToEdit { get; set; }
        public EditTextWindow(Label editedLabel)
        {
            Error = String.Empty;
            InitializeComponent();
            ToEdit = editedLabel;
            color_cb.ItemsSource = DataStorage.allColors;

            SolidColorBrush brushTest, text;
            text = (editedLabel as Label).Foreground as SolidColorBrush;

            // set default values for colors to be displayed
            foreach (string color in DataStorage.allColors)
            {
                brushTest = converter.ConvertFromString(color) as SolidColorBrush;
                if (brushTest == text && text != null)
                    color_cb.SelectedItem = color;
            }

            size_tb.Text = editedLabel.FontSize.ToString();
            opacity_tb.Text = $"{ editedLabel.Opacity * 100}";
        }

        private void edit_btn_Click(object sender, RoutedEventArgs e)
        {
            // get font
            int fontSize;
            double opacity;

            if (Int32.TryParse(size_tb.Text, out fontSize) && fontSize > 0)
                ToEdit.FontSize = fontSize;
            else
                Error = "Font must be positive number! Please try again.";

            if (!String.IsNullOrEmpty(opacity_tb.Text))
            {
                if (double.TryParse(opacity_tb.Text, out opacity) && opacity > 0 && opacity <= 100)
                    ToEdit.Opacity = opacity / 100F;
                else
                    Error = "Opacity must be positive number! Please try again.";
            }

            // get color
            ToEdit.Foreground = converter.ConvertFromString(color_cb.SelectedItem.ToString()) as SolidColorBrush;

            // check errors
            if (String.IsNullOrEmpty(Error))
            {
                this.Close();   // all good ...
            }
            else
            {
                MessageBox.Show(Error, "Error message: ", MessageBoxButton.OK, MessageBoxImage.Error);
                Error = String.Empty;
            }
        }

        private void quit_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
