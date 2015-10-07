using AppFramework.ConstantsEnumerators;
using System;

namespace AppFramework.Core.DataTypes
{
    public class StringDataType : DataTypeBase
    {
        public override string Name
        {
            get { return "string"; }
        }

        public override Type FrameworkDataType
        {
            get { return typeof (string); }
        }

        public override int? StringSize
        {
            get { return 255; }
        }

        public override Enumerators.DataType Code
        {
            get { return Enumerators.DataType.String; }
            set { }
        }
    }
}
