using AbyssDorks.Models;
using Microsoft.Extensions.FileProviders.Physical;
using System.Net;
using System.Text.Json;
using System.Web;

namespace AbyssDorks.Services
{
    public class SearchService
    {
        private readonly HttpClient _httpClient;

        public SearchService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<SearchResult>> ExecuteSearch(HttpContext httpContext, string query)
        {
            var apiKey = httpContext.Session.GetString("GoogleApiKey");
            var cx = httpContext.Session.GetString("GoogleCx");

            Console.WriteLine($"API-ключ із сесії: {apiKey}");
            Console.WriteLine($"CX із сесії: {cx}");

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(cx))
                throw new Exception("API-ключ або CX не задано. Введіть їх у відповідне поле.");

            var allResults = new List<SearchResult>();
            int startIndex = 1;
            int maxIndex = 20;
            Console.WriteLine($"(сервіс) надійшов запит: {query}");

            while (startIndex <= maxIndex)
            {
                var decodeHtmlQuery = WebUtility.HtmlDecode(query); 
                Console.WriteLine($"запит перед додаванням до урл: {decodeHtmlQuery}");
                var url = $"https://www.googleapis.com/customsearch/v1?key={apiKey}&cx={cx}&q={Uri.EscapeDataString(decodeHtmlQuery)}&start={startIndex}&safe=off&filter=0";
                Console.WriteLine($"URL-запиту: {url}");

                try
                {
                    var response = await _httpClient.GetAsync(url);
                    Console.WriteLine($"Статус відповіді: {response.StatusCode}");

                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"Помилка виконання запиту. Код: {response}");

                    var googleResponse = await response.Content.ReadFromJsonAsync<GoogleSearchResponse>();
                    Console.WriteLine($"Отримані дані: {JsonSerializer.Serialize(googleResponse)}");

                    if (googleResponse == null || googleResponse.Items == null)
                        break;

                    allResults.AddRange(googleResponse.Items.Select(item => new SearchResult
                    {
                        Title = item.Title,
                        Url = item.Link,
                        Snippet = item.Snippet,
                        //Relevance = null //пізніше додати коли буде створюватись ШІ
                    }));

                    if (googleResponse.Items.Count < 10)
                        break;

                    startIndex += 10;
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception("Помилка HTTP-запиту: " + ex.Message);
                }
                catch (Exception ex)
                {
                    throw new Exception("Помилка при обробці пошуку: " + ex.Message);
                }
            }

            return allResults;
        }

        public class GoogleSearchResponse
        {
            public List<GoogleSearchItem> Items { get; set; }
        }

        public class GoogleSearchItem
        {
            public string Title { get; set; }
            public string Link { get; set; }
            public string Snippet { get; set; }
        }
    }
}
