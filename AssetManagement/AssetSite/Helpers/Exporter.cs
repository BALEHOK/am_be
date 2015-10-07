using AppFramework.ConstantsEnumerators;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.IE;
using AppFramework.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using WarstarDev.Office2k7;
using WarstarDev.Office2k7.opc_contentTypeItem;
using WarstarDev.Office2k7.opc_relationshipPart.rs;
using WarstarDev.Office2k7.wml.w;

namespace AssetSite.Helpers
{
    public class Exporter
    {
        private const string QUOTE = "\"";
        private const string ESCAPED_QUOTE = "\"\"";
        private static char[] CHARACTERS_THAT_MUST_BE_QUOTED = { ',', '"', '\n' };
        private readonly IAssetTypeRepository _assetTypeRepository;
        private readonly IAssetsService _assetsService;

        public Exporter(IAssetTypeRepository assetTypeRepository, IAssetsService assetsService)
        {
            if (assetTypeRepository == null)
                throw new ArgumentNullException();
            _assetTypeRepository = assetTypeRepository;
            if (assetsService == null)
                throw new ArgumentNullException();
            _assetsService = assetsService;
        }

        public Stream ExportToTxt(AppFramework.Core.Classes.Asset asset)
        {
            var result = new MemoryStream();
            var aType = asset.GetConfiguration();

            foreach (AssetAttribute atrib in asset.AttributesPublic)
            {
                if (atrib.GetConfiguration().DataType.Code == Enumerators.DataType.Image ||
                    atrib.GetConfiguration().DataType.Code == Enumerators.DataType.File ||
                    atrib.GetConfiguration().DataType.Code == Enumerators.DataType.Password)
                    continue;

                string oneString = string.Empty;
                var atribConfig = atrib.GetConfiguration();

                switch (atrib.GetConfiguration().DataType.Code)
                {
                    case Enumerators.DataType.Asset:
                        if (atrib.RelatedAsset == null) break;
                        oneString = atribConfig.NameLocalized + " : " + atrib.RelatedAsset.GetDisplayName(atrib.GetConfiguration().RelatedAssetTypeAttributeID.Value);
                        break;
                    case Enumerators.DataType.Assets:
                        if (atrib.MultipleAssets.Count == 0) break;

                        oneString = atribConfig.NameLocalized + " : " + atrib.MultipleAssets[0].Value;

                        if (atrib.MultipleAssets.Count > 1)
                        {
                            for (int i = 1; i < atrib.MultipleAssets.Count; i++)
                            {
                                oneString += " , " + atrib.MultipleAssets[i].Value;
                            }
                        }
                        break;
                    case Enumerators.DataType.DynList:
                        if (atrib.DynamicListValues.Count == 0) break;
                        oneString = atribConfig.NameLocalized + " : " + atrib.DynamicListValues[0].Value.Replace("<br/>", ",");

                        if (atrib.DynamicListValues.Count > 1)
                        {
                            for (int i = 1; i < atrib.DynamicListValues.Count; i++)
                            {
                                oneString += ", " + atrib.DynamicListValues[i].Value.Replace("<br/>", ",");
                            }
                        }
                        break;
                    case Enumerators.DataType.Document:
                        if (atrib.ValueAsId.HasValue)
                        {
                            oneString = atribConfig.NameLocalized + " : ";
                            var documentAssetType = _assetTypeRepository.GetById(PredefinedAttribute.Get(PredefinedEntity.Document).DynEntityConfigID);
                            var doc = _assetsService.GetAssetById(atrib.ValueAsId.Value, documentAssetType);
                            if (doc != null)
                                oneString += doc.Name;
                        }
                        break;
                    default:
                        oneString = atribConfig.NameLocalized + " : " + atrib.Value;
                        break;
                }

                byte[] buf = Encoding.UTF8.GetBytes(oneString + '\n');
                result.Write(buf, 0, buf.Length);
                result.Flush();
            }

            result.Position = 0;

            return result;
        }

        public static Stream ExportToXml(AppFramework.Core.Classes.Asset asset)
        {
            MemoryStream result = ImportExportManager.ExportSingleAssetToMemory(asset);
            result.Position = 0;
            return result;
        }

        public Stream ExportTodoc(AppFramework.Core.Classes.Asset asset, string appRootFolder, out string filePath)
        {
            string tempPath = appRootFolder + "\\Temp\\";

            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);

