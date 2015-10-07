using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using AssetManager.Infrastructure.Models;
using AssetManager.Infrastructure.Models.TypeModels;
using FormulaBuilder.Annotations;

namespace FormulaBuilder
{
    public class OperationsData : INotifyPropertyChanged
    {
        private readonly Dictionary<string, List<TokenType>> _grammar;
        private readonly Dictionary<string, List<TokenValue>> _data;
        private TypesInfoModel _typesInfo;
        private AssetTypeModel _currenntAssetType;

        public AssetTypeModel CurrenntAssetType
        {
            get { return _currenntAssetType; }
            set
            {
                _currenntAssetType = value;

                if (_currenntAssetType == null)
                    return;

                UpdateData();
                
                OnPropertyChanged();
            }
        }

        private void UpdateData()
        {
            _data["asset_value"] = CurrenntAssetType.Attributes.Select(a => new TokenValue(a.DisplayName, a.DbName)).ToList();
            _data["related_value"] = CurrenntAssetType.Attributes
                .Where(a => a.RelationType != null)
                .Select(a => new TokenValue(a.DisplayName, a.DbName)).ToList();
        }

        public TypesInfoModel TypesInfo
        {
            get { return _typesInfo; }
            set
            {
                _typesInfo = value;

                _data["table_name"] = _typesInfo.ActiveTypes.Select(t => new TokenValue(t.DisplayName, t.DbName)).ToList();
            }
        }        

        private static TokenType CreateToken(string typeId, string name = null, string open = null, string close = null, bool? mutable = null)
        {
            var type = TokenType.T(typeId);

            if (!string.IsNullOrEmpty(name))
                type.SetName(name);
            if (!string.IsNullOrEmpty(open))
                type.Opn(open);
            if (!string.IsNullOrEmpty(close))
                type.Cls(close);
            if (mutable != null)
                type.IsMutable((bool)mutable);

            return type;
        }

        private void Add(TokenType type)
        {
            _grammar[type.TypeId].Add(type);
        }

        public OperationsData()
        {
            _grammar = new Dictionary<string, List<TokenType>>();
            TokenType.Get.Select(t => t.TypeId).ToList().ForEach(typeId => _grammar.Add(typeId, new List<TokenType>()));

            _data = new Dictionary<string, List<TokenValue>>();
            TokenType.Get.Where(t => t.IsValue).ToList().ForEach(t => _data.Add(t.TypeId, new List<TokenValue>()));

            Add(CreateToken("function", "SELECT")
                .AddRightParameter(CreateToken("table_name", "", "[$", "]", false))
                .AddRightParameter(CreateToken("field_name", "", "[$", "]", false))
                .AddRightParameter(CreateToken("asset_value", "", "[", "]", false))
                .AddRightParameter(CreateToken("expression", "", "'$", "'")
                    .OverrideType(TokenType.T("asset_value").Opn("[@"))
                    .OverrideType(TokenType.T("field_name").IsRoot(true))));

            Add(CreateToken("function", "SQLFIND")
                .AddRightParameter(CreateToken("table_name", "", "[$", "]", false))
                .AddRightParameter(CreateToken("field_name", "", "[$", "]", false))                
                .AddRightParameter(CreateToken("expression")));

            Add(CreateToken("function", "SQLINDEX")
                .AddRightParameter(CreateToken("table_name", "", "[$", "]", false))
                .AddRightParameter(CreateToken("field_name", "", "[$", "]", false))
                .AddRightParameter(CreateToken("expression")));

            Add(CreateToken("function", "SUM")
                .AddRightParameter(CreateToken("function", "", "(", ")", false)));

            Add(CreateToken("function", "TOMONEY")
                .AddRightParameter(CreateToken("expression")));

            Add(CreateToken("function", "COUNT")
                .AddRightParameter(CreateToken("expression")));

            Add(CreateToken("function", "ABS")
                .AddRightParameter(CreateToken("expression")));
            Add(CreateToken("function", "TRUNCATE")
                .AddRightParameter(CreateToken("expression")));
            Add(CreateToken("function", "ROUND")
                .AddRightParameter(CreateToken("expression")));
            Add(CreateToken("function", "REMAINDER")
                .AddRightParameter(CreateToken("expression")));

            Add(CreateToken("if_condition", "if")
                .AddRightParameter(CreateToken("expression"))
                .AddRightParameter(CreateToken("expression"))
                .AddRightParameter(CreateToken("expression")));

            _grammar["operator"].Add(CreateToken("operator", "+")
                .AddRightParameter(CreateToken("expression"))
                .AddLeftParameter(CreateToken("expression")));
            _grammar["operator"].Add(CreateToken("operator", "-")
                .AddRightParameter(CreateToken("expression"))
                .AddLeftParameter(CreateToken("expression")));
            _grammar["operator"].Add(CreateToken("operator", "*")
                .AddRightParameter(CreateToken("expression"))
                .AddLeftParameter(CreateToken("expression")));
            _grammar["operator"].Add(CreateToken("operator", "/")
                .AddRightParameter(CreateToken("expression"))
                .AddLeftParameter(CreateToken("expression")));

            Add(CreateToken("variable", "CurrentUserId"));
        }

