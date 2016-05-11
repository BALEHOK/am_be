using Newtonsoft.Json.Linq;

namespace AssetManager.Infrastructure.Models
{
    public class EntityAttribConfigModel
    {
        public string Name { get; set; }
        public JToken Value { get; set; }
    }
}