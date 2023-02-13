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
    /// Interaction logic for ChangeEntityColorWindow.xaml
    /// </summary>
    public partial class ChangeEntityColorWindow : Window
    {
        public ChangeEntityColorWindow()
        {
            InitializeComponent();
            color_cb.ItemsSource = DataStorage.allColors;
            color_cb.SelectedItem = "Teal";
        }

        private void color_btn_Click(object sender, RoutedEventArgs e)
        {
            BrushConverter converter = new BrushConverter();
            DataStorage.NewEntityColor = converter.ConvertFromString(color_cb.SelectedItem.ToString()) as SolidColorBrush;
            this.Close();
        }
    }
}
