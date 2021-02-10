using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Two_Dimensional_Clustering_Tool
{
    public class Iteration
    {
        public static int NumberOfIterations;
        private int iterationid;
        public int IterationID
        {
            get { return iterationid; }
            set { iterationid = value; }
        }

        public Iteration(int ID)
        {
            this.IterationID = ID;
        }
        
        public struct ProvisionalCluster
        {
            public int ClusterID;
            public double CentroidX;
            public double CentroidY;
            public List<DataClass.DataPoint> ClusterMembers;

            public ProvisionalCluster(int clusterid, double Cx, double Cy)
            {
                this.ClusterID = clusterid;
                this.CentroidX = Cx;
                this.CentroidY = Cy;
                ClusterMembers = new List<DataClass.DataPoint>();
            }
        }

        public List<ProvisionalCluster> ClustersOfIteration = new List<ProvisionalCluster>();
    }
}
