using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppFramework.Core.Interfaces;
using System.Xml;

namespace AppFramework.Core.Classes.IE.Providers
{
    public interface IEntityProvider
    {
        string Serialize(IEnumerable<object> entities, Type objType);
        IEnumerable<object> Deserialize(Type objType);
        
        IEnumerable<object> GetEntities();
        
        List<string> GetFields();

        void Save(string xmldata);
 
        StatusInfo Status { get; set; }
    }   
}
