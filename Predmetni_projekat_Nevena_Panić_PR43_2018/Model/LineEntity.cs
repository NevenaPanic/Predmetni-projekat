using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Predmetni_projekat_Nevena_Panić_PR43_2018
{
    public class LineEntity
    {
        private long id;
        private string name;
        private bool isUnderground;
        private float r;
        private string conductorMaterial;
        private string lineType;
        private long thermalConstantHeat;
        private long firstEnd;
        private long secondEnd;
        private double distance;
        private bool isDrawn;

        public LineEntity(){}

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

        public bool IsUnderground
        {
            get { return isUnderground; }

            set { isUnderground = value; }
        }

        public float R
        {
            get { return r; }

            set { r = value; }
        }

        public string ConductorMaterial
        {
            get { return conductorMaterial; }

            set { conductorMaterial = value; }
        }

        public string LineType
        {
            get { return lineType; }

            set { lineType = value; }
        }

        public long ThermalConstantHeat
        {
            get { return thermalConstantHeat; }

            set { thermalConstantHeat = value; }
        }

        public long FirstEnd
        {
            get { return firstEnd; }

            set { firstEnd = value; }
        }

        public long SecondEnd
        {
            get { return secondEnd; }

            set { secondEnd = value; }
        }

        public double Distance
        {
            get { return distance; }
            set { distance = value; }
        }

        public bool IsDrawn 
        {
            get { return isDrawn; }
            set { isDrawn = value; }
        }

        public override string ToString()
        {
            return string.Format("ID: {0}\n" +
                                 "Name: {1}\n" +
                                 "IsUndergound: {2}\n" +
                                 "R: {3}\n" +
                                 "Conducting material: {4}\n" +
                                 "Line type: {5}\n" +
                                 "Thermal constant heat: {6}\n" +
                                 "First end ID: {7}\n" +
                                 "Second end ID: {8}\n",
                                 this.id, this.name,  (this.isUnderground) ? "yes" : "no", this.r, this.conductorMaterial, this.lineType,
                                 this.thermalConstantHeat, this.firstEnd, this.secondEnd);
        }
    }
}
