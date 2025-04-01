using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using AbyssDorks.Models;
using AbyssDorks.Services;

namespace AbyssDorks.BiasClassifier
{
    public class NaiveBayesClassifier
    {
        private Dictionary<string, int> docCountByClass = new Dictionary<string, int>();
        private Dictionary<string, Dictionary<string, int>> wordCountsByClass = new Dictionary<string, Dictionary<string, int>>();
        private Dictionary<string, int> totalWordsInClass = new Dictionary<string, int>();
        private HashSet<string> vocabulary = new HashSet<string>();
        private int totalDocuments = 0;
        private readonly ValidationService _validationService;

        public NaiveBayesClassifier(ValidationService validationService)
        {
            _validationService = validationService;
        }

        public void Train(List<Models.Document> documents)
        {
            foreach (var doc in documents)
            {
                totalDocuments++;
                string label = doc.Label;
                if (!docCountByClass.ContainsKey(label))
                {
                    docCountByClass[label] = 0;
                    wordCountsByClass[label] = new Dictionary<string, int>();
                    totalWordsInClass[label] = 0;
                }
                docCountByClass[label]++;

                var tokens = Tokenize(doc.CombinedText);
                foreach (var token in tokens)
                {
                    vocabulary.Add(token);
                    if (!wordCountsByClass[label].ContainsKey(token))
                        wordCountsByClass[label][token] = 0;
                    wordCountsByClass[label][token]++;
                    totalWordsInClass[label]++;
                }
            }

            
        }

        private IEnumerable<string> Tokenize(string text)
        {
            text = _validationService.NormalizerTextForClassification(text);
            var words = text.Split(' ')
                .Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            List<string> tokens = new List<string>(words);
            for (int i = 0; i < words.Count - 1; i++)
            {
                string bigram = words[i] + "_" + words[i + 1];
                tokens.Add(bigram);
            }
            return tokens;
        }

        private double CalculateLogProbability(string text, string label)
        {
            double logProb = Math.Log((double)docCountByClass[label] / totalDocuments);
            var tokens = Tokenize(text);
            int vocabSize = vocabulary.Count;
            int totalTokens = totalWordsInClass[label];

            foreach (var token in tokens)
            {
                int count = wordCountsByClass[label].ContainsKey(token) ? wordCountsByClass[label][token] : 0;

                double tokenProb = (double)(count + 1) / (totalTokens + vocabSize);
                logProb += Math.Log(tokenProb);
            }
            return logProb;
        }

        public Dictionary<string, double> GetClassLogProbabilities(string text)
        {
            var result = new Dictionary<string, double>();
            foreach (var label in docCountByClass.Keys)
            {
                result[label] = CalculateLogProbability(text, label);
            }
            return result;
        }

        public string Predict(string text)
        {
            string bestLabel = null;
            double bestLogProb = double.NegativeInfinity;
            foreach (var label in docCountByClass.Keys)
            {
                double logProb = CalculateLogProbability(text, label);
                if (logProb > bestLogProb)
                {
                    bestLogProb = logProb;
                    bestLabel = label;
                }
                
            }
            return bestLabel;
        }
    }
}
