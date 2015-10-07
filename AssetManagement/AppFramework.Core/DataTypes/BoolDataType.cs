using AppFramework.ConstantsEnumerators;
using System;

namespace AppFramework.Core.DataTypes
{
    public class BoolDataType : DataTypeBase
    {
        public override string Name
        {
            get { return "bool"; }
        }

        public override Type FrameworkDataType
        {
            get { return typeof (bool); }
        }

        public override int? StringSize
        {
            get { throw new NotSupportedException(); }
        }

        public override Enumerators.DataType Code
        {
            get { return Enumerators.DataType.Bool; }
            set { }
        }
    }
}
