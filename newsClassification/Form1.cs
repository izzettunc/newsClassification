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
    public partial class Form1 : Form
    {
        bool readTest = false, readTrain = false, isUnlabeled = false;
        string trainPath, oldTrainPath, testPath, oldTestPath, stopWordPath, testFileName;
        Visualizer tabV;
        Tokenizer train, test;
        List<string> labels;
        List<string> fileNames;

        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.ShowDialog();
            if (fd.SelectedPath == "") readTrain = false;
            else readTrain = true;
            TrainPath.Text = Path.GetFileName(fd.SelectedPath);
            trainPath = fd.SelectedPath;
            if (readTest && readTrain) startButton.Enabled = true;
            else startButton.Enabled = false;
            button4.Enabled = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            tabV.show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.ShowDialog();
            if (fd.SelectedPath == "") readTest = false;
            else readTest = true;
            TestPath.Text = Path.GetFileName(fd.SelectedPath);
            testPath = fd.SelectedPath;
            testFileName = TestPath.Text;
            if (readTest && readTrain) startButton.Enabled = true;
            else startButton.Enabled = false;
            button4.Enabled = false;
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            isUnlabeled = !isUnlabeled;
            if (isUnlabeled) label2.Text = "Unlabeled Data :";
            else label2.Text = "Test File :";
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" && textBox1.Visible)
            {
                MessageBox.Show("Please enter value of K", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!(checkBox1.Checked || checkBox2.Checked || checkBox3.Checked))
            {
                MessageBox.Show("Please select a classification method", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (stopWordPath==null || stopWordPath=="")
            {
                MessageBox.Show("Please select a file for stop words.If you don't want filter stop words please select a empty txt file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Console.Clear();
                Stopwatch timer = new Stopwatch();

                double[] overall = new double[2];
                tabV = new TabVisualizer();
                tabV.clearCanvas();
                string[] filePaths;

                string[] directories;
                int classIndex;

                if (trainPath != oldTrainPath)
                {
                    train = new Tokenizer(stopWordPath);
                    directories = Directory.GetDirectories(trainPath);
                    classIndex = 0;
                    Console.WriteLine("Tokenizing the train files");
                    labels = new List<string>();
                    foreach (string directory in directories)
                    {
                        labels.Add(Path.GetFileName(directory));
                        filePaths = Directory.GetFiles(directory);
                        foreach (string filePath in filePaths)
                        {
                            train.tokenize(filePath, classIndex);
                        }
                        classIndex++;
                    }
                    Console.WriteLine("Finished");
                    oldTrainPath = trainPath;
                }

                if (testPath != oldTestPath)
                {
                    test = new Tokenizer(stopWordPath);
                    fileNames = new List<string>();
                    classIndex = 0;
                    directories = Directory.GetDirectories(testPath);
                    Console.WriteLine("Tokenizing the test files");
                    foreach (string directory in directories)
                    {
                        filePaths = Directory.GetFiles(directory);
                        foreach (string filePath in filePaths)
                        {
                            fileNames.Add(Path.GetFileName(filePath));
                            test.tokenize(filePath, classIndex);
                        }
                        classIndex++;
                    }
                    Console.WriteLine("Finished");
                    oldTestPath = testPath;
                }

                int[,] result = new int[test.DocumentWordFreq.Count, 2];
                int classCount = train.DocumentClassIndex[train.DocumentClassIndex.Count - 1] + 1;

                double[][,] confusionMatrices = new double[classCount][,];
                for (int i = 0; i < classCount; i++)
                    confusionMatrices[i] = new double[2, 2];
                double[,] performanceMatrix = new double[3, classCount];

                if (checkBox1.Checked)
                {//naive
                    Console.WriteLine("Classification started, using Multinomial Naive Bayes");
                    timer.Reset();
                    timer.Start();
                    result = Classification.Bayes(train, test);
                    timer.Stop();
                    double time = (double)timer.ElapsedMilliseconds / 1000;
                    if (!isUnlabeled)
                    {
                        overall[0] = Metrics.Accuracy(result);
                        overall[1] = time;
                        for (int i = 0; i < classCount; i++)
                        {
                            confusionMatrices[i] = Metrics.confusionMatrix(result, i);
                        }
                        performanceMatrix = Metrics.performanceMatrix(confusionMatrices, classCount);
                        tabV.addNewTrainTestResult("MN Bayes", labels, confusionMatrices, performanceMatrix, overall);
                    }
                    else
                        tabV.addRealResult("MN Bayes", labels, fileNames, result);
                    Logger L = Logger.getLogger();
                    L.printResultsCSV("MN Bayes", result, labels, fileNames, time);
                    Console.WriteLine("Finished");
                }
                if (checkBox2.Checked)
                {//knn
                    Console.WriteLine("Classification started, using kNN with " + comboBox1.SelectedItem.ToString() + " and k equals to " + int.Parse(textBox1.Text));
                    timer.Reset();
                    timer.Start();
                    result = Classification.kNN(int.Parse(textBox1.Text), train, test, comboBox1.SelectedIndex);
                    timer.Stop();
                    double time = (double)timer.ElapsedMilliseconds / 1000;
                    if (!isUnlabeled)
                    {
                        overall[0] = Metrics.Accuracy(result);
                        overall[1] = time;
                        for (int i = 0; i < classCount; i++)
                        {
                            confusionMatrices[i] = Metrics.confusionMatrix(result, i);
                        }
                        performanceMatrix = Metrics.performanceMatrix(confusionMatrices, classCount);
                        tabV.addNewTrainTestResult("K-NN", labels, confusionMatrices, performanceMatrix, overall);
                    }
                    else
                        tabV.addRealResult("K-NN", labels, fileNames, result);
                    Logger L = Logger.getLogger();
                    L.printResultsCSV("K-NN", result, labels, fileNames, time);
                    Console.WriteLine("Finished");
                }
                if (checkBox3.Checked)
                {//rocchio
                    Console.WriteLine("Classification started, using Rocchio with " + comboBox1.SelectedItem.ToString());
                    timer.Reset();
                    timer.Start();
                    result = Classification.Rocchio(train, test, comboBox1.SelectedIndex);
                    timer.Stop();
                    double time = (double)timer.ElapsedMilliseconds / 1000;
                    if (!isUnlabeled)
                    {
                        overall[0] = Metrics.Accuracy(result);
                        overall[1] = time;
                        for (int i = 0; i < classCount; i++)
                        {
                            confusionMatrices[i] = Metrics.confusionMatrix(result, i);
                        }
                        performanceMatrix = Metrics.performanceMatrix(confusionMatrices, classCount);
                        tabV.addNewTrainTestResult("Rocchio", labels, confusionMatrices, performanceMatrix, overall);
                    }
                    else
                        tabV.addRealResult("Rocchio", labels, fileNames, result);
                    Logger L = Logger.getLogger();
                    L.printResultsCSV("Rocchio", result, labels, fileNames, time);
                    Console.WriteLine("Finished");
                }
                tabV.show();
                button4.Enabled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.ShowDialog();
            stopWordPath = fd.FileName;
            stopWordsPath.Text = fd.SafeFileName;
            button4.Enabled = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox1.Text, out int result) && textBox1.Text != "")
            {
                MessageBox.Show("Value of K only can be an integer.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Text = "";
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox2.Checked)
            {
                comboBox1.Visible = !comboBox1.Visible;
                label4.Visible = !label4.Visible;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 2)
            {
                MessageBox.Show("If you use pearson on kNN it will take a bit time to compute.(more than minute probably)", "Beware", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox3.Checked)
            {
                comboBox1.Visible = !comboBox1.Visible;
                label4.Visible = !label4.Visible;
            }
            label6.Visible = !label6.Visible;
            textBox1.Visible = !textBox1.Visible;
        }
    }
}
