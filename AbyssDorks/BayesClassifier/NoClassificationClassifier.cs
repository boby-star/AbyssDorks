namespace AbyssDorks.BayesClassifier
{
    public class NoClassificationClassifier : IClassifier
    {
        public Task<ClassificationResult> ClassifyAsync(string text)
        {
            return Task.FromResult(new ClassificationResult
            {
                PredictedLabel = "Не оцінюється",
                LogProbabilities = new Dictionary<string, double>()
            });
        }
    }
}
