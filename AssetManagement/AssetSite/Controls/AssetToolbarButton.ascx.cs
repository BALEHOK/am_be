using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using AppFramework.ConstantsEnumerators;

namespace AssetSite.Controls
{
    public partial class AssetToolbarButton : System.Web.UI.UserControl, AppFramework.Core.PL.IToolbarButton
    {
        public Action OnButtonClick { get; set; }

        private const string ImgUrlPath = "/images/buttons/{0}";
        private const string Prefixlbl = "lbl";

        private string _imgURL;
        private string _labelText;
        private Enumerators.ToolbarButtonType _buttonType;
        private IDictionary<string, string> _options;

        public AssetToolbarButton()
        {

        }

        /// <summary>
        /// Button constructor
        /// </summary>
        /// <param name="buttonType">Button type</param>
        public AssetToolbarButton(Enumerators.ToolbarButtonType buttonType, IDictionary<string, string> options)
        {
            _buttonType = buttonType;
            _options = options;
        }

        void restoreButton_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (OnButtonClick != null)
                OnButtonClick();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            switch (_buttonType)
            {
                case Enumerators.ToolbarButtonType.Print:
                    _imgURL = String.Format(ImgUrlPath, "print.jpg");
                    _labelText = "Print";
                    btnContainer.Attributes.Add("onclick", "return window.print();");
                    break;

                case Enumerators.ToolbarButtonType.History:
                    _imgURL = String.Format(ImgUrlPath, "history.jpg");
                    _labelText = "History";
                    btnContainer.Attributes.Add("onclick",
                                                String.Format(
                                                    "location.href='/Asset/History.aspx?AssetID={0}&AssetTypeID={1}'",
                                                    _options["AssetID"], _options["AssetTypeID"]));
                    break;

                case Enumerators.ToolbarButtonType.Edit:
                    _imgURL = String.Format(ImgUrlPath, "edit.png");
                    _labelText = "Edit";
                    string editUrl = string.Format("/Asset/Edit.aspx?AssetUID={0}&AssetTypeUID={1}",
                                                   _options["AssetUID"], _options["AssetTypeUID"]);
                    long screenId;
                    if (Request["ScreenId"] != null && long.TryParse(Request["ScreenId"].ToString(), out screenId))
                    {
                        editUrl += "&ScreenId=" + screenId;
                    }
                    btnContainer.Attributes.Add("onclick", string.Format("location.href='{0}'", editUrl));
                    break;

                case Enumerators.ToolbarButtonType.Delete:
                    _imgURL = String.Format(ImgUrlPath, "delete.jpg");
                    _labelText = "Delete";
                    btnContainer.Attributes.Add("onclick",
                                                string.Format("DeleteAsset({0},{1});", _options["AssetTypeUID"],
                                                              _options["AssetID"]));
                    break;
                case Enumerators.ToolbarButtonType.Documents:
                    _imgURL = String.Format(ImgUrlPath, "attach.jpg");
                    btnContainer.Attributes.Add("onclick", _options["ExternalScript"]);
                    _labelText = "Documents";
                    break;

                case Enumerators.ToolbarButtonType.Undo:
                    if (_options.ContainsKey("AssetID") && _options.ContainsKey("AssetTypeID"))
                    {
                        _imgURL = String.Format(ImgUrlPath, "undo.png");
                        var screenIdParameter = Request["ScreenId"] != null ? "&ScreenId=" + Request["ScreenId"] : "";
                        btnContainer.Attributes.Add("onclick",
                                                    string.Format(
                                                        "location.href='/Asset/View.aspx?AssetId={0}&AssetTypeId={1}{2}'",
                                                        _options["AssetID"], _options["AssetTypeID"], screenIdParameter));
                        _labelText = "Undo";
                    }
                    else
                    {
                        btnContainer.Visible = false;
                    }
                    break;

                case Enumerators.ToolbarButtonType.Save:
                    _imgURL = String.Format(ImgUrlPath, "save.png");
                    _labelText = "Save";
                    btnContainer.Attributes.Add("onclick", "return utilities_SaveAsset()");
                                                
                    break;

                case Enumerators.ToolbarButtonType.CurrentVersion:
                    _imgURL = String.Format(ImgUrlPath, "curver.png");
                    _labelText = "CurrentVersion";

                    btnContainer.Attributes.Add("onclick",
                                                String.Format(
                                                    "location.href='/Asset/View.aspx?AssetID={0}&AssetTypeID={1}'",
                                                    _options["AssetID"], _options["AssetTypeID"]));
                    break;

                case Enumerators.ToolbarButtonType.SaveAndAdd:
                    _imgURL = String.Format(ImgUrlPath, "curver.png");
                    _labelText = "SaveAndAddNew";
                    btnContainer.Attributes.Add("onclick", "return utilities_SaveAssetAndAddNew()");
                                                
                    break;

                case Enumerators.ToolbarButtonType.Template:
                    _imgURL = String.Format(ImgUrlPath, "SaveAsTemplate.png");
                    _labelText = "SaveAsTemplate";
                    btnContainer.Attributes.Add("onclick",
                                                "return __doPostBack('ctl00$ctl00$PlaceHolderMainContent$PlaceHolderMiddleColumn$btnSaveTemplate','')");
                    break;

                case Enumerators.ToolbarButtonType.Restore:
                    var restoreButton = new ImageButton
                        {
                            ImageUrl = string.Format(ImgUrlPath, "undo.png"),
                            AlternateText = (string)GetLocalResourceObject(Prefixlbl + "Restore"),
                            ToolTip = (string)GetLocalResourceObject(Prefixlbl + "Restore")
                        };
                    restoreButton.Click += restoreButton_Click;
                    btnContainer.Controls.Clear();
                    btnContainer.Controls.Add(restoreButton);
                    btnContainer.Attributes.Add("class", "button btn-restore");
                    break;
            }
            buttonImage.ImageUrl = _imgURL;
            buttonText.Text = (string)GetLocalResourceObject(Prefixlbl + _labelText);
        }

        public Enumerators.ToolbarButtonType ButtonType
        {
            get { return _buttonType; }
        }
    }
}