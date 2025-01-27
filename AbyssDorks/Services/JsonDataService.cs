using AbyssDorks.Models;
using System.Text.Json;

namespace AbyssDorks.Services
{
    public class JsonDataService
    {
        private readonly string _basePath = Path.Combine("E:\\AbyssDorks\\AbyssDorks\\AbyssDorks\\JsonData");

        public Module LoadModule(string moduleName)
        {
            try
            {
                // Логування шляху до файлу для діагностики
                Console.WriteLine($"Шлях до файлу: {_basePath}\\{moduleName}.json");

                var filePath = Path.Combine(_basePath, $"{moduleName}.json");

                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"JSON файл для модуля {moduleName} не існує за шляхом {filePath}");
                }

                var json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<Module>(json) ?? throw new Exception($"Не вдалося десеріалізувати модуль {moduleName}.");
            }
            catch (Exception ex)
            {
                // Детальний лог помилки
                Console.WriteLine($"Помилка при завантаженні модуля {moduleName}: {ex.Message}");
                throw;
            }
        }

        public List<Operator> LoadOperators()
        {
            var filePath = Path.Combine(_basePath, "operators.json");
            if (!File.Exists(filePath))
                throw new FileNotFoundException("JSON файл з операторами не знайдений");

            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Operator>>(json);
        }
    }
}
