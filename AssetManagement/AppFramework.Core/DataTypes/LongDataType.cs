using AppFramework.ConstantsEnumerators;
using System;

namespace AppFramework.Core.DataTypes
{
    public class LongDataType : DataTypeBase
    {
        public override string Name
        {
            get { return "long"; }
        }

        public override Type FrameworkDataType
        {
            get { return typeof (long); }
        }

        public override int? StringSize
        {
            get { throw new NotSupportedException(); }
        }

        public override Enumerators.DataType Code
        {
            get { return Enumerators.DataType.Long; }
            set { }
        }
    }
}
