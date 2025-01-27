using System.Text.Json.Serialization;

namespace AbyssDorks.Models
{
    public class Module
    {
        [JsonPropertyName("templates")]
        public List<Template> Templates { get; set; }
    }
}
