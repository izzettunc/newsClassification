using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using Nuve;
using Nuve.Lang;
using Nuve.Morphologic.Structure;

namespace newsClassification
{
    class Tokenizer
    {
        public HashSet<string> stopWords;
        public Dictionary<string, int> corpus;
        public Dictionary<string, int> wordIndex;
        public List<Dictionary<string, int>> DocumentWordFreq;
        public List<int> DocumentClassIndex;
        public List<int[]> DocumentMax;
        public int Index;
        public Tokenizer(string stopWordPath)
        {
            corpus = new Dictionary<string, int>();
            stopWords = new HashSet<string>();
            DocumentWordFreq = new List<Dictionary<string, int>>();
            DocumentClassIndex = new List<int>();
            DocumentMax = new List<int[]>();
            wordIndex = new Dictionary<string, int>();
            Index = 0;
            StreamReader r = new StreamReader(stopWordPath, Encoding.GetEncoding("windows-1254"));
            string line;
            while (!r.EndOfStream)
            {
                line = r.ReadLine();
                stopWords.Add(line);
            }
        }
        public void tokenize(string path, int classIndex)
        {
            StreamReader r = new StreamReader(path, Encoding.GetEncoding("windows-1254"));
            Language tr = LanguageFactory.Create(LanguageType.Turkish);
            string line;
            string[] rawWords;
            string[] token;
            string analyzMax = "";
            int analyzMaxL = Int32.MinValue;
            string temp;
            int tempL;
            int docMax = Int32.MinValue;
            int docMax2 = Int32.MinValue;
            int analyzL;
            int tokenL;
            Dictionary<string, int> document = new Dictionary<string, int>();
            while (!r.EndOfStream)//tokenizer kısımları
            {
                line = r.ReadLine();
                rawWords = Regex.Split(line, @"((\)('|’)\w+)|\W('|’)|('|’)\W|^('|’)|$('|’)|\d+('|’)\w+|\d+\w+|\d+[^a-zA-Z ]+\w+|\w+\d+|\d+|(\)|\())|[^\w('|’)]", RegexOptions.ExplicitCapture | RegexOptions.Compiled);
                //Büyülü regex'e göre metini parçalara ayırır
                foreach (string w in rawWords)
                {
                    //boş stringler ve istenmeyen bazı durumlar da -örneğin sayılar- atılır 
                    if (w != "" && Regex.IsMatch(w, @"\D\w", RegexOptions.Compiled))
                    {
                        analyzMaxL = Int32.MinValue;
                        analyzMax = "";
                        //daha çok büyülü regex
                        token = Regex.Split(w, @"(\W*)('|’)(\w+|\W+)", RegexOptions.ExplicitCapture | RegexOptions.Compiled);
                        token[0] = token[0].ToLower();
                        IList<Word> solutions = tr.Analyze(token[0]);//morfolojik analiz
                        foreach (var solution in solutions)
                        {
                            temp = solution.GetStem().GetSurface(); //Stemming
                            tempL = temp.Length;
                            //Genel olarak köke ne kadar az yaklaşırsa metinle o kadar alakalı
                            //olduğunu tespit ettik bu yüzden en uzun stemi aldık
                            if (tempL > analyzMaxL)
                            {
                                analyzMaxL = tempL;
                                analyzMax = temp;
                            }
                        }
                        analyzL = analyzMax.Length;
                        tokenL = token[0].Length;
                        //stop words leri eliyor
                        if (analyzMax != "")
                        {
                            if (analyzL > 2 && !stopWords.Contains(analyzMax))
                            {
                                if (corpus.ContainsKey(analyzMax))
                                {
                                    corpus[analyzMax]++;
                                }
                                else
                                {
                                    corpus.Add(analyzMax, 1);
                                    wordIndex.Add(analyzMax, Index);
                                    Index++;
                                }
                                if (document.ContainsKey(analyzMax))
                                {
                                    document[analyzMax]++;
                                }
                                else
                                {
                                    document.Add(analyzMax, 1);
                                }
                            }
                        }
                        else
                        {
                            if (token[0].Length > 2 && !stopWords.Contains(token[0]))
                            {
                                if (corpus.ContainsKey(token[0]))
                                {
                                    corpus[token[0]]++;
                                }
                                else
                                {
                                    corpus.Add(token[0], 1);
                                    wordIndex.Add(token[0], Index);
                                    Index++;
                                }
                                if (document.ContainsKey(token[0]))
                                {
                                    document[token[0]]++;
                                }
                                else
                                {
                                    document.Add(token[0], 1);
                                }
                            }
                        }
                        if (analyzL > 2 && !stopWords.Contains(analyzMax))
                        {
                            if (document[analyzMax] > docMax)
                            {
                                docMax2 = docMax;
                                docMax = document[analyzMax];
                            }
                            else if (document[analyzMax] > docMax2)
                            {
                                docMax2 = document[analyzMax];
                            }
                            else
                            {
                                if (tokenL > 2 && !stopWords.Contains(token[0]) && analyzMax == "")
                                {
                                    if (document[token[0]] > docMax)
                                    {
                                        docMax2 = docMax;
                                        docMax = document[token[0]];
                                    }
                                    else if (document[token[0]] > docMax2)
                                    {
                                        docMax2 = document[token[0]];
                                    }
                                }
                            }
                        }
                    }
                }
            }
            DocumentMax.Add(new int[] { docMax, docMax2 });
            DocumentClassIndex.Add(classIndex);
            DocumentWordFreq.Add(document);
        }
    }
}
