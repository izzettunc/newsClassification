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
using System.Diagnostics;

namespace newsClassification
{
    interface Visualizer
    {
        void addNewTrainTestResult(string method, List<string> labels, double[][,] confusionMatrix, double[,] performanceMatrix, double[] overall);
        void addRealResult(string method, List<string> labels, List<string> fileNames, int[,] results);
        void clearCanvas();
        void show();

    }

    class TabVisualizer : Visualizer
    {
        private TabControl metrics;
        private TabControl confusionMatrices;
        private DataGridView overall;
        private Form newWindow;

        public TabVisualizer()
        {
            this.newWindow = new Form();
            this.metrics = new TabControl();
            this.confusionMatrices = new TabControl();
            this.overall = new DataGridView();
            this.newWindow.FormClosing += NewWindow_FormClosing;
        }

        private void NewWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            ((Form)sender).Hide();
        }

        public void addNewTrainTestResult(string method, List<string> labels, double[][,] confusionMatrix, double[,] performanceMatrix, double[] overall)
        {
            TabPage newMetricTP = new TabPage(method);
            TabPage newConfusionTP = new TabPage(method);
            TabControl newConfusionTC = new TabControl();
            DataGridView metricView = new DataGridView();
            List<TabPage> newConfusionTPTPlist = new List<TabPage>();

            for (int i = 0; i < labels.Count; i++)
            {
                TabPage temp = new TabPage(labels[i]);
                DataGridView tempView = new DataGridView();
                tempView.Height = 91;
                tempView.Width = 428;
                tempView.Location = new Point(6, 6);
                tempView.Columns.Add("whitespace", " ");
                tempView.Columns.Add("Positive", "Positive");
                tempView.Columns.Add("Negative", "Negative");
                tempView.Rows.Add("Positive");
                tempView.Rows.Add("Negative");
                for (int k = 0; k < 2; k++)
                {
                    for (int l = 0; l < 2; l++)
                    {
                        tempView[k + 1, l].Value = confusionMatrix[i][k, l];
                    }
                }
                temp.Name = labels[i];
                temp.Text = labels[i];
                temp.Controls.Add(tempView);
                newConfusionTC.TabPages.Add(temp);
            }

            newMetricTP.Name = method;
            newConfusionTP.Name = method;

            newMetricTP.Text = method;
            newConfusionTP.Text = method;

            metricView.Location = new Point(6, 6);
            newConfusionTC.Location = new Point(0, 0);
            this.metrics.Location = new Point(10, 10);
            this.confusionMatrices.Location = new Point(511, 170);
            this.overall.Location = new Point(10, 170);

            this.metrics.Height = 150;
            this.confusionMatrices.Height = 150;
            this.overall.Height = 150;
            this.newWindow.Height = 375;
            metricView.Height = 113;
            newConfusionTC.Height = 125;

            metricView.Width = 950;
            newConfusionTC.Width = 448;
            this.newWindow.Width = 1000;
            this.metrics.Width = 970;
            this.overall.Width = 491;
            this.confusionMatrices.Width = 468;


            metricView.Columns.Add("whitespace", " ");
            foreach (string label in labels)
            {
                metricView.Columns.Add(label, label);
            }
            metricView.Rows.Add("Precision");
            metricView.Rows.Add("Recall");
            metricView.Rows.Add("Fscore");

            for (int i = 0; i < performanceMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < labels.Count; j++)
                {
                    metricView[j + 1, i].Value = performanceMatrix[i, j];
                }
            }

            if (this.overall.Columns.Count == 0)
            {
                this.overall.Columns.Add("Metric", "Metric");
                this.overall.Rows.Add("Accuracy");
                this.overall.Rows.Add("Time");
            }
            this.overall.Columns.Add(method, method);
            for (int i = 0; i < 2; i++)
            {
                this.overall[this.overall.Columns.Count - 1, i].Value = overall[i];
            }

            metrics.TabPages.Add(newMetricTP);
            metrics.TabPages[method].Controls.Add(metricView);
            newConfusionTP.Controls.Add(newConfusionTC);
            confusionMatrices.TabPages.Add(newConfusionTP);
            this.newWindow.Controls.Add(metrics);
            this.newWindow.Controls.Add(confusionMatrices);
            this.newWindow.Controls.Add(this.overall);
        }

        public void addRealResult(string method, List<string> labels, List<string> fileNames, int[,] results)
        {
            TabPage newMetricTP = new TabPage(method);
            newMetricTP.Name = method;
            newMetricTP.Text = method;

            DataGridView metricView = new DataGridView();

            metricView.Location = new Point(6, 6);
            this.metrics.Location = new Point(10, 10);

            metricView.Height = 263;
            this.metrics.Height = 300;
            this.newWindow.Height = 375;

            metricView.Width = 950;
            this.newWindow.Width = 1000;
            this.metrics.Width = 970;

            metricView.Columns.Add("whitespace", " ");
            metricView.Columns.Add("Assigned Class", "Assigned Class");
            for (int i = 0; i < results.GetLength(0); i++)
                metricView.Rows.Add(fileNames[i], labels[results[i, 0]]);

            metrics.TabPages.Add(newMetricTP);
            metrics.TabPages[method].Controls.Add(metricView);
            this.newWindow.Controls.Add(metrics);

        }

        public void clearCanvas()
        {
            this.newWindow.Controls.Clear();
            this.metrics.Controls.Clear();
            this.confusionMatrices.TabPages.Clear();
            this.overall.Rows.Clear();
            this.overall.Columns.Clear();
        }

        public void show()
        {
            this.newWindow.Show();
        }


    }
}
