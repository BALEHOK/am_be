using AppFramework.ConstantsEnumerators;
using System;

namespace AppFramework.Core.DataTypes
{
    public class DateTimeDataType : DataTypeBase
    {
        public override string Name
        {
            get { return "DateTime"; }
        }

        public override Type FrameworkDataType
        {
            get { return typeof (DateTime); }
        }

        public override int? StringSize
        {
            get { throw new NotSupportedException(); }
        }

        public override Enumerators.DataType Code
        {
            get { return Enumerators.DataType.DateTime; }
            set { }
        }
    }
}
