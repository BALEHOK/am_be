using System.Collections.Generic;
using System.Linq;
using AssetManagerAdmin.FormulaBuilder.ValueEditors;

namespace AssetManagerAdmin.FormulaBuilder.Expressions.ExpressionTypes
{
    public class RelatedFieldValueEntry : IdentifierEntry
    {
        private List<ExpressionEntry> _subItems;
        private RelatedFieldValueEntry _relatedSelected;
        private ExpressionEntry _subItemSelected;

        public List<ExpressionEntry> SubItems
        {
            get { return _subItems; }
            set
            {
                _subItems = value;
                OnPropertyChanged();
            }
        }

        public override string Value
        {
            get { return base.Value; }
            set
            {
                if (value != null)
                {
                    var relatedName = value.Split('@');

                    Selected = (RelatedFieldValueEntry) Items.SingleOrDefault(a => a.Name == relatedName[0]);
                    if (Selected != null)
                        SubItemSelected = Selected.SubItems.SingleOrDefault(a => a.Name == relatedName[1]);
                }

                base.Value = value;
            }
        }

        public new RelatedFieldValueEntry Selected
        {
            get { return _relatedSelected; }
            set
            {
                _relatedSelected = value;
                OnPropertyChanged();
            }
        }

        public ExpressionEntry SubItemSelected
        {
            get { return _subItemSelected; }
            set
            {
                _subItemSelected = value;
                OnPropertyChanged("Selected");                
            }
        }

        public RelatedFieldValueEntry()
        {
            EditorType = typeof(DoubleComboBoxSelector);
            DisplayName = "Related Field";
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            base.OnPropertyChanged(propertyName);

            if (propertyName != null &&
                (propertyName == "Selected" || propertyName == "Value" || propertyName == "IsEntrySelected"))
            {
                var table = Selected == null ? "..." : Selected.DisplayName;
                var field = SubItemSelected == null ? "..." : " -> " + SubItemSelected.DisplayName;
                Caption = IsEntrySelected ? DisplayName : table + field;
            }
        }

        public override string ToString()
        {
            var name = Selected != null ? Selected.Name : "";
            var subItem = SubItemSelected != null ? SubItemSelected.Name : "";
            var type = FindOverride(this) ?? Type;
            var result = string.Format("{0}{1}@{3}{2}", type.Open, name, type.Close, subItem);
            return result;
        }
    }
}