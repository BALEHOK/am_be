using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace AppFramework.Core.Classes
{
    /// <summary>
    /// Abstract generic class which describes any action parameters set
    /// </summary>
    public abstract class ActionParameters<T, TK> : Dictionary<T, TK>
    {
        /// <summary>
        /// Serialize Dictionary to xml 
        /// </summary>
        /// <returns>Xml serialized parameters dictionary</returns>
        public string ToXml()
        {
            var xml = new XElement("parameters",
                from param in this
                select new XElement("param",
                    new XAttribute("Key", param.Key),
                    new XAttribute("Value", param.Value)));
            return xml.ToString();
        }

        public static ActionParameters<T, TK> GetFromXml(string xmlString)
        {
            throw new NotImplementedException();
        }

        protected static T StringToParameter(string value)
        {
            if (Enum.IsDefined(typeof(T), value))
            {
                return Routines.StringToEnum<T>(value);
            }
            else
            {
                throw new ArgumentException(string.Format("Provided key {0} is not valid parameter", value));
            }
        }
    }
}
