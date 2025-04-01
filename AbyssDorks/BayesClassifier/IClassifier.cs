namespace AbyssDorks.BayesClassifier
{
    public interface IClassifier
    {
        Task<ClassificationResult> ClassifyAsync(string text);
    }
}
