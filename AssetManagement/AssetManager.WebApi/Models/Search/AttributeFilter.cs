using AssetManager.Infrastructure.Models.TypeModels;
using Newtonsoft.Json;

namespace AssetManager.WebApi.Models.Search
{
    public class AttributeFilter
    {
        public enum LogicalOperators
        {
            None = 0,
            And = 1,
            Or = 2
        }

        public enum ParenthesisType
        {
            None = 0,
            Open = 1,
            Closing = 2
        }

        [JsonProperty(PropertyName = "lo")]
        public LogicalOperators LogicalOperator { get; set; }

        [JsonProperty(PropertyName = "operator")]
        public int OperatorId { get; set; }

        public ParenthesisType Parenthesis { get; set; }

        public string Value { get; set; }

        public AttributeTypeModel ReferenceAttrib { get; set; }
    }
}