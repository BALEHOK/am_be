using AssetManager.Infrastructure.JsonConverters;
using AssetManager.Infrastructure.Models.TypeModels;
using Newtonsoft.Json;

namespace AssetManager.Infrastructure.Models.Search
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

        [JsonConverter(typeof(AttributeFilterValueConverter))]
        public IdNamePair<string, string> Value { get; set; }

        public bool UseComplexValue { get; set; }

        public AttributeFilter[] ComplexValue { get; set; }

        public AttributeTypeModel ReferenceAttrib { get; set; }

        public int Index { get; set; }

        public bool IsEmpty
        {
            get
            {
                return Parenthesis == ParenthesisType.None
                    && (
                        (UseComplexValue && (ComplexValue == null || ComplexValue.Length == 0))
                            || (!UseComplexValue && (Value == null || string.IsNullOrEmpty(Value.Id)))
                        );
            }
        }
    }
}