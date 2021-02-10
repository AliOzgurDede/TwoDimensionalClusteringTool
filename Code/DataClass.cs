using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Two_Dimensional_Clustering_Tool
{
    public class DataClass
    {
        // declaring data point
        public struct DataPoint
        {
            public int ID;
            public double CoordX;
            public double CoordY;
        }

        // static collection
        public static List<DataPoint> Data = new List<DataPoint>();
        public static int NumberOfDataPoints;
    }
}
