using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using AssetManagerAdmin.FormulaBuilder.ValueEditors;
using FormulaBuilder.Annotations;

namespace AssetManagerAdmin.FormulaBuilder.Expressions.ExpressionTypes
{
    public enum OperandPosition
    {
        Right = 0,
        Left = 1
    }

    public class ExpressionEntry : ICloneable, INotifyPropertyChanged
    {
        public static event EventHandler<ExpressionEntry> OnSelected;
        public static event EventHandler<ExpressionEntry> OnValueChanged;

        public event EventHandler<ExpressionEntry> OnEntryChanged;

        private ExpressionEntry _selected;
        private bool _isLeftConnectorSelected;
        private bool _isRightConnectorSelected;
        private bool _isEntrySelected;
        private string _value;

        protected List<ExpressionEntry> RightOperands;
        protected List<ExpressionEntry> LeftOperands;
        private Brush _captionColor;
        private string _caption;
        private bool _isRightConnectorVisible;
        private bool _isLeftConnectorVisible;
        private bool _isEditable;

        public string DisplayName { get; set; }

        public virtual string Value
        {
            get { return _value; }
            set
            {
                _value = value;

                if (Items != null)
                    Selected = Items.SingleOrDefault(i => i.Name == _value);

                OnPropertyChanged();
                NotifyValueChanged();
            }
        }

