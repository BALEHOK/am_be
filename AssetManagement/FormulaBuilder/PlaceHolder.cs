using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Annotations;
using FormulaBuilder.Annotations;

namespace FormulaBuilder
{
    public class PlaceHolder : INotifyPropertyChanged
    {
        public static event EventHandler<EventArgs> OnChanged;

        private string _caption;
        private TokenValue _data;
        private List<TokenValue> _variants;
        private readonly OperationsData _operationsData;
        private int _selectedIdx;

        public PlaceHolder Parent { get; set; }

        public ObservableCollection<PlaceHolder> RightParameters { get; private set; }

        public ObservableCollection<PlaceHolder> LeftParameters { get; private set; }

        public List<TokenValue> Variants
        {
            get { return _variants; }
            private set
            {
                _variants = value;
                OnPropertyChanged();
            }
        }

        public string Caption
        {
            get { return _caption; }
            set
            {
                _caption = value;
                OnPropertyChanged();
            }
        }

        public int SelectedIdx
        {
            get { return _selectedIdx; }
            set
            {
                _selectedIdx = value;
                OnPropertyChanged();
            }
        }

        public TokenValue Data
        {
            get { return _data; }
            set
            {
                if (value == null || _data == value)
                    return;

                string previousTypeId = null;
                if (_data != null)
                    previousTypeId = _data.Type.TypeId;

                _data = value;

                Caption = _data != null ? _data.Name : _data.Type.Name;

                if (Data.Type.IsValue && Data.Value != null && Data.Type.TypeId == previousTypeId)
                {
                    Caption = _data.Type.Name;

                    if (Parent != null)
                    {
                        var dependencies =
                            Parent.RightParameters.Union(Parent.LeftParameters)
                                .Where(p => p.Data.Type.DependsOn == Data.Type.TypeId)
                                .ToList();
                        dependencies.ForEach(p => p.UpdateVariants(p.Data, Data.Value));
                    }
                }
                else
                {
                    UpdateVariants(Data);
                    UpdateParameters(Data);
                }

                if (OnChanged != null)
                {
                    OnChanged(this, EventArgs.Empty);
                }
            }
        }

        private void UpdateParameters(TokenValue token)
        {
            if (token == null)
                return;

            RightParameters.Clear();
            LeftParameters.Clear();

            token.Type.RightParameters.ForEach(
                p => RightParameters.Add(new PlaceHolder(_operationsData, new TokenValue(p, token)) { Parent = this }));
            token.Type.LeftParameters.ForEach(
                p => LeftParameters.Add(new PlaceHolder(_operationsData, new TokenValue(p, token)) { Parent = this }));
        }

        public void UpdateVariants(TokenValue token, string value = null)
        {
            var newVariants = new List<TokenValue>(_operationsData.GetVariants(token, value));
            Variants = newVariants;
        }

        public PlaceHolder(OperationsData data, TokenValue token)
        {
            _operationsData = data;

            RightParameters = new ObservableCollection<PlaceHolder>();
            LeftParameters = new ObservableCollection<PlaceHolder>();

            Data = token;

            _operationsData.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "CurrenntAssetType")
                {
                    if (Data.Type.TypeId == "asset_value" || Data.Type.TypeId == "related_value")
                        UpdateVariants(Data);
                }
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            if (Data == null)
                return "";

            string result;

            var parameters = new StringBuilder();

            var tokenType = Data.Type;

            if (Data.Type.TypeId == "function" || tokenType.TypeId == "if_condition")
            {
                RightParameters.ToList().ForEach(p =>
                {
                    parameters.Append(p.ToString());
                    if (p != RightParameters.Last())
                        parameters.Append(",");
                });

                result = string.Format("{0}{1}{2}{3}", tokenType.Name, tokenType.Open, parameters, tokenType.Close);
            }
            else if (Data.Type.TypeId == "field_name" || tokenType.TypeId == "table_name" ||
                     tokenType.TypeId == "asset_value")
            {
                result = string.Format("{0}{1}{2}", tokenType.Open, Data.Value, tokenType.Close);
            }
            else if (tokenType.TypeId == "operator")
            {
                var lp = LeftParameters.FirstOrDefault();
                var rp = RightParameters.FirstOrDefault();

                result = string.Format("({0} {1} {2})", lp != null ? lp.ToString() : "", tokenType.Name, rp != null ? rp.ToString() : "");
            }
            else
                result = string.Format("{0}{1}{2}", tokenType.Open, Data.Type.Name, tokenType.Close);

            if (Data.ParentValue != null && Data.ParentValue.Type.TypeId == "expression")
            {
                result = string.Format("{0}{1}{2}", Data.ParentValue.Type.Open, result, Data.ParentValue.Type.Close);
            }

            return result;
        }
    }
}