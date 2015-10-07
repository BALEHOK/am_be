using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using AppFramework.Core.Classes.SearchEngine.Enumerations;
using AppFramework.Core.Classes.SearchEngine.TypeSearchElements;

namespace AppFramework.Core.Classes
{
    public class GridViewDataDescriptor
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Context { get; set; }
        public string Categories { get; set; }
        public string DateRevision { get; set; }
        public long ID { get; set; }
        public AssetType AssetType { get; set; }

        public GridViewDataDescriptor(AssetType assetType)
		{
			if (assetType == null)
				throw new ArgumentNullException("AssetType");
			this.AssetType = assetType;
		}
    }

    public class SelectedAssetDescriptor
    {
        public bool IsChecked { get; set; }
        public AssetTypeAttribute Attribute { get; set; }

        public SelectedAssetDescriptor()
        {
            this.Attribute = new AssetTypeAttribute();
        }
    }
    
    [DataContract]
    public class NewAssetTaskParametrsDescriptor
    {
        [DataMember]
        public long? ScreenId { get; set; }

        public NewAssetTaskParametrsDescriptor()
        {
        }

        public string Serialize()
        {
            using (var memoryStream = new MemoryStream())
            {
                var ser = new DataContractSerializer(typeof (NewAssetTaskParametrsDescriptor));
                ser.WriteObject(memoryStream, this);

                memoryStream.Position = 0;
                var buffer = new byte[memoryStream.Length];
                memoryStream.Read(buffer, 0, buffer.Length);

                return Encoding.UTF8.GetString(buffer);
            }
        }

        public static NewAssetTaskParametrsDescriptor Deserialize(string xml)
        {
            var ser = new DataContractSerializer(typeof(NewAssetTaskParametrsDescriptor));
            using (var tempStream = new MemoryStream())
            {
                byte[] buffer = Encoding.UTF8.GetBytes(xml);
                tempStream.Write(buffer, 0, buffer.Length);
                tempStream.Position = 0;

                return ser.ReadObject(tempStream) as NewAssetTaskParametrsDescriptor;
            }
        }
    }

    [DataContract]
    [KnownType(typeof(List<AttributeElement>))]
    public class SearchConfigurationDescriptor
    {
        [DataMember]
        public SearchType SearchType { get; set; }

        [DataMember(IsRequired = false)]        
        public object Params { get; set; }

        [DataMember(IsRequired = false)]
        public TimePeriodForSearch Time { get; set; }

        [DataMember(IsRequired = false)]
        public string ConfigsIds { get; set; }

        [DataMember(IsRequired = false)]
        public string TaxonomyItemsIds { get; set; }

        [DataMember(IsRequired = false)]
        public long TypeUID { get; set; }

        public SearchConfigurationDescriptor()
        {
            //this.Elements = new List<AttributeElement>();
        }

        public string Serialize()
        {
            using (var memoryStream = new MemoryStream())
            {
                var ser = new DataContractSerializer(typeof (SearchConfigurationDescriptor));
                ser.WriteObject(memoryStream, this);
                memoryStream.Position = 0;
                var buffer = new byte[memoryStream.Length];
                memoryStream.Read(buffer, 0, buffer.Length);
                return Encoding.UTF8.GetString(buffer);
            }
        }

        public static SearchConfigurationDescriptor Deserialize(string xml)
        {
            var ser = new DataContractSerializer(typeof(SearchConfigurationDescriptor));
            using (var tempStream = new MemoryStream())
            {
                var buffer = Encoding.UTF8.GetBytes(xml);
                tempStream.Write(buffer, 0, buffer.Length);
                tempStream.Position = 0;
                return ser.ReadObject(tempStream) as SearchConfigurationDescriptor;
            }
        }
    }
}