        public ExpressionEntry Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                OnPropertyChanged();
            }
        }

        public ExpressionBuilder Builder { get; set; }

        public List<ExpressionEntry> Items { get; set; }

        public ReadOnlyCollection<ExpressionEntry> RightOperandsList
        {
            get { return RightOperands.AsReadOnly(); }
        }

        public ReadOnlyCollection<ExpressionEntry> LeftOperandsList
        {
            get { return LeftOperands.AsReadOnly(); }
        }

        public ExpressionEntry Type { get; set; }
        public ExpressionEntry Parent { get; set; }
        public bool IsLocalContext { get; set; }        
        public ExpressionEntry Context { get; set; }        

        #region Inheritable properties
        //todo: use attribte to mark such properties
        public bool IsExtendable { get; set; }
        public List<ExpressionEntry> Overrides { get; set; }
        #endregion

        #region Formula Generation Properties
        public string Open { get; set; }
        public string Close { get; set; }
        public string Name { get; set; }
        #endregion

        #region Presentation Properties

        public Type EditorType { get; protected set; }
        public string Group { get; set; }

        public string Suffix { get; set; }
        public string Postfix { get; set; }

        public bool IsEditable
        {
            get { return _isEditable; }
            set
            {
                _isEditable = value;
                OnPropertyChanged();
            }
        }

        public bool IsEntrySelected
        {
            get { return _isEntrySelected; }
            set
            {
                _isEntrySelected = value;
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

        public Brush CaptionColor
        {
            get { return _captionColor; }
            set
            {
                _captionColor = value;
                OnPropertyChanged();
            }
        }

        public bool IsRightConnectorVisible
        {
            get { return _isRightConnectorVisible; }
            set
            {
                _isRightConnectorVisible = value;
                OnPropertyChanged();
            }
        }

        public bool IsLeftConnectorVisible
        {
            get { return _isLeftConnectorVisible; }
            set
            {
                _isLeftConnectorVisible = value;
                OnPropertyChanged();
            }
        }

        public bool IsRightConnectorSelected
        {
            get { return _isRightConnectorSelected; }
            set
            {
                _isRightConnectorSelected = value;
                if (IsRightConnectorSelected && IsLeftConnectorSelected)
                    IsLeftConnectorSelected = false;

                OnPropertyChanged();
            }
        }

        public bool IsLeftConnectorSelected
        {
            get { return _isLeftConnectorSelected; }
            set
            {
                _isLeftConnectorSelected = value;
                if (IsLeftConnectorSelected && IsRightConnectorSelected)
                    IsRightConnectorSelected = false;

                OnPropertyChanged();
            }
        }

        #endregion

        public ExpressionEntry()
        {
            DisplayName = "...";
            CaptionColor = Brushes.Black;

            IsEditable = true;
            IsExtendable = true;
            EditorType = typeof(SingleComboBoxSelector);

            Context = null;            
            Overrides = new List<ExpressionEntry>();
            LeftOperands = new List<ExpressionEntry>();
            RightOperands = new List<ExpressionEntry>();
        }

        protected void NotifyValueChanged()
        {
            if (OnValueChanged != null)
                OnValueChanged(this, this);
            OnPropertyChanged("Value");
        }

        #region Connections handling

        public virtual ExpressionEntry Connect(ExpressionEntry entry)
        {
            // only binary operators can be connected by default
            if (!(entry is BinaryOperatorEntry) || NoConnectorsSelected)
                return null;

            var place = IsRightConnectorSelected ? OperandPosition.Left : OperandPosition.Right;

            if (Parent != null)
                Parent.ReplaceOperand(this, entry);

            entry.AddOperand(this, place);

            return entry;
        }

        public virtual ExpressionEntry Replace(ExpressionEntry entry)
        {
            var container = Parent;
            return container != null
                ? container.ReplaceOperand(this, entry)
                : entry;
        }

        public bool NoConnectorsSelected { get { return !IsLeftConnectorSelected && !IsRightConnectorSelected; } }

        #endregion

        #region Operands handling

        public List<ExpressionEntry> AllOperands
        {
            get
            {
                var list = new List<ExpressionEntry>(LeftOperandsList);
                list.AddRange(RightOperandsList);
                return list;
            }
        }

        public void ClearOperands()
        {
            LeftOperands = new List<ExpressionEntry>();
            RightOperands = new List<ExpressionEntry>();
        }

        public void AddOperand(ExpressionEntry operand, OperandPosition position)
        {
            if (position == OperandPosition.Left)
                AddLeftOperand(operand);
            else
                AddRightOperand(operand);
        }

        public virtual ExpressionEntry AddLeftOperand(ExpressionEntry operand)
        {
            operand.Parent = this;
            LeftOperands.Add(operand);
            return this;
        }

        public virtual ExpressionEntry AddRightOperand(ExpressionEntry operand, Type dependency = null)
        {
            operand.Parent = this;
            RightOperands.Add(operand);
            return this;
        }

        public virtual ExpressionEntry ReplaceOperand(ExpressionEntry source, ExpressionEntry destination)
        {
            var idx = AllOperands.IndexOf(source);

            if (destination == null)
            {
                RightOperands.Remove(source);
            }
            else
            {
                destination.Parent = this;

                destination.Overrides = Type.AllOperands[idx].Overrides;
                destination.IsExtendable = Type.AllOperands[idx].IsExtendable;

                RightOperands[idx] = destination;
            }

            return destination;
        }

        #endregion

        public ExpressionEntry FindOverride(ExpressionEntry entry)
        {
            if (entry == null)
                return null;

            var overriden = entry.Overrides.SingleOrDefault(o => o.GetType() == GetType());

            if (overriden == null && entry.Parent != null)
                return FindOverride(entry.Parent);

            return overriden;
        }

        public object Clone()
        {
            var copy = (ExpressionEntry)MemberwiseClone();
            copy.Type = this;
            copy.ClearOperands();

            return copy;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));

            if (OnEntryChanged != null)
                OnEntryChanged(this, this);

            //todo: move all this stuff to subclasses
            if (propertyName != null &&
                (propertyName == "Selected" || propertyName == "Value" || propertyName == "IsEntrySelected"))
            {
                Caption = IsEntrySelected ? DisplayName : Selected == null ? "..." : Selected.DisplayName;
                IsLeftConnectorVisible = IsRightConnectorVisible = IsEntrySelected && IsExtendable;
            }

            if (propertyName != null && (propertyName == "Selected" || propertyName == "Value"))
            {
                if (OnSelected != null)
                    OnSelected(this, this);
            }
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}