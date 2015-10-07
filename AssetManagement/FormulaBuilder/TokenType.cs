using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace FormulaBuilder
{
    [Serializable]
    public class TokenType
    {
        public static string[] ValueTypes = { "table_name", "field_name", "asset_value", "related_value" };

        public static List<TokenType> Get = new List<TokenType>
        {
            new TokenType
            {
                TypeId = "expression",
                Name = "[...]",
            },
            new TokenType
            {                
                TypeId = "table_name",
                Open = "[",
                Close = "]",
                Name = "Asset type",
            },
            new TokenType
            {                
                TypeId = "field_name",
                Open = "[",
                Close = "]",
                Name = "Field Name",
                DependsOn = "table_name",
            },
            new TokenType
            {
                Root =  true,
                TypeId = "asset_value",
                Open = "[",
                Close = "]",
                Name = "Asset field value",
            },
            new TokenType
            {
                Root =  true,
                TypeId = "related_value",
                Open = "[",
                Close = "]",
                Name = "Related asset field value",
            },
            new TokenType
            {
                Root = true,
                Open = "(",
                Close = ")",
                TypeId = "function",
                Name = "Function",
            },
            new TokenType
            {
                Root = true,
                Open = "(",
                Close = ")",
                TypeId = "if_condition",
                Name = "If condition",
            },
            new TokenType
            {
                Root = true,    
                TypeId = "operator",
                Name = "Operator",
            },
            new TokenType
            {
                Root = true,    
                Open = "[#",
                Close = "]",
                TypeId = "variable",
                Name = "Variable",
            },
        };

        private TokenType _dependencyType;

        public event EventHandler<TokenUpdateEvent> OnChanged;
        public event EventHandler<TokenUpdateEvent> OnDependencyChanged;

        public List<TokenType> RightParameters = new List<TokenType>();
        public List<TokenType> LeftParameters = new List<TokenType>();

        public string Name { get; set; }
        public string Open { get; set; }
        public string Close { get; set; }
        public bool Mutable { get; set; }
        public string TypeId { get; set; }
        public string DependsOn { get; set; }
        public bool Root { get; set; }
        public List<TokenType> TypeOverrides { get; set; }
        public TokenType Parent { get; set; }

        public TokenType DependencyType
        {
            get { return _dependencyType; }
            set
            {
                _dependencyType = value;

                DependencyType.OnChanged += (sender, args) =>
                {
                    if (OnDependencyChanged != null)
                        OnDependencyChanged(this, new TokenUpdateEvent(DependencyType));
                };
            }
        }

        private TokenType()
        {
            Mutable = true;
            TypeOverrides = new List<TokenType>();
        }

        public bool IsValue
        {
            get { return ValueTypes.Contains(TypeId); }
        }

        private TokenType AddParameter(TokenType parameter, bool isLeft = false)
        {
            if (!isLeft)
                RightParameters.Add(parameter);
            else
                LeftParameters.Add(parameter);

            parameter.Parent = this;

            if (!string.IsNullOrEmpty(parameter.DependsOn))
            {
                var source = RightParameters.Single(p => p.TypeId == parameter.DependsOn);
                parameter.DependencyType = source;
            }

            return this;
        }

        public TokenType AddRightParameter(TokenType parameter)
        {
            return AddParameter(parameter);
        }

        public TokenType AddLeftParameter(TokenType parameter)
        {
            return AddParameter(parameter, true);
        }

        public TokenType OverrideType(TokenType type)
        {
            type.Parent = this;
            TypeOverrides.Add(type);

            return this;
        }

        public void FireValueUpdate(string value)
        {
            if (OnChanged != null)
                OnChanged(this, new TokenUpdateEvent(null, value));
        }

        public TokenType SetName(string name)
        {
            Name = name;
            return this;
        }

        public TokenType IsRoot(bool root)
        {
            Root = root;
            return this;
        }

        public TokenType IsMutable(bool mutable)
        {
            Mutable = mutable;
            return this;
        }

        public TokenType Opn(string open)
        {
            Open = open;
            return this;
        }

        public TokenType Cls(string close)
        {
            Close = close;
            return this;
        }

        public static TokenType T(string typeId)
        {
            if (typeId == null)
                return T("expression");

            var existingType = Get.Single(t => t.TypeId == typeId);

            if (existingType == null)
                throw new ArgumentException("type not found: " + typeId);

            var newType = DeepClone(existingType);

            return newType;
        }

        public static T DeepClone<T>(T obj)
        {
            using (var memoryStream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, obj);
                memoryStream.Position = 0;
                return (T)formatter.Deserialize(memoryStream);
            }
        }
    }
}