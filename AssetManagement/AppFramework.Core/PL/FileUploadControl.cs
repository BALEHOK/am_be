using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using AppFramework.Core.Classes;
using AppFramework.Core.Helpers;

namespace AppFramework.Core.PL
{
    public class FileUploadControl : WebControl, IAssetAttributeControl
    {
        #region Properties
        public AssetAttribute AssetAttribute { get; set; }

        public void AddAttribute(string name, string value)
        {
            Attributes.Add(name, value);
        }

        private FileUpload _fileControl;
        private HyperLink _hyperlinkControl;
        private ImageButton _buttonDelete;
        private bool IsDeleted
        {
            get
            {
                return HttpContext.Current.Session[this.UniqueID + "deleted"] != null
                    && bool.Parse(HttpContext.Current.Session[this.UniqueID + "deleted"].ToString()) == true;
            }
            set
            {
                HttpContext.Current.Session[this.UniqueID + "deleted"] = value;
            }
        }

        private string FilePath
        {
            get
            {
                if (HttpContext.Current.Session[this.UniqueID + "filePath"] != null)
                {
                    return HttpContext.Current.Session[this.UniqueID + "filePath"].ToString();
                }

                return string.Empty;
            }
            set
            {
                HttpContext.Current.Session[this.UniqueID + "filePath"] = value;
            }
        }
        #endregion

        public FileUploadControl(AssetAttribute attribute)
        {
            this.AssetAttribute = attribute;

            _fileControl = new FileUpload();
            _hyperlinkControl = new HyperLink() { CssClass = "image" };
            _buttonDelete = new ImageButton()
            {
                ImageUrl = "/images/buttons/delete.png",
                AlternateText = "delete",
                CssClass = "deleteImage",
                ImageAlign = ImageAlign.AbsMiddle
            };
            _buttonDelete.Click += new System.Web.UI.ImageClickEventHandler(_buttonDelete_Click);

            this.Controls.Add(_fileControl);
            this.Controls.Add(_hyperlinkControl);
            this.Controls.Add(_buttonDelete);
        }

        protected void _buttonDelete_Click(object sender, EventArgs e)
        {
            this.AssetAttribute.Value = string.Empty;
            FilePath = String.Empty;
            this.IsDeleted = true;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (string.IsNullOrEmpty(AssetAttribute.Value))
            {
                _fileControl.Visible = true;
                _hyperlinkControl.Visible = false;
                _buttonDelete.Visible = false;
            }
            else
            {
                _fileControl.Visible = false;
                _hyperlinkControl.Visible = true;
                _buttonDelete.Visible = true;
                _hyperlinkControl.Text = _hyperlinkControl.NavigateUrl = AssetAttribute.Value;
            }
        }

        #region IAssetAttributeControl Members

        public AssetAttribute GetAttribute()
        {
            if ((FilePath != null && FilePath != String.Empty) && (!_fileControl.HasFile))
            {
                this.AssetAttribute.Value = FilePath;
                return this.AssetAttribute;
            }

            if (_fileControl.HasFile)
            {
                string uploadFolder = ApplicationSettings.UploadFolder;
                bool isNetworkPath = false;
                bool.TryParse(ApplicationSettings.IsNetworkUploadFolder, out isNetworkPath);

                string destinationFolder;
                DirectoryInfo folder;
                string relativePath;
                string ext = _fileControl.FileName.Split(new char[] { '.' }).Last().ToLower();

                if (!isNetworkPath)
                {
                    destinationFolder = string.Format("/{0}/{1}",
                        uploadFolder.Trim(new char[] { '/', '\\' }),
                        Randomization.GetLetter());

                    folder = new DirectoryInfo(HttpContext.Current.Server.MapPath(destinationFolder));
                    relativePath = string.Format(@"{0}/{1}", destinationFolder, string.Format("{0}.{1}", Guid.NewGuid(), ext));
                }
                else
                {
                    destinationFolder = string.Format("{0}\\{1}", uploadFolder, Randomization.GetLetter());
                    folder = new DirectoryInfo(destinationFolder);
                    relativePath = string.Format(@"{0}\\{1}", destinationFolder, string.Format("{0}.{1}", Guid.NewGuid(), ext));
                }

                if (!folder.Exists)
                    folder.Create();

                if (!isNetworkPath)
                {
                    _fileControl.SaveAs(HttpContext.Current.Server.MapPath(relativePath));
                }
                else
                {
                    _fileControl.SaveAs(relativePath);
                }
                this.AssetAttribute.Value = relativePath;

                FilePath = relativePath;
            }
            else if (IsDeleted)
            {
                this.AssetAttribute.Value = string.Empty;

                FilePath = String.Empty;

                IsDeleted = false;
            }
            return this.AssetAttribute;
        }

        public bool Editable { get; set; }

        #endregion
    }
}
