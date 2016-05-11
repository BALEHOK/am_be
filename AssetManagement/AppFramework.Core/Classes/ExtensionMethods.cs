using AppFramework.Core.AC.Authentication;
using AppFramework.Core.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;

namespace AppFramework.Core.Classes
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Glues columns names for SQL queries
        /// </summary>
        /// <param name="columns">List of DynColumnDescription items</param>
        /// <returns>Comma separated list of column names</returns>
        public static string NamesToSQLString(this List<DynColumn> columns)
        {
            StringBuilder sb = new StringBuilder();
            char Comma = ',';
            foreach (DynColumn column in columns)
            {
                sb.Append("[" + column.Name + "]");
                sb.Append(Comma);
            }
            return sb.ToString().TrimEnd(Comma);
        }

        /// <summary>
        /// Adds all rules together and returns the result permission
        /// </summary>
        /// <param name="rules"></param>
        /// <returns></returns>
        public static Permission Or(this IEnumerable<RightsEntry> rules)
        {
            return PermissionsProvider.Or(rules.Select(r => r.Permission).ToArray());
        }

        /// <summary>
        /// Converts the collection of MembershipUsers to assets objects
        /// </summary>
        /// <param name="users">MembershipUserCollection</param>
        /// <returns>IEnumerable of assets</returns>
        public static IEnumerable<Asset> ToAssets(this MembershipUserCollection users)
        {
            foreach (AssetUser user in users)
            {
                yield return user.Asset;
            }
            yield break;
        }

        /// <summary>
        /// Removing all trailing occuriences of comma and whitespace characters
        /// of the current System.String object
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static string TrimEndComma(this string inputString)
        {
            Char[] trimChr = new Char[] { ',', ' ' };
            return inputString.TrimEnd(trimChr);
        }

        /// <summary>
        /// Determines whether collection is null or empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns>
        /// 	<c>true</c> if the specified collection is null or empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            return collection == null || collection.Count() == 0;
        }

        /// <summary>
        /// Converts TreeNodesCollection to simple Enumeration
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static IEnumerable<TreeNode> ToEnumeration(this TreeNodeCollection c)
        {
            foreach (TreeNode n in c) yield return n;
        }

        /// <summary>
        /// Extension method for collections
        /// </summary>
        public static void ForEachWithIndex<T>(this IEnumerable<T> enumerable, Action<T, int> handler)
        {
            int idx = 0;
            foreach (T item in enumerable)
                handler(item, idx++);
        }

        /// <summary>
        /// Returns the display name of asset by ID of attribute
        /// </summary>        
        public static string GetDisplayName(this Asset asset, long attributeId)
        {
            var attribute = asset.Attributes.SingleOrDefault(a => a.GetConfiguration().ID == attributeId);
            return attribute != null ? attribute.Value : string.Empty;
        }

        public static XElement GetXElement(this XmlNode node)
        {
            XDocument xDoc = new XDocument();
            using (XmlWriter xmlWriter = xDoc.CreateWriter())
                node.WriteTo(xmlWriter);
            return xDoc.Root;
        }

        public static XmlNode GetXmlNode(this XElement element)
        {
            using (XmlReader xmlReader = element.CreateReader())
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlReader);
                return xmlDoc;
            }
        }

        public static T NumToEnum<T>(int number)
        {
            return (T)Enum.ToObject(typeof(T), number);
        }

        public static string ToXML<T>(this List<T> input)
        {
            MemoryStream memoryStream = new MemoryStream();
            DataContractSerializer ser = new DataContractSerializer(input.GetType());
            ser.WriteObject(memoryStream, input);

            memoryStream.Position = 0;
            byte[] buffer = new byte[memoryStream.Length];
            memoryStream.Read(buffer, 0, buffer.Length);

            return Encoding.UTF8.GetString(buffer);
        }

        public static List<T> FromXML<T>(this List<T> source, string xml)
        {
            DataContractSerializer ser = new DataContractSerializer(typeof(List<T>));
            MemoryStream tempStream = new MemoryStream();

            byte[] buffer = Encoding.UTF8.GetBytes(xml);
            tempStream.Write(buffer, 0, buffer.Length);
            tempStream.Position = 0;

            return ser.ReadObject(tempStream) as List<T>;
        }
    }
}