        private TokenType FindTypeOverride(TokenValue token, string typeId)
        {
            var over = token.Type.TypeOverrides.SingleOrDefault(o => o.TypeId == typeId);

            if (over == null && token.ParentValue != null)
                over = FindTypeOverride(token.ParentValue, typeId);

            return over;
        }

        public TokenType GetModifiedType(TokenValue token, TokenType source)
        {
            if (token.Type.TypeId == "expression")
                return source;

            var modifiedType = FindTypeOverride(token, source.TypeId);
            if (modifiedType == null)
            {
                modifiedType = TokenType.DeepClone(source);
                modifiedType.Mutable = token.Type.Mutable;
                modifiedType.Open = token.Type.Open ?? modifiedType.Open;
                modifiedType.Close = token.Type.Close ?? modifiedType.Close;
                modifiedType.DependsOn = token.Type.DependsOn ?? modifiedType.DependsOn;
                modifiedType.Root = token.Type.Root;
            }

            return modifiedType;
        }

        private TokenValue FindExpression(TokenValue token)
        {
            return token.Type.TypeId == "expression" ? token : FindExpression(token.ParentValue);
        }

        public List<TokenValue> GetVariants(TokenValue token, string value = "")
        {
            List<TokenValue> result;

            var parent = token;
            if (token.Type.TypeId != "expression")
                parent = token.ParentValue;

            if (token.Type.IsValue)
            {
                var type = GetModifiedType(token, TokenType.T(token.Type.TypeId));
                result = _data[token.Type.TypeId].Select(v => new TokenValue(GetModifiedType(token, type), parent)
                    {
                        Name = v.Name,
                        Value = v.Value
                    }).ToList();
            }
            else
            {
                if (token.Type.TypeId == "expression")
                {
                    result = TokenType.Get.Where(t =>
                    {
                        var type = FindTypeOverride(token, t.TypeId);
                        return type != null ? type.Root : t.Root;
                    }).Select(t => new TokenValue(GetModifiedType(token, t), parent)).ToList();
                }
                else
                {
                    result = _grammar[token.Type.TypeId]
                        .Select(t => new TokenValue(GetModifiedType(token, t), parent)).ToList();
                }
            }

            if (token.Type.Mutable && token.Type.TypeId != "expression")
            {
                result.Insert(0, FindExpression(token));
            }

            if (!string.IsNullOrEmpty(value))
            {
                if (token.Type.TypeId == "field_name")
                {
                    _data[token.Type.TypeId] = TypesInfo.ActiveTypes.Single(table => table.DbName == value)
                        .Attributes.Select(a => new TokenValue(a.DisplayName, a.DbName)).ToList();
                }

                return GetVariants(token);
            }

            return result;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}