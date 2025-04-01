using AbyssDorks.BiasClassifier;
using AbyssDorks.Services;

namespace AbyssDorks.BayesClassifier
{
    public class ClassifierManager
    {
        readonly ValidationService _validationService;
        private readonly Dictionary<string, IClassifier> _classifiers;

        public ClassifierManager(ValidationService validationService)
        {
            _validationService = validationService;
            _classifiers = new Dictionary<string, IClassifier>();

            // Завантаження навчальних даних для модуля AdminPanelsModule
            string adminDataFile = Path.Combine("E:\\AbyssDorks\\AbyssDorks\\AbyssDorks\\BayesClassifier\\LearningData\\data_AdminPanels.txt");
            var naiveBayesAdmin = new NaiveBayesClassifier(_validationService);
            List<Models.Document> adminDocs = ReadTrainingData(adminDataFile);     
            naiveBayesAdmin.Train(adminDocs);
            _classifiers["AdminPanelsModule"] = new AdminPanelsModuleClassifier(naiveBayesAdmin, _validationService);

            // Завантаження навчальних даних для модуля FilesDocumentModule
            string fileDocDataFile = Path.Combine("E:\\AbyssDorks\\AbyssDorks\\AbyssDorks\\BayesClassifier\\LearningData\\data_FilesDocument.txt");
            List<Models.Document> fileDocs = ReadTrainingData(fileDocDataFile);
            var naiveBayesFiles = new NaiveBayesClassifier(_validationService);
            naiveBayesFiles.Train(fileDocs);
            _classifiers["FilesDocumentModule"] = new AdminPanelsModuleClassifier(naiveBayesFiles, _validationService);

            // Завантаження навчальних даних для модуля MetadataModule
            string metaDataFile = Path.Combine("E:\\AbyssDorks\\AbyssDorks\\AbyssDorks\\BayesClassifier\\LearningData\\data_Metadata.txt");
            List<Models.Document> metaDocs = ReadTrainingData(metaDataFile);
            var naiveBayesMetaData = new NaiveBayesClassifier(_validationService);
            naiveBayesMetaData.Train(metaDocs);
            _classifiers["MetadataModule"] = new AdminPanelsModuleClassifier(naiveBayesMetaData, _validationService);

            // Завантаження навчальних даних для модуля NetworkDevicesModule
            string networkDevicesFile = Path.Combine("E:\\AbyssDorks\\AbyssDorks\\AbyssDorks\\BayesClassifier\\LearningData\\data_NetworkDevices.txt");
            List<Models.Document> networkDocs = ReadTrainingData(networkDevicesFile);
            var naiveBayesNetwork = new NaiveBayesClassifier(_validationService);
            naiveBayesNetwork.Train(networkDocs);
            _classifiers["NetworkDevicesModule"] = new AdminPanelsModuleClassifier(naiveBayesNetwork, _validationService);

            // Завантаження навчальних даних для модуля VulnerabilitiesModule
            string vulnFile = Path.Combine("E:\\AbyssDorks\\AbyssDorks\\AbyssDorks\\BayesClassifier\\LearningData\\data_Vulnerabilities.txt");
            List<Models.Document> vulnDocs = ReadTrainingData(vulnFile);
            var naiveBayesVuln = new NaiveBayesClassifier(_validationService);
            naiveBayesVuln.Train(vulnDocs);
            _classifiers["VulnerabilitiesModule"] = new AdminPanelsModuleClassifier(naiveBayesVuln, _validationService);

            // Завантаження навчальних даних для модуля WebPageModule
            string webPageFile = Path.Combine("E:\\AbyssDorks\\AbyssDorks\\AbyssDorks\\BayesClassifier\\LearningData\\data_WebPage.txt");
            List<Models.Document> webPageDocs = ReadTrainingData(webPageFile);
            var naiveBayesWebPage = new NaiveBayesClassifier(_validationService);
            naiveBayesWebPage.Train(webPageDocs);
            _classifiers["WebPageModule"] = new AdminPanelsModuleClassifier(naiveBayesWebPage, _validationService);

            // Завантаження навчальних даних для модуля WebServicesModule
            string webSerFile = Path.Combine("E:\\AbyssDorks\\AbyssDorks\\AbyssDorks\\BayesClassifier\\LearningData\\data_WebServices.txt");
            List<Models.Document> webSerDocs = ReadTrainingData(webSerFile);
            var naiveBayesWebSer = new NaiveBayesClassifier(_validationService);
            naiveBayesVuln.Train(webSerDocs);
            _classifiers["WebServicesModule"] = new AdminPanelsModuleClassifier(naiveBayesWebSer, _validationService);

            _classifiers["PeopleSearchModule"] = new NoClassificationClassifier();
            _classifiers["EmailSearchModule"] = new NoClassificationClassifier();
        }

        public IClassifier GetClassifier(string moduleName)
        {
            if (_classifiers.TryGetValue(moduleName, out var classifier))
            {
                return classifier;
            }
            throw new Exception($"Класифікатор для модуля {moduleName} не знайдений");
        }

        

        private List<Models.Document> ReadTrainingData(string filePath)
        {
            List<Models.Document> docs = new List<Models.Document>();
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                string trimmed = line.Trim();
                if (trimmed.StartsWith("\"") && trimmed.EndsWith("\""))
                    trimmed = trimmed.Substring(1, trimmed.Length - 2);
                var parts = trimmed.Split(new string[] { "\",\"" }, StringSplitOptions.None);
                if (parts.Length < 5) continue;

                Models.Document doc = new Models.Document
                {
                    Title = parts[0],
                    Url = parts[1],
                    Snippet = parts[2],
                    Module = parts[3],
                    Label = parts[4]
                };
                // Об'єднуємо Title, Url і Snippet
                doc.CombinedText = _validationService.NormalizerTextForClassification(doc.Title + " " + doc.Url + " " + (doc.Snippet ?? ""));
                docs.Add(doc);
            }
            return docs;
        }
    }

}
