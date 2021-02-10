using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Two_Dimensional_Clustering_Tool
{
    public class ClusterClass
    {
        // declaring cluster center
        public struct ClusterCenter
        {
            public int ID;
            public double CoordX;
            public double CoordY;
        }

        // static collection
        public static List<ClusterCenter> Cluster = new List<ClusterCenter>();
        public static int NumberOfClusters;
    }
}
