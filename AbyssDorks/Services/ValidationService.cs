using AbyssDorks.Models;
using System.Text.RegularExpressions;

namespace AbyssDorks.Services
{
    public class ValidationService
    {
        private readonly JsonDataService _jsonDataService;

        public ValidationService(JsonDataService jsonDataService)
        {
            _jsonDataService = jsonDataService;
        }

        public bool ValidateOperators(string searchQuery)
        {
            var operators = _jsonDataService.LoadOperators();
            var operatorNames = operators.Select(op => op.Name).ToList();

            var regex = new Regex(@"(\w+:)");
            var matches = regex.Matches(searchQuery);

            foreach ( Match match in matches )
            {
                var operatorName = match.Value.TrimEnd(':');
                if (!operatorNames.Contains(operatorName) )
                {
                    return false;
                }
            }
            return true;
        }

        public bool ValidateSyntax(string searchQuery)
        {
            if (searchQuery.Contains("\"\"") || searchQuery.Contains("::"))
                return false;

            return true;
        }

        public bool ValidateParameters(Template template, Dictionary<string, string> parameters)
        {
            foreach (var param in parameters.Keys)
            {
                if (!template.TemplateText.Contains($"{{{param}}}"))
                {
                    return false;
                }
            }
            return true;
        }

        public string NormalizerTextForClassification(string input)
        {
            input = input.ToLower();
            input = Regex.Replace(input, @"[^\w\s]", " ");
            input = Regex.Replace(input, @"\s+", " ");
            return input.Trim();
        }
    }
}
