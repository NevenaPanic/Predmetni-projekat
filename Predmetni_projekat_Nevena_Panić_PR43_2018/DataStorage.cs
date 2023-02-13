using Predmetni_projekat_Nevena_Panić_PR43_2018.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Predmetni_projekat_Nevena_Panić_PR43_2018
{
    public class DataStorage
    {
        // canvas size, doesn't change
        public static int canvasWidth = 1125;   //px
        public static int canvasHeight = 750;   //px

        // matrix size values
        public static int matrix_X = 300;   // width of matrix for entities
        public static int matrix_Y = (int)(Math.Floor(matrix_X / 1.5));     // height of matrix for entities

        // entity size
        public static double entitySize = (double)canvasWidth / matrix_X;     // canvas size is 1200px
        public static double entityObjSize = entitySize / 2F;
        public static double entityObjMove = entitySize / 4F;

        // matrix with all entitys
        public static PowerEntity[,] entities = new PowerEntity[matrix_X, matrix_Y];
        public static Dictionary<long, PowerEntity> allEntitiesDict = new Dictionary<long, PowerEntity>();

        // list with all lines
        public static List<LineEntity> lines = new List<LineEntity>();
        public static char[,] lineMap = new char[matrix_X, matrix_Y];

        public static bool[,] visited_s;
        public static bool[,] visited_d;
        public static MatrixPosition[,] parentCell_s;
        public static MatrixPosition[,] parentCell_d;
        public static MatrixPosition intersection;
        public static List<MatrixPosition> crossPositions = new List<MatrixPosition>();



        // values to scale my canvas with
        public static double minLatitude;
        public static double minLongitude;
        public static double maxLatitude;
        public static double maxLongitude;

        // parameters for shape drawing
        public static List<Canvas> allDrawings = new List<Canvas>();
        public static List<System.Windows.Point> points = new List<System.Windows.Point>();
        public static string DrawType { get; set; }

        // list of all colors from Brushes will be stored here
        public static List<string> allColors = new List<string>();
        public static SolidColorBrush NewEntityColor { get; set; }

        // input text property
        public static string Text { get; set; }
        public static int FontSize { get; set; }
        public static SolidColorBrush TextColor { get; set; }
        public static double TextOpacity { get; set; }

        public static void InitPathMap(char[,] lineMap)
        {
            for (int i = 0; i < matrix_X; i++)
                for (int j = 0; j < matrix_Y; j++)
                    lineMap[i,j] = 'o';
        }

        public static void PrintLineMap()
        {
            for (int i = 0; i < matrix_X; i++)
            {
                Console.Write(Environment.NewLine);
                for (int j = 0; j < matrix_Y; j++)
                    Console.Write(lineMap[i, j]);
            }
        }
    }
}
