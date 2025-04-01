namespace AbyssDorks.BayesClassifier
{
    public class ClassificationResult
    {
        public string? PredictedLabel {  get; set; }
        public Dictionary<string, double>? LogProbabilities { get; set; }
    }
}
