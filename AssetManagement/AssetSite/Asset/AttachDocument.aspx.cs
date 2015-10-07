namespace AssetSite.Asset
{
    using AppFramework.Core.Classes;
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;
    using Core = AppFramework.Core.Classes;

    public partial class AttachDocument : BasePage
    {
        private Core.Asset asset;
        private string elementId;
        private string classNew;
        private string classExist;

        protected void Page_Load(object sender, EventArgs e)
        {
            var at =
                    AssetTypeRepository.GetById(
                        PredefinedAttribute.Get(
                            AppFramework.ConstantsEnumerators.PredefinedEntity.Document
                        ).DynEntityConfigID
                    );

            asset = AssetsService.CreateAsset(at);
            this.AssetAttributePanels.Asset = asset;

            if (string.IsNullOrEmpty(Request.QueryString["ElementId"])
                || string.IsNullOrEmpty(Request.QueryString["ClassExist"])
                || string.IsNullOrEmpty(Request.QueryString["ClassNew"]))
            {
                throw new ArgumentException("Bad AttributeUid parameter value");
            }
            else
            {
                this.elementId = Request.QueryString["ElementId"];
                this.classNew = Request.QueryString["ClassNew"];
                this.classExist = Request.QueryString["ClassExist"];
            }
        }

        protected void AttachClicked(object sender, EventArgs e)
        {
            switch (this.DocumentAttachType.SelectedItem.Value)
            {
                case "new":
                    AttachNewDocument();
                    break;
                case "existing":
                    AttachExistingDocument();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Attaches the existing document as return value of popup
        /// </summary>
        public void AttachExistingDocument()
        {

            if (!string.IsNullOrEmpty((ExistingDocument.FindControl("TextBoxHidden") as HiddenField).Value))
            {
                long documentId = long.Parse((ExistingDocument.FindControl("TextBoxHidden") as HiddenField).Value);

                if (documentId != 0)
                {
                    var documentAssetType = AssetTypeRepository.GetById(PredefinedAttribute.Get(AppFramework.ConstantsEnumerators.PredefinedEntity.Document).DynEntityConfigID);
                    var document = AssetsService.GetAssetById(documentId, documentAssetType);
                    if (document != null)
                    {
                        string returnScript = this.GetReturnScript(document);
                        ClientScript.RegisterStartupScript(this.GetType(), "closeWnd", returnScript);
                    }
                }
            }

        }

        /// <summary>
        /// Attaches the new document as return value of popup
        /// </summary>
        private void AttachNewDocument()
        {
            AppFramework.Core.Classes.Asset asset;
            IDictionary<AssetAttribute, AppFramework.Core.Classes.Asset> dependencies;
            var isValid = AssetAttributePanels.TryGetValidAssetWithDependencies(out asset, out dependencies);
            if (isValid)
            {
                AssetsService.InsertAsset(asset, dependencies);
                string returnScript = this.GetReturnScript(asset);
                ClientScript.RegisterStartupScript(this.GetType(), "closeWnd", returnScript);
            }
        }

        private string GetReturnScript(Asset ast)
        {
            string link = string.Format(@"<a href=\'{0}\'>{1}</a>", ast.NavigateUrl, ast.Name); ;
            string returnScript = string.Format(
                @"<script type=""text/javascript"">
                        returnAttachedDocument('{0}', '{1}','{2}', '{3}', '{4}');
                     </script>",
                ast.ID,
                link,
                this.elementId,
                this.classExist,
                this.classNew
            );
            return returnScript;
        }

        protected void DocumentAttachType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (DocumentAttachType.SelectedItem.Value)
            {
                case "new":
                    NewDocument.Visible = true;
                    ExistingDocument.Visible = false;
                    break;
                case "existing":
                    ExistingDocument.Visible = true;
                    NewDocument.Visible = false;
                    break;
                default:
                    break;
            }
        }

        //protected void FillDocumentsList()
        //{
        //    // TODO it will be nice to use here autocomplete
        //    this.DocumentsList.DataSource = AssetFactory.GetAllOnlyIdName(PredefinedAttribute.Get(AppFramework.ConstantsEnumerators.PredefinedEntity.Document).DynEntityConfigID);
        //    this.DocumentsList.DataBind();
        //}
    }
}
