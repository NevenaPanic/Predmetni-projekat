using Predmetni_projekat_Nevena_Panić_PR43_2018.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Predmetni_projekat_Nevena_Panić_PR43_2018
{
    public static class BFS_Algorithm
    {

        public static Dictionary<long, List<MatrixPosition>> Search(bool excludeNodes)
        {
            Dictionary<long, List<MatrixPosition>> bfsPathLines = new Dictionary<long, List<MatrixPosition>>();

            DataStorage.lineMap = new char[DataStorage.matrix_X, DataStorage.matrix_Y];
            DataStorage.InitPathMap(DataStorage.lineMap);

            int foundBFS = 0;
            int notFound = 0;
            int secondPassBfs = 0;
            
            List<LineEntity> lines = new List<LineEntity>();

            if (excludeNodes)
                lines = DataStorage.lines.FindAll(x => !(DataStorage.allEntitiesDict[x.FirstEnd] is NodeEntity));
            else
                lines = DataStorage.lines;

            foreach (LineEntity line in lines)   // 2223 lines
            {
                MatrixPosition s = new MatrixPosition(DataStorage.allEntitiesDict[line.FirstEnd].MatrixX, DataStorage.allEntitiesDict[line.FirstEnd].MatrixY );
                MatrixPosition d = new MatrixPosition(DataStorage.allEntitiesDict[line.SecondEnd].MatrixX, DataStorage.allEntitiesDict[line.SecondEnd].MatrixY );

                bool res = FindPath(s, d, DataStorage.lineMap);
                if (res)
                {
                    int index = DataStorage.lines.IndexOf(line);
                    DataStorage.lines[index].IsDrawn = true;
                    bfsPathLines.Add(line.Id, ReconstructPath(s, d, false));
                    foundBFS++;
                }
                else
                    notFound++;
            }

            if (excludeNodes)
                lines = DataStorage.lines.FindAll(x => !(DataStorage.allEntitiesDict[x.FirstEnd] is NodeEntity) && !x.IsDrawn);
            else
                lines = DataStorage.lines.FindAll(x => !x.IsDrawn);
            char[,] falseMap = new char[DataStorage.matrix_X, DataStorage.matrix_Y];
            DataStorage.InitPathMap(falseMap);

            foreach (LineEntity line in lines)   // 2223 lines
            {
                MatrixPosition s = new MatrixPosition(DataStorage.allEntitiesDict[line.FirstEnd].MatrixX, DataStorage.allEntitiesDict[line.FirstEnd].MatrixY);
                MatrixPosition d = new MatrixPosition(DataStorage.allEntitiesDict[line.SecondEnd].MatrixX, DataStorage.allEntitiesDict[line.SecondEnd].MatrixY);

                bool res = FindPath(s, d, falseMap);
                if (res)
                {
                    int index = DataStorage.lines.IndexOf(line);
                    DataStorage.lines[index].IsDrawn = true;
                    bfsPathLines.Add(line.Id, ReconstructPath(s, d, true));
                    secondPassBfs++;
                }
            }

            //DataStorage.PrintLineMap();
            Console.WriteLine($"\n{foundBFS} found lines using bfs");
            Console.WriteLine($"\n{notFound} not found lines using bfs");
            Console.WriteLine($"\n{secondPassBfs} found lines using bfs second time");

            return bfsPathLines;
        }

        public static bool FindPath(MatrixPosition source, MatrixPosition destination, char[,] lineMap)
        {
            // queues for matrix
            Queue<MatrixPosition> q_source = new Queue<MatrixPosition>();
            Queue<MatrixPosition> q_destination = new Queue<MatrixPosition>();


            q_source.Enqueue(source);
            // init matrix for visited cells
            DataStorage.visited_s = new bool[DataStorage.matrix_X, DataStorage.matrix_Y];
            DataStorage.visited_s[source.X, source.Y] = true;
            DataStorage.parentCell_s = new MatrixPosition[DataStorage.matrix_X, DataStorage.matrix_Y];

            q_destination.Enqueue(destination);
            DataStorage.visited_d = new bool[DataStorage.matrix_X, DataStorage.matrix_Y];
            DataStorage.visited_d[destination.X, destination.Y] = true;
            DataStorage.parentCell_d = new MatrixPosition[DataStorage.matrix_X, DataStorage.matrix_Y];

            bool found;

            while (q_source.Count != 0 && q_destination.Count != 0)
            {
                found = BFS_Step(q_source, destination, DataStorage.parentCell_s, DataStorage.visited_s, DataStorage.visited_d, lineMap);
                if (found)
                    return true;

                found = BFS_Step(q_destination, source, DataStorage.parentCell_d, DataStorage.visited_d, DataStorage.visited_s, lineMap);
                if (found)
                    return true;
            }
            return false;
        }


        public static bool BFS_Step(Queue<MatrixPosition> queue, MatrixPosition target, MatrixPosition[,] parentCell, bool[,] visited, bool[,] visited_target, char[,] lineMap)
        {
            MatrixPosition currentCell;
            List<MatrixPosition> neighbours;

            if (queue.Count > 0)
            {
                currentCell = queue.Dequeue();
                if (currentCell.Equals(target) || visited_target[currentCell.X, currentCell.Y])
                {
                    DataStorage.intersection = currentCell;
                    return true;
                }

                neighbours = GetNeighbours(currentCell);
                foreach (MatrixPosition neighbour in neighbours)
                {
                    if (IsValidCell(neighbour, visited, lineMap))
                    {
                        queue.Enqueue(neighbour);
                        parentCell[neighbour.X, neighbour.Y] = currentCell;
                        visited[neighbour.X, neighbour.Y] = true;
                    }
                }
            }

            return false;
        }

        public static List<MatrixPosition> ReconstructPath(MatrixPosition source, MatrixPosition destination, bool second_pass)
        {
            List<MatrixPosition> path = new List<MatrixPosition>();
            MatrixPosition intersection = DataStorage.intersection;

            MatrixPosition currentPoint = intersection;

            while (currentPoint != source & currentPoint != null)
            {
                path.Add(currentPoint);
                currentPoint = DataStorage.parentCell_s[currentPoint.X, currentPoint.Y];
            }
            path.Add(source);
            path.Reverse();

            currentPoint = DataStorage.parentCell_d[intersection.X, intersection.Y];
            while (currentPoint != destination && currentPoint != null)
            {
                path.Add(currentPoint);
                currentPoint = DataStorage.parentCell_d[currentPoint.X, currentPoint.Y];
            }
            path.Add(destination);

            if (!second_pass)
            {
                for (int i = 1; i < path.Count - 1; i++)
                    DataStorage.lineMap[path[i].X, path[i].Y] = 'x';
            }
            else
            {
                for (int i = 1; i < path.Count - 1; i++)
                {
                    if (DataStorage.lineMap[path[i].X, path[i].Y] == 'x')
                        DataStorage.crossPositions.Add(path[i]);
                    else
                        DataStorage.lineMap[path[i].X, path[i].Y] = 'x';
                }
            }

            return path;
        }


        /*
        public static bool FindPath(MatrixPosition source, MatrixPosition destination)
        {
            Queue<MatrixPosition> q = new Queue<MatrixPosition>();
            q.Enqueue(source);

            // init matrix for visited cells
            DataStorage.visited = new bool[DataStorage.matrix_X, DataStorage.matrix_Y];
            DataStorage.visited[source.X, source.Y] = true;

            // all cells init to null
            DataStorage.parentCell = new MatrixPosition[DataStorage.matrix_X, DataStorage.matrix_Y];

            MatrixPosition currentCell;
            List<MatrixPosition> neighbours;

            while (q.Count > 0) 
            {
                currentCell = q.Dequeue();
                if (currentCell.Equals(destination))
                    return true;

                neighbours = GetNeighbours(currentCell);
                foreach (MatrixPosition neighbour in neighbours)
                {
                    if (IsValidCell(neighbour))
                    {
                        q.Enqueue(neighbour);
                        DataStorage.parentCell[neighbour.X, neighbour.Y] = currentCell;
                        DataStorage.visited[neighbour.X, neighbour.Y] = true;
                    }
                }
            }

            return false;
        }

        public static List<MatrixPosition> ReconstructPath(MatrixPosition start, MatrixPosition destination)
        {
            List<MatrixPosition> path = new List<MatrixPosition>();
            path.Add(destination);
            MatrixPosition currentPoint = DataStorage.parentCell[destination.X, destination.Y];    // set it to end

            while (currentPoint != start)
            {
                path.Add(currentPoint);
                DataStorage.lineMap[currentPoint.X, currentPoint.Y] = 'x';
                currentPoint = DataStorage.parentCell[currentPoint.X, currentPoint.Y];
            }
            path.Add(start);

            path.Reverse();

            return path;
        }
        */

        public static List<MatrixPosition> GetNeighbours(MatrixPosition cell) 
        {
            List<MatrixPosition> neighbours = new List<MatrixPosition>();

            neighbours.Add( new MatrixPosition(cell.X - 1, cell.Y));   // left
            neighbours.Add( new MatrixPosition(cell.X + 1, cell.Y));   // right
            neighbours.Add( new MatrixPosition(cell.X, cell.Y + 1));   // top
            neighbours.Add( new MatrixPosition(cell.X, cell.Y - 1));   // bottom

            return neighbours;
        }

        public static bool IsValidCell(MatrixPosition cell, bool[,] visited, char[,] lineMap)
        {
            if (cell.X >= 0 && cell.X < DataStorage.matrix_X && cell.Y >= 0 && cell.Y < DataStorage.matrix_Y &&         // check valid index
                !visited[cell.X, cell.Y] && lineMap[cell.X, cell.Y] == 'o')                     // check fields
                return true;
            else
                return false;
        }
    }
}
