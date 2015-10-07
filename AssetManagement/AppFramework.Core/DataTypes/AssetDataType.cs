using AppFramework.ConstantsEnumerators;
using System;

namespace AppFramework.Core.DataTypes
{
    public class AssetDataType : DataTypeBase
    {
        public override string Name
        {
            get { return "asset"; }
        }

        public override Type FrameworkDataType
        {
            get { return typeof (Classes.Asset); }
        }

        public override int? StringSize
        {
            get { throw new NotSupportedException(); }
        }

        public override Enumerators.DataType Code
        {
            get { return Enumerators.DataType.Asset; }
            set { }
        }

        public AssetDataType()
        {
            
        }
    }
}
