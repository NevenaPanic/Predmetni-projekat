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
    /// Interaction logic for PolygonWindow.xaml
    /// </summary>
    public partial class PolygonWindow : Window
    {
        string Error { get; set; }
        public PolygonWindow()
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
            Canvas newCanvas = new Canvas();
            Polygon newPolygon = new Polygon();
            BrushConverter converter = new BrushConverter();
            double stroke_thickness, opacity;
            double avgX = 0, avgY = 0;      // used to pinpoint where to draw textblock

            List<double> allXs = new List<double>();
            List<double> allYs = new List<double>();

            // add all points
            foreach (Point point in DataStorage.points)
            {
                newPolygon.Points.Add(point);
                allXs.Add(point.X);
                allYs.Add(point.Y);
            }
            avgX = (allXs.Min() + allXs.Max()) / 2;
            avgY = (allYs.Min() + allYs.Max()) / 2;

            if (!double.TryParse(thickness_tb.Text, out stroke_thickness) || stroke_thickness < 0)
                Error = "Stroke thickness must be positive number! \n";
            if (!double.TryParse(opacity_tb.Text, out opacity) || opacity < 0 || opacity > 100)
                Error += "Opacity must be positive number! \n";

            if (String.IsNullOrEmpty(Error))
            {
                newPolygon.StrokeThickness = stroke_thickness;
                // get all colors picked
                SolidColorBrush fillColorBrush, strokeColorBrush;
                fillColorBrush = (fill_color_cb.SelectedItem != null) ? converter.ConvertFromString(fill_color_cb.SelectedItem.ToString()) as SolidColorBrush : converter.ConvertFromString("White") as SolidColorBrush;
                strokeColorBrush = (stroke_color_cb.SelectedItem != null) ? converter.ConvertFromString(stroke_color_cb.SelectedItem.ToString()) as SolidColorBrush : converter.ConvertFromString("Black") as SolidColorBrush;
                newPolygon.Fill = fillColorBrush;
                newPolygon.FillRule = FillRule.Nonzero;
                newPolygon.Stroke = strokeColorBrush;
                newPolygon.Opacity = opacity / 100F;

                newCanvas.Children.Add(newPolygon);

                // if text exists
                if (!String.IsNullOrEmpty(text_tb.Text))
                {
                    TextBlock text = new TextBlock();
                    text.Text = text_tb.Text;
                    SolidColorBrush textColorBrush = converter.ConvertFromString(text_color_cb.SelectedItem.ToString()) as SolidColorBrush;
                    text.Foreground = textColorBrush;
                    text.Opacity = opacity / 100F;

                    Canvas.SetTop(text, avgY);
                    Canvas.SetLeft(text, avgX - 0.05 * avgX);

                    newCanvas.Children.Add(text);
                }
            }


            if (String.IsNullOrEmpty(Error))
            {
                DataStorage.points.Clear();
                DataStorage.DrawType = String.Empty;

                DataStorage.allDrawings.Add(newCanvas);
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
            DataStorage.DrawType = String.Empty;
            DataStorage.points.Clear();
            this.Close();
        }
    }
}
