using AbyssDorks.Models;

namespace AbyssDorks.Services
{
    public class DorkGeneratorService
    {
        private readonly JsonDataService _jsonDataService;
        private readonly ValidationService _validationService;

        public DorkGeneratorService(JsonDataService jsonDataService, ValidationService validationService)
        {
            _jsonDataService = jsonDataService;
            _validationService = validationService;
        }

        public List<Template> GetTemplateByTags(string moduleName, List<string> tags)
        {
            var module = _jsonDataService.LoadModule(moduleName);

            var normalizedTags = tags.Select(t => t.Trim().ToLower()).ToList();

            return module.Templates
                .Where(t => t.Tags.Select(tag => tag.Trim().ToLower()).Intersect(normalizedTags).Any()).ToList();
        }

        public string GenerateSearchQuery(Template template, Dictionary<string, string> parameters)
        {
            if (!_validationService.ValidateParameters(template, parameters))
                throw new Exception("Невідповідність параметрів у шаблоні");

            var query = template.TemplateText;
            foreach (var param in parameters)
            {
                query = query.Replace($"{{{param.Key}}}", param.Value);
            }

            if (!_validationService.ValidateSyntax(query))
                throw new Exception("Синтаксис запиту некоректний");

            return query;
        }
    }
}
