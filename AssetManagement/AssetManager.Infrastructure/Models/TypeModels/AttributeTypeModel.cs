using System.ComponentModel;
using System.Runtime.CompilerServices;
using AppFramework.ConstantsEnumerators;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace AssetManager.Infrastructure.Models.TypeModels
{
    public class AttributeTypeModel : INotifyPropertyChanged
    {
        private bool _isHighlighted;
        public long Id { get; set; }
        public string DisplayName { get; set; }
        public string DbName { get; set; }
        public int DisplayOrder { get; set; }
        public AssetTypeModel RelationType { get; set; }
        public long RelationId { get; set; }
        public string ValidationExpression { get; set; }
        public string CalculationFormula { get; set; }
        public string ScreenFormula { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Enumerators.DataType DataType { get; set; }

        // true if this attribute represents child assets relationship
        public bool IsChildAssets { get; set; }

        public bool HasDatabaseFormula
        {
            get { return !string.IsNullOrWhiteSpace(CalculationFormula); }
        }

        public bool HasScreenFormula
        {
            get { return !string.IsNullOrWhiteSpace(ScreenFormula); }
        }
        public bool HasValidationExpression
        {
            get { return !string.IsNullOrEmpty(ValidationExpression); }
        }

        public bool IsHighlighted
        {
            get { return _isHighlighted; }
            set
            {
                _isHighlighted = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public AttributeTypeModel ShallowCopy()
        {
            return (AttributeTypeModel)MemberwiseClone();
        }
    }
}