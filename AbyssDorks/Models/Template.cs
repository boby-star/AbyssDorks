using System.Text.Json.Serialization;

namespace AbyssDorks.Models
{
    public class Template
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("template")]
        public string TemplateText { get; set; }
        [JsonPropertyName("hint")]
        public string Hint { get; set; }
        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; }
    }
}
