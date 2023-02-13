using Predmetni_projekat_Nevena_Panić_PR43_2018.Model;
using Predmetni_projekat_Nevena_Panić_PR43_2018.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Predmetni_projekat_Nevena_Panić_PR43_2018
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private XML_Loader loader = new XML_Loader();
        public UIElement deletedElement { get; set; }

        // this is so I can backup canvas after clean
        Canvas BackUp { get; set; }
        bool cleared = false;
        bool gridDrawn = false;

        // canvas size chart
        List<string> canvasSizes = new List<string>() { "300 x 200", "1125 x 750", "2250 x 1500"};

        // TODO: fix undo/redo/clear with buttons
        // add opacity to shape windows

        public MainWindow()
        {
            DataStorage.DrawType = String.Empty;
            GetAllColors();
            InitializeComponent();
            canvas_size_cb.ItemsSource = canvasSizes;
            canvas_size_cb.SelectedItem = canvasSizes[1];
            customMap.Height = 750;
            customMap.Width = 1125;

            matrix_size.Content = $"{DataStorage.matrix_X} x {DataStorage.matrix_Y}";
        }

        private void load_btn_Click(object sender, RoutedEventArgs e)
        {
            loader.LoadEntities();
            MessageBox.Show("All information loaded.\n" +
                            "Number of entities: [" + DataStorage.allEntitiesDict.Count + "]\n" +
                            "Number of lines: [" + DataStorage.lines.Count + "]", "Successfully loaded Geographics.xml file.", MessageBoxButton.OK, MessageBoxImage.Information);
            load_btn.IsEnabled = false;
        }

        private void DrawEntities()
        {
            // Draw all entities
            foreach (PowerEntity entity in DataStorage.allEntitiesDict.Values)
            {
                Rectangle rect = new Rectangle();
                rect.Width = DataStorage.entityObjSize;
                rect.Height = DataStorage.entityObjSize;

                if (entity is SubstationEntity)
                {
                    rect.Fill = (Brush)(new BrushConverter().ConvertFrom("#272AB0"));
                    rect.ToolTip = "[Substation entity]\n";
                }
                else if (entity is NodeEntity)
                {
                    rect.Fill = (Brush)(new BrushConverter().ConvertFrom("#C21858"));
                    rect.ToolTip = "[Node entity]\n";
                }
                else if (entity is SwitchEntity)
                { 
                    rect.Fill = (Brush)(new BrushConverter().ConvertFrom("#57ACDC"));
                    rect.ToolTip = "[Switch entity]\n";
                }
                rect.StrokeThickness = 0.1;

                Canvas.SetLeft(rect, entity.MatrixX * DataStorage.entitySize);
                Canvas.SetTop(rect, entity.MatrixY * DataStorage.entitySize);

                rect.ToolTip += entity.ToString();
                rect.Name = String.Format("Entity_{0}", entity.Id);
                // must register new names to use FindName()
                RegisterName(rect.Name, rect);

                customMap.Children.Add(rect);
            }
        }

        private void DrawBFSLines(bool excludeNodes)            // bfs lines
        {
            Dictionary<long, List<MatrixPosition>> bfsLines = BFS_Algorithm.Search(excludeNodes);

            foreach (long id in bfsLines.Keys)
            {
                Polyline newLine = new Polyline();
                newLine.Stroke = (Brush)(new BrushConverter().ConvertFrom("#CED0E4"));
                newLine.StrokeThickness = DataStorage.entityObjMove;

                MatrixPosition point = bfsLines[id].First();
                // start
                newLine.Points.Add(new Point() { X = point.X * DataStorage.entitySize + DataStorage.entityObjMove, Y = point.Y * DataStorage.entitySize + DataStorage.entityObjMove });

                for (int i = 1; i < bfsLines[id].Count - 1; i++)
                {
                    point = bfsLines[id][i];
                    newLine.Points.Add(new Point() { X = point.X * DataStorage.entitySize + DataStorage.entityObjMove, Y = point.Y * DataStorage.entitySize + DataStorage.entityObjMove });
                }

                // end
                point = bfsLines[id].Last();
                newLine.Points.Add(new Point() { X = point.X * DataStorage.entitySize + DataStorage.entityObjMove, Y = point.Y * DataStorage.entitySize + DataStorage.entityObjMove });

                newLine.Name = $"Line_{id}";
                // must register new names to use FindName()
                RegisterName(newLine.Name, newLine);
                // tooltip required
                newLine.ToolTip = DataStorage.lines.Find(x => x.Id == id).ToString();

                customMap.Children.Add(newLine);
            }
        }

        private void DrawCrossSections()
        {
            foreach (MatrixPosition point in DataStorage.crossPositions)
            {
                if (FindName($"Cross_{point.X}_{point.Y}") == null)
                {

                    Ellipse cross = new Ellipse();
                    cross.Height = DataStorage.entityObjMove * 1.5;
                    cross.Width = DataStorage.entityObjMove * 1.5;
                    cross.Fill = Brushes.Black;

                    Canvas.SetLeft(cross, point.X * DataStorage.entitySize);
                    Canvas.SetTop(cross, point.Y * DataStorage.entitySize);

                    cross.ToolTip = $"Cross_{point.X}_{point.Y}";
                    cross.Name = $"Cross_{point.X}_{point.Y}";
                    // must register new namescto use FindName()
                    RegisterName(cross.Name, cross);

                    customMap.Children.Add(cross);
                }
            }
        }

        // =======================================================   Actions for button clicks and odther interactions from UI   =======================================================

        private void draw_grid_btn_Click(object sender, RoutedEventArgs e)
        {
            if (!load_btn.IsEnabled && !gridDrawn)
            {
                bool excludeNodes = (bool)nodes_checkbox.IsChecked;
                DateTime start = DateTime.Now;
                DrawBFSLines(excludeNodes);
                DrawCrossSections();
                DateTime end = DateTime.Now;
                draw_lines_time.Content = end.Subtract(start).ToString();
                DrawEntities();
                gridDrawn = true;
            }
            else if(load_btn.IsEnabled)
                    MessageBox.Show("Please load information from XML file first!", "ERROR: Missing info", MessageBoxButton.OK, MessageBoxImage.Error);
            else if (gridDrawn)
                    MessageBox.Show("Grid already drawn!", "ERROR: Can't draw same grid twice", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void customMap_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!String.IsNullOrEmpty(DataStorage.DrawType))               // if it is used for drawing, enter here
            {
                System.Windows.Point newPoint = new System.Windows.Point(e.GetPosition(customMap).X, e.GetPosition(customMap).Y);
                
                switch (DataStorage.DrawType)
                {
                    case "ellipse":
                        int drawingsNum = DataStorage.allDrawings.Count;
                        EllipseWindow ellipseWindow = new EllipseWindow();
                        ellipseWindow.ShowDialog();

                        if (DataStorage.allDrawings.Count > drawingsNum)        // new ellipse is added, proccess information 
                        {
                            // set ellipse at right coordinates
                            Canvas.SetLeft(DataStorage.allDrawings.Last(), newPoint.X);
                            Canvas.SetTop(DataStorage.allDrawings.Last(), newPoint.Y);

                            // now add it to canvas
                            customMap.Children.Add(DataStorage.allDrawings.Last());
                        }
                        break;
                    case "polygon":
                        // collecting all points of polygon
                        DataStorage.points.Add(newPoint);
                        break;
                    case "text":
                        TextWindow textWindow = new TextWindow();
                        textWindow.ShowDialog();

                        if (!String.IsNullOrEmpty(DataStorage.Text))           // only if text is entered
                        {
                            Label newText = new Label();
                            newText.Content = DataStorage.Text;
                            newText.FontSize = DataStorage.FontSize;
                            newText.Foreground = DataStorage.TextColor;
                            newText.Opacity = DataStorage.TextOpacity;

                            Canvas.SetLeft(newText, newPoint.X);
                            Canvas.SetTop(newText, newPoint.Y);

                            customMap.Children.Add(newText);
                            //allLabels.Add(newText);

                            DataStorage.DrawType = String.Empty;
                        }
                        break;
                    default:
                        break;
                }
            }
            else if (e.Source is Polyline)  // ako nesto puca ovde si dodala else if
            {
                ChangeEntityColorWindow lineWindow = new ChangeEntityColorWindow();
                lineWindow.ShowDialog();

                // get line ID frome name of clicked elemen => get clicked line
                long lineID = long.Parse((e.Source as FrameworkElement).Name.Split('_')[1]);
                LineEntity clickedLine = DataStorage.lines.Find(x => x.Id == lineID);

                // find end nodes using IDs
                Rectangle entity_1 = (Rectangle)this.FindName(String.Format("Entity_{0}", DataStorage.allEntitiesDict[clickedLine.FirstEnd].Id));
                Rectangle entity_2 = (Rectangle)this.FindName(String.Format("Entity_{0}", DataStorage.allEntitiesDict[clickedLine.SecondEnd].Id));

                // change color
                if (entity_1 != null)
                    entity_1.Fill = DataStorage.NewEntityColor;
                if (entity_2 != null)
                    entity_2.Fill = DataStorage.NewEntityColor;

                (e.Source as Polyline).Stroke = Brushes.Beige;
            }
        }

        private void customMap_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DataStorage.DrawType.Equals("polygon"))
            {
                int drawingCount = DataStorage.allDrawings.Count;
                PolygonWindow polygonWindow = new PolygonWindow();
                polygonWindow.ShowDialog();

                if (DataStorage.allDrawings.Count > drawingCount)   // only if drawing is added to list
                {
                    customMap.Children.Add(DataStorage.allDrawings.Last());
                }
            }
            else
            {
                if ((e.Source is Polygon || e.Source is Ellipse || e.Source is TextBlock) && LogicalTreeHelper.GetParent(e.OriginalSource as DependencyObject) is Canvas)
                {
                    EditWindow editWindow = new EditWindow(LogicalTreeHelper.GetParent(e.OriginalSource as DependencyObject) as Canvas);
                    editWindow.ShowDialog();
                }
                else if (e.Source is Label)
                {
                    EditTextWindow editTextWindow = new EditTextWindow(e.Source as Label);
                    editTextWindow.ShowDialog();
                }
            }
        }

        private void draw_ellipse_btn_Click(object sender, RoutedEventArgs e)
        {
            DataStorage.DrawType = "ellipse";
        }

        private void draw_polygon_btn_Click(object sender, RoutedEventArgs e)
        {
            DataStorage.DrawType = "polygon";
        }

        private void add_text_btn_Click(object sender, RoutedEventArgs e)
        {
            DataStorage.DrawType = "text";
        }

        private void GetAllColors()
        {
            Type colorsType = typeof(System.Windows.Media.Colors);
            PropertyInfo[] colorsTypePropertyInfos = colorsType.GetProperties(BindingFlags.Public | BindingFlags.Static);

            foreach (PropertyInfo colorsTypePropertyInfo in colorsTypePropertyInfos)
                DataStorage.allColors.Add(colorsTypePropertyInfo.Name);
        }

        // UNDO, REDO, CLEAR FUNCTIONS
        private void undo_btn_Click(object sender, RoutedEventArgs e)
        {
            if (cleared)    // after clean, return to state prior
            {
                var childrenList = BackUp.Children.Cast<UIElement>().ToArray();
                foreach (UIElement child in childrenList)
                {
                    BackUp.Children.Remove(child);
                    customMap.Children.Add(child);
                }
                cleared = false;
            }
            else if (customMap.Children.Count > 0)
            {
                deletedElement = customMap.Children[customMap.Children.Count - 1];
                customMap.Children.Remove(deletedElement);
            }
        }

        private void redo_btn_Click(object sender, RoutedEventArgs e)
        {
            if (deletedElement != null && !cleared)
                customMap.Children.Add(deletedElement);
            deletedElement = null;
        }

        private void clear_btn_Click(object sender, RoutedEventArgs e)
        {
            BackUp = new Canvas();
            gridDrawn = false;

            var childrenList = customMap.Children.Cast<UIElement>().ToArray();
            BackUp.Children.Clear();
            foreach (var child in childrenList)
            {
                customMap.Children.Remove(child);
                BackUp.Children.Add(child);
                if (child is Shape)
                    UnregisterName(((Shape)child).Name);
            }

            foreach (LineEntity line in DataStorage.lines)
                line.IsDrawn = false;

            DataStorage.crossPositions.Clear();

            cleared = true;
        }

        private void canvas_size_cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string newSize = canvasSizes[canvas_size_cb.SelectedIndex];
            gridDrawn = false;

            // canvas size
            DataStorage.canvasWidth = Int32.Parse(newSize.Split('x')[0].Trim());    // 300, 1125, 2250
            DataStorage.canvasHeight = (int)(DataStorage.canvasWidth / 1.5);

            customMap.Height = DataStorage.canvasHeight;
            customMap.Width = DataStorage.canvasWidth;

            // matrix size
            switch (DataStorage.canvasWidth)
            {
                case 300:
                    DataStorage.matrix_X = 100;
                    break;
                case 1125:
                    DataStorage.matrix_X = 300;
                    break;
                case 2250:
                    DataStorage.matrix_X = 500;
                    break;
            }
            DataStorage.matrix_Y = (int)(Math.Floor(DataStorage.matrix_X / 1.5));
            matrix_size.Content = $"{DataStorage.matrix_X} x {DataStorage.matrix_Y}";

            // entity size
            DataStorage.entitySize = (double)DataStorage.canvasWidth / DataStorage.matrix_X;
            DataStorage.entityObjSize = DataStorage.entitySize / 2F;
            DataStorage.entityObjMove = DataStorage.entitySize / 4F;

            // matrixx with all entitys
            DataStorage.entities = new PowerEntity[DataStorage.matrix_X, DataStorage.matrix_Y];
            if (DataStorage.allEntitiesDict != null)
            { 
                foreach (var entity in DataStorage.allEntitiesDict.Values)
                    XML_Loader.Scale(entity);
            }

            foreach (LineEntity line in DataStorage.lines)
                line.IsDrawn = false;

            DataStorage.lineMap = new char[DataStorage.matrix_X, DataStorage.matrix_Y];
            DataStorage.InitPathMap(DataStorage.lineMap);
            DataStorage.crossPositions.Clear();

            foreach (var child in customMap.Children)
            { 
                if(child is Shape)
                    UnregisterName((child as Shape).Name);
            }
            customMap.Children.Clear();
        }

        private void screenshot_btn_Click(object sender, RoutedEventArgs e)
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(customMap);

            RenderTargetBitmap rtb = new RenderTargetBitmap((Int32)bounds.Width, (Int32)bounds.Height, 96, 96, PixelFormats.Pbgra32);

            DrawingVisual dv = new DrawingVisual();

            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(customMap);
                dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }

            rtb.Render(dv);

            PngBitmapEncoder png = new PngBitmapEncoder();

            png.Frames.Add(BitmapFrame.Create(rtb));

            using (Stream stm = File.Create(Environment.CurrentDirectory + DateTime.Now.ToString()))
            {
                png.Save(stm);
                MessageBox.Show("Canvas saved as image.", "Canvas saved", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
