/*--------------------------------------------------------
* DocumentControl.cs
* 
* Copyright: 
* Author: aNesterov
* Created: 8/17/2009 4:45:25 PM
* Purpose: 
* 
* Revisions:
* -------------------------------------------------------*/

using AppFramework.Core.AC.Authentication;
using Microsoft.Practices.Unity;

namespace AppFramework.Core.PL
{
    using System;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using AppFramework.ConstantsEnumerators;
    using AppFramework.Core.Classes;

    [Serializable]
    internal struct PageState
    {
        public long DocumentId;
        public long DeleteAttachedDocument;
        public long DeleteAttachedDocumentId;
    }

    public class DocumentControl : UserControl, IAssetAttributeControl
    {
        [Dependency]
        public IAssetsService AssetsService { get; set; }
        [Dependency]
        public IAssetTypeRepository AssetTypeRepository { get; set; }
        [Dependency]
        public IAuthenticationService AuthenticationService { get; set; }

        public AssetAttribute AssetAttribute { get; set; }

        private readonly string documentId;
        private readonly string deleteAttachedDocument;
        private readonly string deleteAttachedDocumentId;

        public void AddAttribute(string name, string value)
        {
            Attributes.Add(name, value);
        }

        private PageState PageState;
        private readonly string classNew, classExist;

        public DocumentControl(AssetAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException("AssetAttribute");
            this.AssetAttribute = attribute;
            long uid = this.AssetAttribute.GetConfiguration().UID;
            this.documentId = string.Format("{0}_DocumentId", uid);
            this.deleteAttachedDocument = string.Format("{0}_DeleteAttachedDocument", uid);
            this.deleteAttachedDocumentId = string.Format("{0}_DeleteAttachedDocumentId", uid);
            this.classExist = string.Format("{0}_classExist", uid);
            this.classNew = string.Format("{0}_classNew", uid);
        }

        public bool Editable
        {
            get;
            set;
        }

        public AssetAttribute GetAttribute()
        {
            long documentId = 0, deleteAttachedDocument = 0, attachedDocumentId = 0;
            long.TryParse(Request.Form[this.documentId], out documentId);
            long.TryParse(Request.Form[this.deleteAttachedDocument], out deleteAttachedDocument);
            long.TryParse(Request.Form[this.deleteAttachedDocumentId], out attachedDocumentId);

            // if user choose to delete old attached document
            if ((documentId != attachedDocumentId) && (deleteAttachedDocument != 0))
            {
                var documentAssetType = AssetTypeRepository.GetById(
                    PredefinedAttribute.Get(PredefinedEntity.Document).DynEntityConfigID);
                if (documentAssetType != null && attachedDocumentId != 0)
                {
                    var asset = AssetsService.GetAssetById(attachedDocumentId, documentAssetType);
                    var permission = AuthenticationService.GetPermission(asset);
                    if (!permission.CanDelete())
                        throw new Exception("User has no permission to delete asset");
                    AssetsService.DeleteAsset(asset);
                }
            }

            // save values to control state
            this.PageState.DocumentId = documentId;
            this.PageState.DeleteAttachedDocument = deleteAttachedDocument;
            this.PageState.DeleteAttachedDocumentId = attachedDocumentId;

            this.AssetAttribute.ValueAsId = documentId;
            return this.AssetAttribute;
        }

        protected void ImageButton_Click(object sender, ImageClickEventArgs e)
        {
            long tmp = 0;
            long.TryParse(Request.Form["DeleteAttachedDocument"], out tmp);
            bool deleteAttached = tmp != 0;
        }

        protected override void OnInit(EventArgs e)
        {
            Page.RegisterRequiresControlState(this);
            base.OnInit(e);

            // on first load, initialize state with attribute values
            if (!IsPostBack)
            {
                if (AssetAttribute.ValueAsId.HasValue)
                    this.PageState.DocumentId = AssetAttribute.ValueAsId.Value;
                this.PageState.DeleteAttachedDocument = 0;
                this.PageState.DeleteAttachedDocumentId = 0;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (this.Editable)
            {
                this.Controls.Clear();

                // first, generate all controls - for already attached document and for new document
                // all manipulation with visibility will be made in JavaScript on client, so until postback
                // no changes will take effect on asset
                //AssetAttributeHyperlink documentLink = new AssetAttributeHyperlink(this.attribute)
                //{
                //    CssClass = classExist
                //};
                HyperLink documentLink = new HyperLink()
                {
                    CssClass = classExist
                };

                var documentAssetType = AssetTypeRepository.GetById(
                    PredefinedAttribute.Get(PredefinedEntity.Document).DynEntityConfigID);
                if (AssetAttribute.ValueAsId > 0)
                {
                    Asset doc = AssetsService.GetAssetById(AssetAttribute.ValueAsId.Value, documentAssetType);
                    if (doc != null)
                    {
                        documentLink.Text = doc.Name;
                        documentLink.NavigateUrl = doc.NavigateUrl;
                    }
                    else
                    {
                        this.AssetAttribute.ValueAsId = 0;//for the previously "corrupted" assets
                    }
                }
                this.Controls.Add(documentLink);

                HyperLink detachImageButton = new HyperLink()
                {
                    CssClass = classExist,
                    ImageUrl = @"/images/buttons/delete.png",
                    NavigateUrl = "#"
                };
                string script = string.Format(
                    "return detachDocument('{0}','{1}','{2}','{3}','{4}','{5}');",
                    this.AssetAttribute.ValueAsId,
                    this.documentId,
                    this.deleteAttachedDocument,
                    this.deleteAttachedDocumentId,
                    this.classExist,
                    this.classNew);
                detachImageButton.Attributes.Add("onclick", script);
                this.Controls.Add(detachImageButton);

                string attachScript = string.Format("return attachDocument('{0}', '{1}', '{2}');", this.documentId, this.classExist, this.classNew);
                LinkButton attachLink = new LinkButton()
                {
                    OnClientClick = attachScript,
                    Text = "Attach document",
                    PostBackUrl = "#",
                    CssClass = classNew
                };
                this.Controls.Add(attachLink);

                // creating ot hidden field made through Literal control, not HtmlControlHidder
                // because it's impossible now set static ID to control for using in JS
                Literal hiddenFields = new Literal()
                {
                    Text = string.Format(
                            @"<input type=""hidden"" id=""{0}"" name=""{0}"" value=""{3}"" />
                             <input type=""hidden"" id=""{1}"" name=""{1}"" value=""{4}"" />
                             <input type=""hidden"" id=""{2}"" name=""{2}"" value=""{5}"" />",
                            this.documentId,
                            this.deleteAttachedDocument,
                            this.deleteAttachedDocumentId,
                            this.PageState.DocumentId,
                            this.PageState.DeleteAttachedDocument,
                            this.PageState.DeleteAttachedDocumentId)
                };

                this.Controls.Add(hiddenFields);

                // if document already attached to asset
                if (AssetAttribute.ValueAsId.HasValue && AssetAttribute.ValueAsId > 0)
                {
                    attachLink.Attributes.Add("style", "display:none");
                }
                else
                {
                    detachImageButton.Attributes.Add("style", "display:none");
                    documentLink.Attributes.Add("style", "display:none");
                }
            }
            else
            {
                this.Controls.Clear();
            }
        }

        protected override object SaveControlState()
        {
            return this.PageState;
        }

        protected override void LoadControlState(object savedState)
        {
            this.PageState = (PageState)savedState;
        }
    }
}