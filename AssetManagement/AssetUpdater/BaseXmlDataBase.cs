using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.IO;

namespace AssetUpdater
{
    public class BaseXmlDataBase: IDisposable
    {
        private XmlDocument _doc;
        private string _dbPath;

        public BaseXmlDataBase(string path)
        {
            _doc = new XmlDocument();

            if (File.Exists(path))
                _doc.Load(path);
            else
            {
                XmlElement rootElem = _doc.CreateElement("settings");
                XmlDeclaration decl = _doc.CreateXmlDeclaration("1.0", "utf-8", null);

                _doc.AppendChild(decl);
                _doc.AppendChild(rootElem);
            }

            _dbPath = path;
        }

        //private T GetOption<T>(string elementName) where T : class
        //{
        //    T result = default(T);

        //    XmlElement elem = _doc.ChildNodes[1].ChildNodes.Cast<XmlElement>().FirstOrDefault(e => e.Name.ToLower() == elementName.ToLower());

        //    return result;
        //}

        protected string GetOption(string name)
        {
            string result = string.Empty;

            XmlElement elem = _doc.ChildNodes[1].ChildNodes.Cast<XmlElement>().FirstOrDefault(e => e.Name.ToLower() == name.ToLower());
            if (elem == null)
                return string.Empty;

            result = elem.InnerText;

            return result;
        }
        protected void SetOption(string name, string value)
        {
            XmlElement elem = _doc.ChildNodes[1].ChildNodes.Cast<XmlElement>().FirstOrDefault(e => e.Name.ToLower() == name.ToLower());
            if (elem != null)
            {
                _doc.ChildNodes[1].RemoveChild(elem);
            }

            XmlElement nVersion = _doc.CreateElement(name);
            nVersion.InnerText = value;

            _doc.ChildNodes[1].AppendChild(nVersion);
        }

        public void Dispose()
        {
            _doc.Save(_dbPath);
        }
    }
}