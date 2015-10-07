using System;
using System.Collections.Generic;
using System.Linq;
using AppFramework.Core.Classes;
using AssetManagerAdmin.FormulaBuilder.Expressions.ExpressionTypes;

namespace AssetManagerAdmin.FormulaBuilder.Expressions
{
    public class ExpressionBuilder
    {
        private readonly Stack<ExpressionEntry> _changeHistory = new Stack<ExpressionEntry>();
        private ExpressionEntry _rootEntry;
        private ExpressionEntry _currentExpression;
        private readonly ExpressionEntryFactory _expressionEntryFactory;

        public event EventHandler OnExpressionChanged;

        public ExpressionEntry Expression
        {
            get { return _rootEntry; }
        }

        public ExpressionBuilder(IEntryFactoryDataProvider dataProvider, ExpressionsGrammar grammar)
        {            
            _expressionEntryFactory = new ExpressionEntryFactory(this, grammar, dataProvider);

            ExpressionEntry.OnSelected += ExpressionEntryOnValueChanged;
        }

        private void ExpressionEntryOnValueChanged(object sender, ExpressionEntry entry)
        {
            if (entry.Type != null && entry.Type.IsLocalContext)
            {
                ExpressionEntry.OnSelected -= ExpressionEntryOnValueChanged;
                UpdateDependencies(_rootEntry, entry);
                ExpressionEntry.OnSelected += ExpressionEntryOnValueChanged;
            }

//            if (entry.Parent is FunctionEntry)
//            {
//                var nextOperand = entry.Parent.AllOperands.FirstOrDefault(o => o.Selected == null);
//                if (nextOperand != null)
//                    ToggleSelection(nextOperand);
//            }

            NotifyOnChange();
        }

        public void UpdateValues<T>(ExpressionEntry entry) where T : ExpressionEntry
        {
            if (entry is T)
            {
                var newEntry = _expressionEntryFactory.GetByType(entry, entry.Parent);
                entry.Replace(newEntry);
                NotifyOnChange();
            }

            if (entry != null)
                entry.AllOperands.ForEach(UpdateValues<T>);
        }

        public static ExpressionEntry CopyExpression(ExpressionEntry entry)
        {
            if (entry == null)
                return null;

            var cloned = (ExpressionEntry)entry.Clone();
            entry.RightOperandsList.Select(CopyExpression)
                .ToList()
                .ForEach(o => cloned.AddRightOperand(o));
            entry.LeftOperandsList.Select(CopyExpression)
                .ToList()
                .ForEach(o => cloned.AddLeftOperand(o));

            return cloned;
        }

        private void SaveCurrentExpression()
        {
            var clone = CopyExpression(_rootEntry);
            if (clone != null)
                _changeHistory.Push(clone);
        }      

        private void UpdateDependencies(ExpressionEntry entry, ExpressionEntry updatedEntry)
        {
            if (entry == null || updatedEntry == null)
                return;            

            if (entry.Context == updatedEntry)
            {
                var updated = _expressionEntryFactory.GetByName(entry.Type, entry.Name, entry.Parent);
                entry.Replace(updated);
            }

            entry.AllOperands.ForEach(e => UpdateDependencies(e, updatedEntry));
        }

        public void UndoLastAction()
        {
            if (_changeHistory.Count > 0)
            {
                SetRootEntry(_changeHistory.Pop());
            }
        }

        public void Reset(bool cleanHistory = false)
        {
            if (!cleanHistory)
                SaveCurrentExpression();
            else
                _changeHistory.Clear();

            _rootEntry = null;
            _currentExpression = null;
            NotifyOnChange();
        }

        public void SetCurrentExpression(ExpressionEntry entry)
        {
            _currentExpression = entry;
        }

        public ExpressionEntry AddEntry(ExpressionEntry entry)
        {
            SaveCurrentExpression();

            var selected = GetSelectedEntry();
            _currentExpression = selected ?? _currentExpression;

            var newEntry = _expressionEntryFactory.GetByType(entry, _currentExpression);
            newEntry.IsRightConnectorSelected = true;

            if (newEntry is FunctionEntry)
            {
                newEntry.Type.AllOperands.ForEachWithIndex((expressionEntry, i) =>
                {
                    var type = newEntry.Type.AllOperands[i];
                    var parameter = _expressionEntryFactory.GetByType(type, newEntry);
                    parameter.IsExtendable = type.IsExtendable;
                    newEntry.AddRightOperand(parameter);
                });
            }

            if (_currentExpression == null)
            {
                _rootEntry = newEntry;
                _currentExpression = newEntry;
            }
            else if (_currentExpression.IsExtendable)
            {
                if (selected != null && selected.NoConnectorsSelected)
                {
                    _currentExpression = _currentExpression.Replace(newEntry) ?? _currentExpression;
                    ResetSelection(_rootEntry);
                }
                else
                {
                    _currentExpression = _currentExpression.Connect(newEntry) ?? _currentExpression;
                }

                if (_currentExpression.Parent == null)
                    _rootEntry = _currentExpression;
            }

            //ResetSelection(_rootEntry);

            if (newEntry is FunctionEntry && newEntry.AllOperands.Count > 0)
            {
                ToggleSelection(newEntry.AllOperands.First());
            }
            else
                ToggleSelection(newEntry);

            NotifyOnChange();

            return _rootEntry;
        }

        public void RemoveSelectedEntry()
        {
            SaveCurrentExpression();

            var selected = GetSelectedEntry();

            if (selected != null)
            {
                if (selected.Parent == null)
                {
                    Reset();
                }
                else
                {
                    if (selected.Parent is FunctionEntry)
                    {
                        var idx = selected.Parent.AllOperands.IndexOf(selected);
                        var empty = _expressionEntryFactory
                            .GetByType(selected.Parent.Type.AllOperands[idx], _currentExpression);
                        selected.Replace(empty);
                    }
                    else
                        selected.Replace(null);

                    ToggleSelection(selected.Parent);
                }
            }

            NotifyOnChange();
        }

        public ExpressionEntry GetSelectedEntry(ExpressionEntry entry = null)
        {
            var root = entry ?? _rootEntry;

            if (root == null)
                return null;

            var result = root.IsEntrySelected
                ? root
                : root.AllOperands.Select(GetSelectedEntry).SingleOrDefault(e => e != null);

            return result;
        }

        public void SetRootEntry(ExpressionEntry entry)
        {
            _rootEntry = entry;
            _currentExpression = _rootEntry;
            NotifyOnChange();
        }

        public void ToggleSelection(ExpressionEntry entry)
        {
            if (entry == null)
                return;

            if (entry.IsEntrySelected)
                ResetSelection(_rootEntry);
            else
                ResetSelection(_rootEntry, entry);
        }

        private void ResetSelection(ExpressionEntry root, ExpressionEntry selection = null)
        {
            if (root == null)
                return;

            root.IsLeftConnectorSelected = false;
            root.IsRightConnectorSelected = root.GetType() != typeof(ExpressionEntry);
            root.IsEntrySelected = selection != null && root == selection;
            root.AllOperands.ForEach(e => ResetSelection(e, selection));
        }

        private void NotifyOnChange()
        {
            if (OnExpressionChanged != null)
                OnExpressionChanged(this, EventArgs.Empty);
        }
    }
};