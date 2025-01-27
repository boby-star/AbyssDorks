namespace AbyssDorks.Services
{
    public class TagsService
    {
        private readonly JsonDataService _jsonDataService;

        public TagsService(JsonDataService jsonDataService)
        {
            _jsonDataService = jsonDataService;
        }

        public List<string> GetTagsForModule(string moduleName)
        {
            var module = _jsonDataService.LoadModule(moduleName);
            //if (module?.Templates == null)
            //    throw new Exception($"НЕ ЗНАЙДЕНО шаблонів для модуля {moduleName}");
            return module.Templates.SelectMany(t => t.Tags).Distinct().ToList();
        }
    }
}
