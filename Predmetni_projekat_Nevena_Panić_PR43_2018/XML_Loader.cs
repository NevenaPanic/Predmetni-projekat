using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Predmetni_projekat_Nevena_Panić_PR43_2018
{
    public class XML_Loader
    {
        public XML_Loader() { }
		public void LoadEntities()
		{
			// Help lists
			List<double> allXs = new List<double>();
			List<double> allYs = new List<double>();

			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load("Geographic.xml");

			XmlNodeList nodeList;
			double lat, lon;
			int counter = 0;

			// load all substations
			nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Substations/SubstationEntity");
			foreach (XmlNode node in nodeList)
			{
				SubstationEntity _sub = new SubstationEntity();
				_sub.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
				_sub.Name = node.SelectSingleNode("Name").InnerText;
				_sub.X = double.Parse(node.SelectSingleNode("X").InnerText);
				_sub.Y = double.Parse(node.SelectSingleNode("Y").InnerText);


				// const 34 is for Serbia
				ToLatLon(_sub.X, _sub.Y, 34, out lat, out lon);
				_sub.Latitude = lat;
				_sub.Longitude = lon;

				allXs.Add(lat);
				allYs.Add(lon);

				DataStorage.allEntitiesDict.Add(_sub.Id, _sub);
				counter++;
			}
			Console.WriteLine("Loaded [" + counter + "] substations.");

			counter = 0;
			// load all nodes
			nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Nodes/NodeEntity");
			foreach (XmlNode node in nodeList)
			{
				NodeEntity _node = new NodeEntity();
				_node.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
				_node.Name = node.SelectSingleNode("Name").InnerText;
				_node.X = double.Parse(node.SelectSingleNode("X").InnerText);
				_node.Y = double.Parse(node.SelectSingleNode("Y").InnerText);

				ToLatLon(_node.X, _node.Y, 34, out lat, out lon);
				_node.Latitude = lat;
				_node.Longitude = lon;

				allXs.Add(lat);
				allYs.Add(lon);

				DataStorage.allEntitiesDict.Add(_node.Id, _node);
				counter++;
			}
			Console.WriteLine("Loaded [" + counter + "] nodes.");

			counter = 0;
			// load all switches
			nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Switches/SwitchEntity");
			foreach (XmlNode node in nodeList)
			{
				SwitchEntity _switch = new SwitchEntity();
				_switch.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
				_switch.Name = node.SelectSingleNode("Name").InnerText;
				_switch.X = double.Parse(node.SelectSingleNode("X").InnerText);
				_switch.Y = double.Parse(node.SelectSingleNode("Y").InnerText);
				_switch.Status = node.SelectSingleNode("Status").InnerText;

				ToLatLon(_switch.X, _switch.Y, 34, out lat, out lon);

				_switch.Latitude = lat;
				_switch.Longitude = lon;

				allXs.Add(lat);
				allYs.Add(lon);

				DataStorage.allEntitiesDict.Add(_switch.Id, _switch);
				counter++;
			}
			Console.WriteLine("Loaded [" + counter + "] switches.");

			// Scaling values for my canvas
			DataStorage.minLatitude = DataStorage.allEntitiesDict.Values.Min(x => x.Latitude);
			DataStorage.maxLatitude = DataStorage.allEntitiesDict.Values.Max(x => x.Latitude);
			DataStorage.minLongitude = DataStorage.allEntitiesDict.Values.Min(x => x.Longitude);
			DataStorage.maxLongitude = DataStorage.allEntitiesDict.Values.Max(x => x.Longitude);

			allXs.Clear();
			allYs.Clear();

			foreach (PowerEntity entity in DataStorage.allEntitiesDict.Values)
			{
				Scale(entity);
				//.WriteLine($"New Canvas coordinates : [" + entity.ScaledX + ", " + entity.ScaledY + "]");
			}

			// load all lines, ignore vertices 
			nodeList = xmlDoc.DocumentElement.SelectNodes("/NetworkModel/Lines/LineEntity");
			foreach (XmlNode node in nodeList)
			{
				LineEntity _line = new LineEntity();
				_line.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
				_line.Name = node.SelectSingleNode("Name").InnerText;
				_line.IsUnderground = bool.Parse(node.SelectSingleNode("IsUnderground").InnerText);
				_line.R = float.Parse(node.SelectSingleNode("R").InnerText);
				_line.ConductorMaterial = node.SelectSingleNode("ConductorMaterial").InnerText;
				_line.LineType = node.SelectSingleNode("LineType").InnerText;
				_line.FirstEnd = long.Parse(node.SelectSingleNode("FirstEnd").InnerText);
				_line.SecondEnd = long.Parse(node.SelectSingleNode("SecondEnd").InnerText);
				_line.IsDrawn = false;


				if (DataStorage.allEntitiesDict.ContainsKey(_line.FirstEnd) && DataStorage.allEntitiesDict.ContainsKey(_line.SecondEnd))
				{
					_line.Distance = GetDistance(DataStorage.allEntitiesDict[_line.FirstEnd], DataStorage.allEntitiesDict[_line.SecondEnd]);
					DataStorage.lines.Add(_line);
				}
			}
			Console.WriteLine("Loaded [" + DataStorage.lines.Count + "] lines.");

			// all lines sorted by distance
			DataStorage.lines = DataStorage.lines.OrderBy(x => x.Distance).ToList();
			//DataStorage.lines.Reverse();
		}


		private double GetDistance(PowerEntity source, PowerEntity destination)
		{
			return Math.Sqrt(Math.Pow( (source.MatrixX - destination.MatrixX), 2) + Math.Pow((source.MatrixY - destination.MatrixY), 2));
		}

		public static void Scale(PowerEntity entity)
		{
			// max positions in  matrix = size - 1
			double maxMatrixSizeX = DataStorage.matrix_X - 1;
			double maxMatrixSizeY = DataStorage.matrix_Y - 1;

			// this is the coordinates of this object on my custom canvas
			// scaling function:
			//        (b-a)(x - min)
			// f(x) = --------------  + a     a = 0, b = matrix_X or matrix_Y
			//          max - min
			entity.MatrixY = (int)(Math.Floor(maxMatrixSizeY - (maxMatrixSizeY * (entity.Latitude - DataStorage.minLatitude)) / (DataStorage.maxLatitude - DataStorage.minLatitude)));
			entity.MatrixX = (int)(Math.Floor((maxMatrixSizeX * (entity.Longitude - DataStorage.minLongitude)) / (DataStorage.maxLongitude - DataStorage.minLongitude)));

			if (DataStorage.entities[entity.MatrixX, entity.MatrixY] == null)
			{
				DataStorage.entities[entity.MatrixX, entity.MatrixY] = entity;
			}
			else
			{
				FindFirstClosestPosition(entity);
			}
		}

		private static void FindFirstClosestPosition(PowerEntity entity)
		{
			bool found_best_fit = false;
			int step = 1;
			int upper, lower, left, right;

			while (!found_best_fit)
			{
				for (int i = entity.MatrixX - step; i <= entity.MatrixX + step; i++)
                {
                    if (i < 0 || i == DataStorage.matrix_X)
                        break;
                    upper = (entity.MatrixY - step < 0) ? 0 : entity.MatrixY - step;
					lower = (entity.MatrixY + step >= DataStorage.matrix_Y - 1) ? DataStorage.matrix_Y - 1 : entity.MatrixY + step;

					if (DataStorage.entities[i, upper] == null && !found_best_fit)
					{
						DataStorage.entities[i, upper] = entity;
						entity.MatrixX = i;
						entity.MatrixY = upper;
						found_best_fit = true;
						break;
					}
					else if (DataStorage.entities[i, lower] == null && !found_best_fit)
					{
						DataStorage.entities[i, lower] = entity;
						entity.MatrixX = i;
						entity.MatrixY = lower;
						found_best_fit = true;
						break;
					}
				}

				if (!found_best_fit)
				{
					for (int j = entity.MatrixY - step + 1; j < entity.MatrixY + step; j++)
					{
                        if (j < 0 || j == DataStorage.matrix_Y)
                            break;
                        right = (entity.MatrixX + step >= DataStorage.matrix_X) ? DataStorage.matrix_X - 1 : entity.MatrixX + step;
						left = (entity.MatrixX - step < 0) ? 0 : entity.MatrixX - step;

						if (DataStorage.entities[right, j] == null && !found_best_fit)
						{
							DataStorage.entities[right, j] = entity;
							entity.MatrixX = right;
							entity.MatrixY = j;
							found_best_fit = true;
							break;
						}
						else if (DataStorage.entities[left, j] == null && !found_best_fit)
						{
							DataStorage.entities[left, j] = entity;
							entity.MatrixX = left;
							entity.MatrixY = j;
							found_best_fit = true;
							break;
						}
					}
				}
				step++;
			}
		}

		//From UTM to Latitude and longitude in decimal
		public static void ToLatLon(double utmX, double utmY, int zoneUTM, out double latitude, out double longitude)
		{
			bool isNorthHemisphere = true;

			var diflat = -0.00066286966871111111111111111111111111;
			var diflon = -0.0003868060578;

			var zone = zoneUTM;
			var c_sa = 6378137.000000;
			var c_sb = 6356752.314245;
			var e2 = Math.Pow((Math.Pow(c_sa, 2) - Math.Pow(c_sb, 2)), 0.5) / c_sb;
			var e2cuadrada = Math.Pow(e2, 2);
			var c = Math.Pow(c_sa, 2) / c_sb;
			var x = utmX - 500000;
			var y = isNorthHemisphere ? utmY : utmY - 10000000;

			var s = ((zone * 6.0) - 183.0);
			var lat = y / (c_sa * 0.9996);
			var v = (c / Math.Pow(1 + (e2cuadrada * Math.Pow(Math.Cos(lat), 2)), 0.5)) * 0.9996;
			var a = x / v;
			var a1 = Math.Sin(2 * lat);
			var a2 = a1 * Math.Pow((Math.Cos(lat)), 2);
			var j2 = lat + (a1 / 2.0);
			var j4 = ((3 * j2) + a2) / 4.0;
			var j6 = ((5 * j4) + Math.Pow(a2 * (Math.Cos(lat)), 2)) / 3.0;
			var alfa = (3.0 / 4.0) * e2cuadrada;
			var beta = (5.0 / 3.0) * Math.Pow(alfa, 2);
			var gama = (35.0 / 27.0) * Math.Pow(alfa, 3);
			var bm = 0.9996 * c * (lat - alfa * j2 + beta * j4 - gama * j6);
			var b = (y - bm) / v;
			var epsi = ((e2cuadrada * Math.Pow(a, 2)) / 2.0) * Math.Pow((Math.Cos(lat)), 2);
			var eps = a * (1 - (epsi / 3.0));
			var nab = (b * (1 - epsi)) + lat;
			var senoheps = (Math.Exp(eps) - Math.Exp(-eps)) / 2.0;
			var delt = Math.Atan(senoheps / (Math.Cos(nab)));
			var tao = Math.Atan(Math.Cos(delt) * Math.Tan(nab));

			longitude = ((delt * (180.0 / Math.PI)) + s) + diflon;
			latitude = ((lat + (1 + e2cuadrada * Math.Pow(Math.Cos(lat), 2) - (3.0 / 2.0) * e2cuadrada * Math.Sin(lat) * Math.Cos(lat) * (tao - lat)) * (tao - lat)) * (180.0 / Math.PI)) + diflat;
		}
	}
}
