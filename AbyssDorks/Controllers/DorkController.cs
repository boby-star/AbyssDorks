using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using AbyssDorks.Models;
using AbyssDorks.Services;
using Microsoft.AspNetCore.Mvc;

namespace AbyssDorks.Controllers
{
    public class DorkController : Controller
    {
        private readonly ILogger<DorkController> _logger;
        private readonly JsonDataService _jsonDataService;
        private readonly TagsService _tagsService;
        private readonly DorkGeneratorService _dorkGeneratorService;
        private readonly SearchService _searchService;
        private readonly ValidationService _validationService;

        public DorkController(ILogger<DorkController> logger, JsonDataService jsonDataService, TagsService tagsService, DorkGeneratorService dorkGeneratorService, SearchService searchService, ValidationService validationService)
        {
            _logger = logger;
            _jsonDataService = jsonDataService;
            _tagsService = tagsService;
            _dorkGeneratorService = dorkGeneratorService;
            _searchService = searchService;
            _validationService = validationService;
        }

        public IActionResult Index()
        {
            //var apiKey = HttpContext.Session.GetString("GoogleApiKey");
            //var cx = HttpContext.Session.GetString("GoogleCx");

            //if (string.IsNullOrEmpty(cx) || string.IsNullOrEmpty(apiKey))
            //{
            //    return RedirectToAction("SetCredentials");
            //}

            var modules = new List<string>
            {
                "AdminPanelsModule",
                "EmailSearchModule",
                "FilesDocumentModule",
                "MetadataModule",
                "NetworkDevicesModule",
                "PeopleSearchModule",
                "VulnerabilitiesModule",
                "WebPageModule",
                "WebServicesModule"
            };

            return View(modules);
        }

        public IActionResult SetCredentials()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SaveCredentials(string apiKey, string cx)
        {
            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(cx))
            {
                return BadRequest("API-ключ або CX не може бути пустим.");
            }

            HttpContext.Session.SetString("GoogleApiKey", apiKey);
            HttpContext.Session.SetString("GoogleCx", cx);

            Console.WriteLine($"API: {apiKey}");
            Console.WriteLine($"CX: {cx}");

            return Ok("Дані успішно збережено");
        }

        public IActionResult GetTags(string moduleName)
        {
            var tags = _tagsService.GetTagsForModule(moduleName);
            return Json(tags);
        }

        public IActionResult GetTemplates(string moduleName, List<string> tags)
        {
            var templates = _dorkGeneratorService.GetTemplateByTags(moduleName, tags);
            return Json(templates);
        }

        [HttpPost]
        public IActionResult GenerateQuery(string moduleName, string templateName, Dictionary<string, string> parameters)
        {
            var module = _jsonDataService.LoadModule(moduleName);
            var template = module.Templates.FirstOrDefault(t => t.Name == templateName);

            if (template == null)
            {
                return BadRequest("шаблон не знайдено");
            }

            if (!_validationService.ValidateParameters(template, parameters))
            {
                return BadRequest("Некоректні параметри шаблону");
            }

            var query = _dorkGeneratorService.GenerateSearchQuery(template, parameters);

            if (!_validationService.ValidateSyntax(query))
            {
                return BadRequest("Некоректний синтаксис запиту");
            }

            return Json(new { Query =  query });
        }

        [HttpPost]
        public async Task<IActionResult> ExecuteSearch(string query)
        {
            try
            {
                var decodeQuery = WebUtility.UrlDecode(query);
                Console.WriteLine($"(контролер) Запит: {query}");
                Console.WriteLine($"(контролер) декодований запит: {decodeQuery}");

                var results = await _searchService.ExecuteSearch(HttpContext, decodeQuery);
                return Json(new { Items = results});
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Results(string query)
        {
            ViewBag.Query = query;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        ////public IActionResult Error()
        ////{
        ////    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        ////}
    }
}