            //WordDocument wdDoc = new WordDocument(tempPath, true);
            WordDocument wdDoc = new WordDocument();
            wdDoc.CustomDirectory = tempPath;
            wdDoc.Document = new Document();
            wdDoc.Document.Body = new CT_Body();

            AssetType aType = asset.GetConfiguration();

            foreach (AssetAttribute atrib in asset.AttributesPublic)
            {
                if (atrib.GetConfiguration().DataType.Code == Enumerators.DataType.Image ||
                    atrib.GetConfiguration().DataType.Code == Enumerators.DataType.File ||
                    atrib.GetConfiguration().DataType.Code == Enumerators.DataType.Password)
                    continue;

                CT_P dataContainer = new CT_P();

                CT_R rowTitle = new CT_R();
                rowTitle.rPr = new CT_RPr();
                rowTitle.rPr.b = true;

                CT_R rowValue = new CT_R();

                AssetTypeAttribute atribConfig = atrib.GetConfiguration();

                bool secondTab = ((atribConfig.NameLocalized + " :").Length <= 6);

                switch (atrib.GetConfiguration().DataType.Code)
                {
                    case Enumerators.DataType.Asset:
                        if (atrib.RelatedAsset == null) break;

                        rowTitle.Text = atribConfig.NameLocalized + " :";
                        rowValue.Text = atrib.RelatedAsset.GetDisplayName(atrib.GetConfiguration().RelatedAssetTypeAttributeID.Value);
                        break;
                    case Enumerators.DataType.Assets:
                        if (atrib.MultipleAssets.Count == 0) break;

                        rowTitle.Text = atribConfig.NameLocalized + " :";
                        rowValue.Text = atrib.MultipleAssets[0].Value;

                        if (atrib.MultipleAssets.Count > 1)
                        {
                            for (int i = 1; i < atrib.MultipleAssets.Count; i++)
                            {
                                rowValue.Text += " , " + atrib.MultipleAssets[i].Value;
                            }
                        }
                        break;
                    case Enumerators.DataType.DynList:
                        if (atrib.DynamicListValues.Count == 0) break;
                        rowTitle.Text = atribConfig.NameLocalized + " :";
                        rowValue.Text = atrib.DynamicListValues[0].Value.Replace("<br/>", ",");

                        if (atrib.DynamicListValues.Count > 1)
                        {
                            for (int i = 1; i < atrib.DynamicListValues.Count; i++)
                            {
                                rowValue.Text += ", " + atrib.DynamicListValues[i].Value.Replace("<br/>", ",");
                            }
                        }
                        break;
                    case Enumerators.DataType.Document:
                        if (atrib.ValueAsId.HasValue)
                        {
                            rowTitle.Text = atribConfig.NameLocalized + " :";
                            var documentAssetType = _assetTypeRepository.GetById(PredefinedAttribute.Get(PredefinedEntity.Document).DynEntityConfigID);
                            var doc = _assetsService.GetAssetById(atrib.ValueAsId.Value, documentAssetType);
                            if (doc != null)
                                rowValue.Text = doc.Name;
                        }
                        break;
                    default:
                        rowTitle.Text = atribConfig.NameLocalized + " :";
                        rowValue.Text = atrib.Value;
                        break;
                }

                dataContainer.Content.Add(rowTitle);
                dataContainer.Content.Add(new CT_R() { tab = true });
                if (secondTab)
                    dataContainer.Content.Add(new CT_R() { tab = true });
                dataContainer.Content.Add(rowValue);

                wdDoc.Document.Body.Content.Add(dataContainer);
            }

            wdDoc.ContentItems = new CT_Types();
            wdDoc.ContentItems.Default.Add(new CT_Default("xml", "application/xml"));
            wdDoc.ContentItems.Default.Add(new CT_Default("rels", "application/vnd.openxmlformats-package.relationships+xml"));
            wdDoc.ContentItems.Override.Add(new CT_Override("application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml", "/word/document.xml"));

            wdDoc.LoadRootRels();
            wdDoc.LoadDocumentRels();
            CT_Relationship rel = new CT_Relationship();
            rel.Id = "rId1";
            rel.Target = "word/document.xml";
            rel.Type = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument";
            wdDoc.RootRels.Relationship.Add(rel);

            wdDoc.TempSaveRootRels();
            wdDoc.TempSaveContentItems();

            filePath = tempPath + "export" + DateTime.Now.ToLongTimeString().Replace(".", "_").Replace(":", "_") + ".docx";

            wdDoc.Save(filePath, true);

            Stream result = File.OpenRead(filePath);

            return result;
        }
    }
}