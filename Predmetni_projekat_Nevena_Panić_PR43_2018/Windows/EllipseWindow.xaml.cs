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
    /// Interaction logic for EllipseWindow.xaml
    /// </summary>
    public partial class EllipseWindow : Window
    {
        public string Error { get; set; }
        public EllipseWindow()
        {
            Error = String.Empty;
            InitializeComponent();

            // fixed
            fill_color_cb.ItemsSource = DataStorage.allColors;
            stroke_color_cb.ItemsSource = DataStorage.allColors;
            text_color_cb.ItemsSource = DataStorage.allColors;

            // set default values for colors to be displayed
            fill_color_cb.SelectedItem = "White";
            stroke_color_cb.SelectedItem = "Black";
            text_color_cb.SelectedItem = "Black";
        }

        private void draw_btn_Click(object sender, RoutedEventArgs e)
        {
            // get all values for Ellipse drawing
            double x, y, stroke_thickness, opacity;
            if (!double.TryParse(x_tb.Text, out x) || x < 0)
                Error += "X must be positive number! \n";
            if (!double.TryParse(y_tb.Text, out y) || y < 0)
                Error += "Y must be positive number! \n";
            if (!double.TryParse(thickness_tb.Text, out stroke_thickness) || stroke_thickness < 0)
                Error += "Stroke thickness must be positive number! \n";
            if (!double.TryParse(opacity_tb.Text, out opacity) || opacity < 0 || opacity > 100)
                Error += "Opacity must be positive number! \n";


            // get all colors picked
            BrushConverter converter = new BrushConverter();
            SolidColorBrush fillColorBrush, strokeColorBrush, textColorBrush;
            fillColorBrush = (fill_color_cb.SelectedItem != null) ? converter.ConvertFromString(fill_color_cb.SelectedItem.ToString()) as SolidColorBrush : converter.ConvertFromString("White") as SolidColorBrush;
            strokeColorBrush = (stroke_color_cb.SelectedItem != null) ? converter.ConvertFromString(stroke_color_cb.SelectedItem.ToString()) as SolidColorBrush : converter.ConvertFromString("Black") as SolidColorBrush;


            // if error string is null
            if (String.IsNullOrEmpty(Error))
            {
                // make new canvas, transperent layer until stated otherwise
                Canvas newCanvas = new Canvas();

                // make new Ellipse
                Ellipse newEllipse = new Ellipse() { Width = x, Height = y, StrokeThickness = stroke_thickness, Stroke = strokeColorBrush, Fill = fillColorBrush, Opacity = opacity / 100F };

                // add ellipse to new canvas
                newCanvas.Children.Add(newEllipse);

                // check if user wants to add text - OPTIONAL 
                if (!String.IsNullOrEmpty(text_tb.Text))
                {
                    textColorBrush = converter.ConvertFromString(text_color_cb.SelectedItem.ToString()) as SolidColorBrush;

                    TextBlock newText = new TextBlock() { Width = 0.7 * x, Height = 0.7 * y };
                    newText.HorizontalAlignment = HorizontalAlignment.Center;
                    newText.VerticalAlignment = VerticalAlignment.Center;
                    newText.TextWrapping = TextWrapping.Wrap;

                    newText.Text = text_tb.Text;
                    newText.Foreground = textColorBrush;
                    newText.Opacity = opacity / 100F;

                    Canvas.SetTop(newText, 0.15 * y);
                    Canvas.SetLeft(newText, 0.15 * x);

                    newCanvas.Children.Add(newText);

                    //TODO: text isn't centered, fix that
                }

                // clear all data holders for next drawing
                DataStorage.DrawType = String.Empty;
                DataStorage.points.Clear();

                // add it to the drawing list
                DataStorage.allDrawings.Add(newCanvas);

                this.Close();                   // successfully added ellipse, close the window
            }
            else
            {
                MessageBox.Show(Error, "Error list", MessageBoxButton.OK, MessageBoxImage.Error);
                Error = String.Empty;
            }
        }

        private void quit_btn_Click(object sender, RoutedEventArgs e)
        {
            // clear all data holders for next drawing
            DataStorage.DrawType = String.Empty;
            DataStorage.points.Clear();

            this.Close();
        }
    }
}
