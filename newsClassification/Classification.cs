using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newsClassification
{
    class Classification
    {
        private static double tf(int wordFreq, int[] docMax)
        {
            if (wordFreq == docMax[0])
                return wordFreq * 1.0 / docMax[1];
            return wordFreq * 1.0 / docMax[0];
        }
        private static double idf(string word, List<Dictionary<string, int>> docWordFreq)
        {
            int docCount = 0;//kelimeyi içeren döküman sayısı
            foreach (var doc in docWordFreq)
            {
                if (doc.Keys.Contains(word))
                    docCount++;
            }
            return Math.Log10(docWordFreq.Count * 1.0 / docCount);
        }
        private static double[] tf_idf_Vector(List<Dictionary<string, int>> allDocs, int[] docMax, Dictionary<string, int> corpus, Dictionary<string, int> newDoc, Dictionary<string, int> wordIndex)
        {
            double[] doc_tf_idf = new double[corpus.Count + 1];
            double mean = 0;
            foreach (KeyValuePair<string, int> wordFreq in newDoc)
            {
                if (corpus.Keys.Contains(wordFreq.Key))
                {
                    doc_tf_idf[wordIndex[wordFreq.Key]] = tf(wordFreq.Value, docMax) * idf(wordFreq.Key, allDocs);
                    mean += doc_tf_idf[wordIndex[wordFreq.Key]];
                }
            }
            doc_tf_idf[corpus.Count] = mean / corpus.Count;
            return doc_tf_idf;
        }
        private static double[][] tf_idf_Matrix(List<Dictionary<string, int>> docWordFreq, List<int[]> docMax, Dictionary<string, int> corpus, Dictionary<string, int> wordIndex)
        {
            int docCount = docWordFreq.Count;//toplam train döküman sayısı
            double[][] tf_idf_Matrix = new double[docCount][];
            int i = 0;
            foreach (var doc in docWordFreq)
            {
                tf_idf_Matrix[i] = tf_idf_Vector(docWordFreq, docMax[i], corpus, doc, wordIndex);
                i++;
            }
            return tf_idf_Matrix;
        }
        private static double Cosine(double[] doc1, double[] doc2)
        {
            double sim, den1 = 0, den2 = 0, num = 0;
            for (int i = 0; i < doc1.Length - 1; i++)
            {
                num += doc1[i] * doc2[i];
                den1 += doc1[i] * doc1[i];
                den2 += doc2[i] * doc2[i];
            }
            sim = num * 1.0 / (Math.Sqrt(den1) * Math.Sqrt(den2));
            return sim;
        }
        private static double Pearson(double[] doc1, double[] doc2, double mean1, double mean2)
        {
            //Şayet ortalama hesaplanmadıysa burda hesaplanır.
            if (mean1 == -1)
                mean1 = doc1.Average();
            if (mean2 == -1)
                mean2 = doc2.Average();
            double coef, num = 0, den1 = 0, den2 = 0;
            for (int i = 0; i < doc1.Length - 1; i++)
            {
                num += (doc1[i] - mean1) * (doc2[i] - mean2);
                den1 += Math.Pow((doc1[i] - mean1), 2);
                den2 += Math.Pow((doc2[i] - mean2), 2);
            }
            coef = num / (Math.Sqrt(den1) * Math.Sqrt(den2));
            return coef;
        }
        private static double Euler(double[] doc1, double[] doc2)
        {
            double res = 0;
            for (int i = 0; i < doc1.Length - 1; i++)
                res += Math.Pow(doc1[i] - doc2[i], 2);
            return Math.Sqrt(res);
        }
        private static double Manhattan(double[] doc1, double[] doc2)
        {
            double res = 0;
            int doc2Len = doc2.Length;
            for (int i = 0; i < doc1.Length - 1; i++)
            {
                res += Math.Abs(doc1[i] - doc2[i]);
            }
            return res;
        }
        public static int[,] kNN(int k, Tokenizer train, Tokenizer test, int distType)
        {
            double[][] train_tf_idf = tf_idf_Matrix(train.DocumentWordFreq, train.DocumentMax, train.corpus, train.wordIndex);
            double[] NN = new double[k];//ilk k tane dökümanın uzaklığını tutar
            int[] NNC = new int[k];//ilk k tane dökümanın class indexini tutar
            int j = 0;
            bool dType = false;
            double tempDist;
            double[] newDoc;
            double dist = 0;
            int classCount = train.DocumentClassIndex[train.DocumentClassIndex.Count - 1] + 1;
            int[] classes;
            int index, tempClassIndex;
            int max, chosenClass;
            int[,] res = new int[test.DocumentWordFreq.Count,2];//sonuç matrisi 0 tahmin 1 gerçek değer
            foreach (var doc in test.DocumentWordFreq)
            {
                max = 0; chosenClass = -1;
                classes = new int[classCount];
                for (int l = 0; l < k; l++)//en uygun uzaklıktaki k lıyı bulmak için atılan başlangıç değerleri
                {
                    if (distType == 0 || distType == 3)
                        NN[l] = double.MaxValue;
                    else
                        NN[l] = double.MinValue;
                    NNC[l] = -1;
                }
                newDoc = tf_idf_Vector(train.DocumentWordFreq, test.DocumentMax[j], train.corpus, doc, train.wordIndex);
                for (int i = 0; i < train.DocumentWordFreq.Count; i++)
                {
                    tempDist = 0; tempClassIndex = 0;
                    switch (distType)//seçilen uzaklık metriğine göre uzaklık hesaplanır
                    {
                        case 0:
                            dist = Euler(train_tf_idf[i], newDoc);
                            break;
                        case 1:
                            dist = Cosine(train_tf_idf[i], newDoc);
                            dType = true;
                            break;
                        case 2:
                            dist = Pearson(train_tf_idf[i], newDoc, train_tf_idf[i][train.corpus.Count], newDoc[train.corpus.Count]);
                            dType = true;
                            break;
                        case 3:
                            dist = Manhattan(train_tf_idf[i], newDoc);
                            break;
                    }
                    for (int t = 0; t < k; t++)//en optimal k elemanı bulmak için öteleme işlemleri
                    {
                        if ((!dType && NN[t] > dist) || (dType && NN[t] < dist))
                        {
                            if (tempDist != 0)
                            {
                                double tempDist2 = NN[t];
                                int tempClassIndex2 = NNC[t];
                                NN[t] = tempDist;
                                NNC[t] = tempClassIndex;
                                tempDist = tempDist2;
                                tempClassIndex = tempClassIndex2;
                            }
                            else
                            {
                                tempDist = NN[t];
                                tempClassIndex = NNC[t];
                                NN[t] = dist;
                                NNC[t] = train.DocumentClassIndex[i];
                            }
                        }
                    }
                }
                for (int t = 0; t < k; t++)//en optimali bul eğer birden fazla optimal varsa ilkini seç
                {
                    index = NNC[t];
                    classes[index]++;
                    if (classes[index] > max)
                    {
                        max = classes[index];
                        chosenClass = index;
                    }
                }
                res[j,0] = chosenClass;
                res[j,1] = test.DocumentClassIndex[j];
                j++;
            }
            return res;
        }
        public static int[,] Rocchio(Tokenizer train, Tokenizer test, int distType)
        {
            int classCount = train.DocumentClassIndex[train.DocumentClassIndex.Count - 1] + 1;
            double[][] train_tf_idf = tf_idf_Matrix(train.DocumentWordFreq, train.DocumentMax, train.corpus, train.wordIndex);
            double[][] matrix = new double[classCount][];
            int i;
            int[] classDocCount = new int[classCount];
            for (i = 0; i < classCount; i++)
                matrix[i] = new double[train.corpus.Count];
            int tempClassIndex = 0, j = 0;
            i = 0;
            foreach (var doc in train.DocumentWordFreq)//kelime tf-idf değerlerinin her class için ortalamasını buluyor
            {
                if (train.DocumentClassIndex[j] != tempClassIndex)
                {
                    for (int l = 0; l < doc.Count; l++)
                    {
                        matrix[i][l] /= classDocCount[i] * 1.0;
                    }
                    i++;
                    tempClassIndex = i;
                }
                classDocCount[i]++;
                foreach (KeyValuePair<string, int> word in doc)
                {
                    matrix[i][train.wordIndex[word.Key]] += train_tf_idf[j][train.wordIndex[word.Key]];
                }
                j++;
            }//indislemeden dolayı son classın ortalaması bu noktada hesaplanıyor
            for (int l = 0; l < train.DocumentWordFreq[train.DocumentWordFreq.Count - 1].Count; l++)
                matrix[i][l] /= classDocCount[i] * 1.0;
            double dist = 0, maxDist;
            int chosenClass;
            j = 0;
            bool dType = false;
            double[] newDoc = new double[train.corpus.Count];
            int[,] res = new int[test.DocumentWordFreq.Count,2];
            foreach (var testDoc in test.DocumentWordFreq)//uzaklık hesaplamaları
            {
                if (distType == 0 || distType == 3)
                    maxDist = double.MaxValue;
                else
                    maxDist = double.MinValue;
                chosenClass = -1;
                newDoc = tf_idf_Vector(train.DocumentWordFreq, test.DocumentMax[j], train.corpus, testDoc, train.wordIndex);
                for (int l = 0; l < classCount; l++)
                {
                    switch (distType)
                    {
                        case 0:
                            dist = Euler(matrix[l], newDoc);
                            break;
                        case 1:
                            dist = Cosine(matrix[l], newDoc);
                            dType = true;
                            break;
                        case 2:
                            dist = Pearson(matrix[l], newDoc, -1, newDoc[train.corpus.Count]);
                            dType = true;
                            break;
                        case 3:
                            dist = Manhattan(matrix[l], newDoc);
                            break;
                    }
                    if ((!dType && maxDist > dist) || (dType && maxDist < dist))//en optimal uzaklığı bul ve seç
                    {
                        maxDist = dist;
                        chosenClass = l;
                    }
                }
                res[j,0] = chosenClass;
                res[j,1] = test.DocumentClassIndex[j];
                j++;
            }
            return res;
        }
        public static int[,] Bayes(Tokenizer train, Tokenizer test)
        {
            /*
             * Olasılık hesaplanırken sayılar çarpılarak çok fazla küçüldüğünü için (1/15000)^3 gibi
             * double ın 10^-324 lük sınırına sığmadı bunu aşmak içinde bigFloat kütüphanesini kullandık
             * https://github.com/Osinko/BigFloat
             */
            int classCount = train.DocumentClassIndex[train.DocumentClassIndex.Count - 1] + 1;
            BigFloat[] ratio = new BigFloat[classCount];//her classa ait döküman sayısının tüm döküman sayısına oranı
            BigFloat[] prob = new BigFloat[classCount];
            int[] count = new int[classCount];
            Dictionary<string, int>[] classWordFreq = new Dictionary<string, int>[classCount];
            int[] classWordFreqSum = new int[classCount];
            int[,] result = new int[test.DocumentWordFreq.Count, 2];
            for (int i=0;i<classCount;i++)//atama işlemleri
            {
                ratio[i] = new BigFloat(0);
                prob[i] = new BigFloat(0);
            }
            for (int i = 0; i < train.DocumentClassIndex.Count; i++)
            {
                ratio[train.DocumentClassIndex[i]]++;
            }
            for (int i = 0; i < classCount; i++)
            {
                classWordFreq[i] = new Dictionary<string, int>();
                ratio[i] /= train.DocumentClassIndex.Count;
            }
            for (int j = 0; j < train.DocumentWordFreq.Count; j++)// her bir classın kelime frekansını bulur
            {
                foreach (KeyValuePair<string, int> pair in train.DocumentWordFreq[j])
                {
                    if (classWordFreq[train.DocumentClassIndex[j]].ContainsKey(pair.Key))
                    {
                        classWordFreq[train.DocumentClassIndex[j]][pair.Key] += pair.Value;
                    }
                    else
                    {
                        classWordFreq[train.DocumentClassIndex[j]].Add(pair.Key, pair.Value);
                    }
                    classWordFreqSum[train.DocumentClassIndex[j]] += pair.Value;//Her bir classın içindeki toplam kelime sayısı
                }
            }
            int counter = 0;
            int freq = 0;
            foreach (Dictionary<string, int> document in test.DocumentWordFreq)
            {
                for(int i=0;i<classCount;i++)//her döküman için olasılık önce orana eşitlenir
                {
                    prob[i] = ratio[i];
                }
                //ardından mn bayes formülüne göre oran kelimelerin olasılıkları ile çarpılır
                //ve dökümanın her classa ait olma olasılığı bulunur
                foreach (KeyValuePair<string, int> pair in document)
                {
                    for (int i = 0; i < classCount; i++)
                    {
                        if ((classWordFreq[i].ContainsKey(pair.Key))) freq = classWordFreq[i][pair.Key];
                        else freq = 0;
                        prob[i] *= new BigFloat(new BigFloat(freq + 1).Divide( new BigFloat(classWordFreqSum[i] + train.corpus.Count)).Pow(pair.Value));
                    }
                }
                BigFloat max = new BigFloat(-1);
                int maxclass=-1;
                for(int i=0;i<classCount;i++)//en büyük oran seçilir.Birbirine eşit oldukları durumda ilk bulduğunu seçer
                {
                   if(prob[i]>max)
                    {
                        max = prob[i];
                        maxclass = i;
                    }
                }
                result[counter, 0] = maxclass;//tahmin
                result[counter, 1] = test.DocumentClassIndex[counter];//gerçek
                counter++;
            }
            return result;
        }

    }
}
