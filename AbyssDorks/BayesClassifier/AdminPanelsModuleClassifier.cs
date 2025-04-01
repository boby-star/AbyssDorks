using AbyssDorks.BiasClassifier;
using AbyssDorks.Services;

namespace AbyssDorks.BayesClassifier
{
    public class AdminPanelsModuleClassifier : IClassifier
    {
        private readonly NaiveBayesClassifier _classifier;
        private readonly ValidationService _validationService;

        public AdminPanelsModuleClassifier(NaiveBayesClassifier classifier, ValidationService validationService)
        {
            _classifier = classifier;
            _validationService = validationService;
        }

        public Task<ClassificationResult> ClassifyAsync(string text)
        {
            string cleanedText = _validationService.NormalizerTextForClassification(text);
            var logProbs = _classifier.GetClassLogProbabilities(cleanedText);
            string predictedLabel = _classifier.Predict(cleanedText);
            return Task.FromResult(new ClassificationResult
            {
                PredictedLabel = predictedLabel,
                LogProbabilities = logProbs
            });
        }
    }

}
