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
    class Logger
    {
        private static Logger instance;
        private StreamWriter writer;

        public static Logger getLogger()
        {
            if (instance == null)
                instance = new Logger();
            return instance;
        }

        public void printResultsCSV(string method,int[,] result,List<string> labels,List<string> fileNames,double time)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.ShowDialog();
            string path = sf.FileName;
            if(!(path=="" || path == null))
            { 
                writer = new StreamWriter(path);
                writer.WriteLine("fileName,assignedClass");
                for(int i=0;i<result.GetLength(0);i++)
                {
                    writer.WriteLine(fileNames[i] + "," + labels[result[i, 0]]);
                }
                writer.WriteLine("Used method " + method + ",Time : " + ((int)(time*1000)) + " ms");
                writer.Close();
            }
        }


    }
}
