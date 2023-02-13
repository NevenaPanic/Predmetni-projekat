using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Predmetni_projekat_Nevena_Panić_PR43_2018
{
    public class PowerEntity
    {
        private long id;
        private string name;
        private double x;
        private double y;
        // converted values
        private double latitude;
        private double longitude;
        // matrix coordinates
        private int matrixX;
        private int matrixY;

        public PowerEntity()
        {

        }

        public long Id
        {
            get { return id; }

            set { id = value; }
        }

        public string Name
        {
            get { return name; }

            set { name = value; }
        }

        public double X
        {
            get { return x; }

            set { x = value; }
        }

        public double Y
        {
            get { return y; }

            set { y = value; }
        }

        public double Latitude
        {
            get { return latitude; }

            set { latitude = value; }
        }
        public double Longitude
        {
            get { return longitude; }

            set { longitude = value; }
        }

        public int MatrixX
        {
            get { return matrixX; }

            set { matrixX = value; }
        }
        public int MatrixY
        {
            get { return matrixY; }

            set { matrixY = value; }
        }
        public override string ToString()
        {
            return string.Format("ID : [{0}] \n" +
                                 "X  : [{1}] \n" +
                                 "Y  : [{2}] \n" +
                                 "Name: [{3}] \n" + 
                                 "Longitude: [{4}] \n" +
                                 "Latitude : [{5}]", id, x, y, name, longitude, latitude) ;
        }
    }
}
