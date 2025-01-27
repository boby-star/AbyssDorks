using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace AbyssDorks.Models
{
    public class Operator
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("example")]
        public string Example {  get; set; }
        [JsonPropertyName("parameters")]
        public bool Parameters { get; set; }
        [JsonPropertyName("categories")]
        public List<string> Categories { get; set; }   
                        
    }
}
