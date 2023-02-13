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
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        Canvas ToEdit { get; set; }
        string Error { get; set; }
        BrushConverter converter = new BrushConverter();
        public EditWindow(Canvas editedCanvas)
        {
            Error = String.Empty;
            ToEdit = editedCanvas;
            InitializeComponent();

            // fixed
            fill_color_cb.ItemsSource = DataStorage.allColors;
            stroke_color_cb.ItemsSource = DataStorage.allColors;
            text_color_cb.ItemsSource = DataStorage.allColors;

            // display values of passed element
            thickness_tb.Text = (ToEdit.Children[0] as Shape).StrokeThickness.ToString();

            SolidColorBrush brushTest, text;
            SolidColorBrush fill = (ToEdit.Children[0] as Shape).Fill as SolidColorBrush;
            SolidColorBrush stroke = (ToEdit.Children[0] as Shape).Stroke as SolidColorBrush;
            text = null;
            if (ToEdit.Children[1] != null)
            {
                text = (ToEdit.Children[1] as TextBlock).Foreground as SolidColorBrush;
                opacity_tb.Text = $"{ToEdit.Children[1].Opacity * 100}";
            }

            // set default values for colors to be displayed
            foreach (string color in DataStorage.allColors)
            {
                brushTest = converter.ConvertFromString(color) as SolidColorBrush;
                if (brushTest == fill)
                    fill_color_cb.SelectedItem = color;
                if (brushTest == stroke)
                    stroke_color_cb.SelectedItem = color;
                if (brushTest == text && text != null)
                    text_color_cb.SelectedItem = color;
            }
        }

        private void edit_btn_Click(object sender, RoutedEventArgs e)
        {
            double stroke_thickness, opacity;

            if (!double.TryParse(thickness_tb.Text, out stroke_thickness) || stroke_thickness < 0)
                Error += "Stroke thickness must be positive number! \n";
            else
                (ToEdit.Children[0] as Shape).StrokeThickness = stroke_thickness;

            if (double.TryParse(opacity_tb.Text, out opacity) && opacity > 0 && opacity <= 100)
                ToEdit.Opacity = opacity / 100F;
            else
                Error += "Opacity must be positive number! \n";

            if (String.IsNullOrEmpty(Error))
            {   
                SolidColorBrush fillColorBrush, strokeColorBrush;
                fillColorBrush = (fill_color_cb.SelectedItem != null) ? converter.ConvertFromString(fill_color_cb.SelectedItem.ToString()) as SolidColorBrush : converter.ConvertFromString("White") as SolidColorBrush;
                strokeColorBrush = (stroke_color_cb.SelectedItem != null) ? converter.ConvertFromString(stroke_color_cb.SelectedItem.ToString()) as SolidColorBrush : converter.ConvertFromString("Black") as SolidColorBrush;

                (ToEdit.Children[0] as Shape).Fill = fillColorBrush;
                (ToEdit.Children[0] as Shape).Stroke = strokeColorBrush;
                (ToEdit.Children[0] as Shape).Opacity = opacity / 100F;

                if (ToEdit.Children[1] != null)
                {
                    SolidColorBrush textColorBrush = converter.ConvertFromString(text_color_cb.SelectedItem.ToString()) as SolidColorBrush;
                    (ToEdit.Children[1] as TextBlock).Foreground = textColorBrush;
                    (ToEdit.Children[1] as TextBlock).Opacity = opacity / 100F;
                }

                this.Close();
            }
            else
            {
                MessageBox.Show(Error, "Error list:", MessageBoxButton.OK, MessageBoxImage.Error);
                Error = String.Empty;
            }
        }

        private void quit_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
