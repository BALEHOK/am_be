using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppFramework.Core.Classes.IE
{
    public class ImportBinding
    {
        public long DestinationAttributeId { get; set; }

        public string DataSourceFieldName { get; set; }

        public long? DestinationRelatedAttributeId { get; set; }

        public string DefaultValue { get; set; }
    }
}
