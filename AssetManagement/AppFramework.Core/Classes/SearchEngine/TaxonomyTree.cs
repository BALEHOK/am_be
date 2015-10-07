using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace AppFramework.Core.Classes.SearchEngine
{
    public class TaxonomyTree
    {
        public long Id{get;set;}
        public string Name { get; set; }
        public List<XElement> Tree { get; set; }
        public bool IsCategory { get; set; }
    }
}
