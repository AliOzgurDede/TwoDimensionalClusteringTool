using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Two_Dimensional_Clustering_Tool
{
    public partial class kMeansClustering : Form
    {
        // declaring the collection that stores the point-cluster assignments
        Dictionary<DataClass.DataPoint, ClusterClass.ClusterCenter> Assignments = new Dictionary<DataClass.DataPoint, ClusterClass.ClusterCenter>();

        // declaring algorithm stopping variable
        public bool StopAlgorithm;

        // declaring the collection that stores iterations
        List<Iteration> Iterations = new List<Iteration>();

        public kMeansClustering()
        {
            InitializeComponent();
        }

        public void AddRowsToGrid(DataGridView dataGridView, TextBox textBox)
        {
            int rows = Int32.Parse(textBox.Text);
            for (int i = 0; i < rows; i++)
            {
                dataGridView.Rows.Add();
            }
        }

        public void PasteFromClipboard(DataGridView dataGridView)
        {
            string s = Clipboard.GetText();
            string[] lines = s.Split('\n');
            int row = dataGridView.CurrentCell.RowIndex;
            int col = dataGridView.CurrentCell.ColumnIndex;
            foreach (string line in lines)
            {
                string[] cells = line.Split('\t');
                int cellsSelected = cells.Length;
                if (row < dataGridView.Rows.Count)
                {
                    for (int i = 0; i < cellsSelected; i++)
                    {
                        if (col + i < dataGridView.Columns.Count)
                            dataGridView[col + i, row].Value = cells[i];
                        else
                            break;
                    }
                    row++;
                }
                else
                {
                    break;
                }
            }
        }

        // method for calculating euclidean distances
        public double EuclideanDistance(DataClass.DataPoint point, ClusterClass.ClusterCenter center)
        {
            double distance = 0;
            double a = point.CoordX - center.CoordX;
            double b = point.CoordY - center.CoordY;
            distance = Math.Sqrt((Math.Pow(a, 2)) + (Math.Pow(b, 2)));
            return distance;
        }

        // generating collection members from input data
        public void InitializationStep()
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1[0, i].Value != null && dataGridView1[1, i].Value != null)
                {
                    DataClass.NumberOfDataPoints = DataClass.NumberOfDataPoints + 1;
                }
            }

            for (int i = 0; i < dataGridView2.Rows.Count; i++)
            {
                if (dataGridView2[0, i].Value != null && dataGridView2[1, i].Value != null)
                {
                    ClusterClass.NumberOfClusters = ClusterClass.NumberOfClusters + 1;
                }
            }

            for (int i = 0; i < DataClass.NumberOfDataPoints; i++)
            {
                DataClass.DataPoint point;
                point.ID = i;
                point.CoordX = Double.Parse(dataGridView1[0, i].Value.ToString());
                point.CoordY = Double.Parse(dataGridView1[1, i].Value.ToString());
                DataClass.Data.Add(point);
            }

            for (int i = 0; i < ClusterClass.NumberOfClusters; i++)
            {
                ClusterClass.ClusterCenter center;
                center.ID = i;
                center.CoordX = Double.Parse(dataGridView2[0, i].Value.ToString());
                center.CoordY = Double.Parse(dataGridView2[1, i].Value.ToString());
                ClusterClass.Cluster.Add(center);
            }
        }

        // assignment step of algorithm
        public void AssignmentStep()
        {
            DataClass.NumberOfDataPoints = DataClass.Data.Count;
            ClusterClass.NumberOfClusters = ClusterClass.Cluster.Count;
            Assignments.Clear();
            List<double> distanceRecording = new List<double>();

            for (int i = 0; i < DataClass.NumberOfDataPoints; i++)
            {
                distanceRecording.Clear();

                for (int j = 0; j < ClusterClass.NumberOfClusters; j++)
                {
                    distanceRecording.Add(EuclideanDistance(DataClass.Data[i], ClusterClass.Cluster[j]));
                }

                double minimumValue = distanceRecording.Min();
                int k = distanceRecording.IndexOf(minimumValue);
                Assignments.Add(DataClass.Data[i], ClusterClass.Cluster[k]);
            }
        }

        // update step of algorithm
        public void UpdateStep()
        {
            DataClass.NumberOfDataPoints = DataClass.Data.Count;
            ClusterClass.NumberOfClusters = ClusterClass.Cluster.Count;
            int CentersNotChanged = 0;

            for (int i = 0; i < ClusterClass.NumberOfClusters; i++)
            {
                double NewCoordX = 0;
                double NewCoordY = 0;

                var ClusterMembers =
                        from a in Assignments
                        where a.Value.ID == ClusterClass.Cluster[i].ID
                        select a.Key;

                foreach (var cm in ClusterMembers)
                {
                    NewCoordX = NewCoordX + cm.CoordX;
                    NewCoordY = NewCoordY + cm.CoordY;
                }

                int ClusterSize = ClusterMembers.Count();
                NewCoordX = NewCoordX / ClusterSize;
                NewCoordY = NewCoordY / ClusterSize;

                if (NewCoordX == ClusterClass.Cluster[i].CoordX && NewCoordY == ClusterClass.Cluster[i].CoordY)
                {
                    CentersNotChanged = CentersNotChanged + 1;
                }
                else
                {
                    ClusterClass.ClusterCenter center = ClusterClass.Cluster[i];
                    center.ID = i;
                    center.CoordX = NewCoordX;
                    center.CoordY = NewCoordY;
                    ClusterClass.Cluster[i] = center;
                }
            }

            if (CentersNotChanged == ClusterClass.NumberOfClusters)
            {
                StopAlgorithm = true;
            }
            else
            {
                StopAlgorithm = false;
            }
        }

        // saving an iteration
        public void SavingIteration()
        {
            Iteration iteration = new Iteration(Iteration.NumberOfIterations);
            
            for (int i = 0; i < ClusterClass.Cluster.Count; i++)
            {
                Iteration.ProvisionalCluster provisionalCluster = new Iteration.ProvisionalCluster();
                provisionalCluster.ClusterID = ClusterClass.Cluster[i].ID;
                provisionalCluster.CentroidX = ClusterClass.Cluster[i].CoordX;
                provisionalCluster.CentroidY = ClusterClass.Cluster[i].CoordY;
                provisionalCluster.ClusterMembers = new List<DataClass.DataPoint>();

                var Members =
                        from a in Assignments
                        where a.Value.ID == ClusterClass.Cluster[i].ID
                        select a.Key;

                foreach (var member in Members)
                {
                    provisionalCluster.ClusterMembers.Add(member);
                }

                iteration.ClustersOfIteration.Add(provisionalCluster);
            }

            Iterations.Add(iteration);
        }

        // visualizing cluster members on graph
        public void VisualizingDataPoints()
        {
            for (int i = 0; i < ClusterClass.NumberOfClusters; i++)
            {
                // adding new series to graph
                System.Windows.Forms.DataVisualization.Charting.Series series = new System.Windows.Forms.DataVisualization.Charting.Series();
                this.Graph.Series.Add(series);
                series.Name = "Cluster " + (i + 1);
                series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;

                // selecting data points that belongs to cluster
                var ClusterMembers =
                        from a in Assignments
                        where a.Value.ID == ClusterClass.Cluster[i].ID
                        select a.Key;

                // adding data points to series
                foreach (var cm in ClusterMembers)
                {
                    this.Graph.Series[series.Name].Points.AddXY(cm.CoordX, cm.CoordY);
                }
            }
        }

        // creating iteration report document
        public void Report()
        {
            string filePath = @"C:\Users\User\Desktop\IterationReport.txt";
            FileStream fileStream = new FileStream(filePath, FileMode.Create);

            string report = "| ITERATION REPORT  |";
            report += "\n" + "|" + DateTime.Now.ToString() + "|";
            report += "\n" + "_____________________";

            for (int i = 0; i < Iterations.Count; i++)
            {
                report += "\n" + "Iteration " + Iterations[i].IterationID;
                report += "\n" + "---------------------";
                
                for (int j = 0; j < Iterations[i].ClustersOfIteration.Count; j++)
                {
                    report += "\n" + "Cluster " + (Iterations[i].ClustersOfIteration[j].ClusterID + 1);
                    report += "\n" + "Centroid X:" + Iterations[i].ClustersOfIteration[j].CentroidX;
                    report += "\n" + "Centroid Y:" + Iterations[i].ClustersOfIteration[j].CentroidY;
                    report += "\n" + "Member Data Points";

                    for (int k = 0; k < Iterations[i].ClustersOfIteration[j].ClusterMembers.Count; k++)
                    {
                        report += "\n";
                        report += "PointID:" + Iterations[i].ClustersOfIteration[j].ClusterMembers[k].ID;
                        report += " X:" + Iterations[i].ClustersOfIteration[j].ClusterMembers[k].CoordX;
                        report += " Y:" + Iterations[i].ClustersOfIteration[j].ClusterMembers[k].CoordY;
                    }

                    report += "\n";
                }

                report += "\n" + "_____________________";
            }

            using (StreamWriter streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.WriteLine(report);
            }
        }

        // main algorithm loop
        public void kMeansClusteringAlgorithm()
        {
            Iteration.NumberOfIterations = 0;
            InitializationStep();

            while (StopAlgorithm == false)
            {
                AssignmentStep();
                UpdateStep();
                Iteration.NumberOfIterations += 1;
                SavingIteration();
            }

            VisualizingDataPoints();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                this.WindowState = FormWindowState.Normal;
            }
            else if (tabControl1.SelectedIndex == 1)
            {
                this.WindowState = FormWindowState.Maximized;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == null)
            {
                MessageBox.Show("Missing information, number of the rows");
            }
            else
            {
                AddRowsToGrid(dataGridView1, textBox1);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == null)
            {
                MessageBox.Show("Missing information, number of the rows");
            }
            else
            {
                AddRowsToGrid(dataGridView2, textBox2);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PasteFromClipboard(dataGridView1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            PasteFromClipboard(dataGridView2);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            kMeansClustering newForm = new kMeansClustering();
            newForm.Show();
            this.Hide();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                kMeansClusteringAlgorithm();
                tabControl1.SelectedIndex = 1;
            }
            catch (Exception ex)
            {
                throw new Exception("Error! Refresh and try again");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (StopAlgorithm == true)
            {
                try
                {
                    Report();
                    MessageBox.Show("Report is generated successfully");
                }
                catch (Exception ex)
                {
                    throw new Exception("Report not found");
                }
            }
            else
            {
                MessageBox.Show("The algorithm must be executed");
            }
        }
    }
}
