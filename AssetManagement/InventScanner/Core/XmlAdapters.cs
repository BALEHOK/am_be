/*--------------------------------------------------------
* XmlAdapters.cs
* 
* Copyright: 
* Author: aNesterov
* Created: 8/24/2009 3:51:32 PM
* Purpose: 
* 
* Revisions:
* -------------------------------------------------------*/

namespace InventScanner.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.Xml.Serialization;
    using System.Xml;

    [Serializable]
    public class XmlAdapters : List<XmlAdapter>
    {
        /// <summary>
        /// Saves instance as XML.
        /// </summary>
        /// <param name="path">The path.</param>
        public void SaveAsXml(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                XmlSerializer xmlSerialize = new XmlSerializer(typeof(XmlAdapters));
                using (var fileStream = File.Create(path))
                {
                    xmlSerialize.Serialize(fileStream, this);
                }
            }
            catch (IOException)
            {
                throw;
            }
            catch (XmlException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Loads instance from XML.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Return <see cref="XmlAdapters"/> value</returns>
        public static XmlAdapters LoadFromXml(string path)
        {
            XmlAdapters adapters = null;
            if (File.Exists(path))
            {
                XmlSerializer xmlSerialize = new XmlSerializer(typeof(XmlAdapters));
                using (var fileStream = File.Open(path, FileMode.Open))
                {
                    adapters = xmlSerialize.Deserialize(fileStream) as XmlAdapters;
                }
            }

            return adapters;
        }

        public void InventAsset(string barcode, XmlAdapter sender)
        {
            bool found = false;
            var result = sender.InventAsset(barcode);

            //  if it's not dublicate - i.e. not yet scanned
            if (!result.Duplicate)
            {
                // if asset found in sender assets table
                if (result.Status == AppFramework.Core.Classes.Stock.LogRecordStatus.OK)
                {
                    // insert in log table and make other processing
                    result.CurrentLocation = sender.LocationName;
                    result.CurrentLocationId = sender.LocationId;
                    sender.ProcessInventResult(result);
                }
                else
                {
                    // check this asset in other xmlAdapters
                    foreach (var adapter in this.Where(a => !a.Equals(sender)))
                    {
                        var otherResult = adapter.InventAsset(barcode);
                        if (otherResult.Status == AppFramework.Core.Classes.Stock.LogRecordStatus.OK)
                        {
                            // return to sender result found in other adapter and mark it as warning
                            var senderResult = otherResult;
                            senderResult.Status = AppFramework.Core.Classes.Stock.LogRecordStatus.Warning;
                            senderResult.Message = string.Format("Asset with barcode {0} found in {1} location", barcode, adapter.LocationName);
                            senderResult.CurrentLocation = adapter.LocationName;
                            senderResult.CurrentLocationId = adapter.LocationId;
                            sender.ProcessInventResult(senderResult);

                            found = true;
                        }
                    }

                    // if asset not found in any location, add it as missed
                    if (!found)
                    {
                        sender.ProcessInventResult(result);
                    }
                }
            }
        }
    }
}
