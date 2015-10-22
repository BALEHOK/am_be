 using System;
 using System.Web.Security;
using System.Web.UI;
 using AppFramework.ConstantsEnumerators;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
 using Microsoft.Practices.Unity;

namespace AppFramework.Core.PL
{
    public interface IAttributeFieldFactory
    {
        /// <summary>
        /// Returns appropriate Web UI Control for given attribute
        /// </summary>
        IAssetAttributeControl GetControl(AssetAttribute attribute, bool editable, bool mysettingpage);
    }

    public class AttributeFieldFactory : IAttributeFieldFactory
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ContolDependencyResolver _resolver;

        public AttributeFieldFactory(
            IAuthenticationService authenticationService, 
            IUnityContainer container)
        {
            if (authenticationService == null)
                throw new ArgumentNullException();
            _authenticationService = authenticationService;
            if (container == null)
                throw new ArgumentNullException("container");
            _resolver = new ContolDependencyResolver(container);
        }

        /// <summary>
        /// Returns appropriate Web UI Control for given attribute
        /// </summary>
        /// <param name="attribute">Asset Attribute</param>
        /// <param name="editable">Is editing available or not</param>
        /// <param name="handlerID">ID of parent object.</param>
        /// <param name="bindable">Is returned control will retrieve data on data binding or on instansing</param>
        /// <returns>Web UI Control</returns>
        public IAssetAttributeControl GetControl(AssetAttribute attribute, bool editable, bool mysettingpage)
        {
            IAssetAttributeControl ctrl;
            switch (attribute.GetConfiguration().DataTypeEnum)
            {
                case Enumerators.DataType.Asset:
                    if (attribute.GetConfiguration().IsUpdateUser)
                    {
                        if (editable)
                            attribute.ValueAsId = _authenticationService.CurrentUserId;
                        ctrl = new AssetAttributeHyperlink(attribute);
                        (ctrl as AssetAttributeHyperlink).Text = 
                            _authenticationService.CurrentUser.UserName;
                    }
                    else
                    {
                        if (editable)
                        {
                            ctrl = (new Page()).LoadControl("~/Controls/AssetDropDownListEx.ascx") 
                                as IAssetAttributeControl;
                            ctrl.Editable = editable;
                            ctrl.AssetAttribute = attribute;
                        }
                        else
                        {
                            ctrl = new AssetAttributeHyperlink(attribute);
                        }
                    }
                    break;

                case Enumerators.DataType.Assets:
                    if (editable)
                        ctrl = (new Page()).LoadControl("~/Controls/MultipleAssetsEditor.ascx") as IAssetAttributeControl;
                    else
                        ctrl = (new Page()).LoadControl("~/Controls/MultipleAssetsGrid.ascx") as IAssetAttributeControl;
                    ctrl.Editable = editable;
                    ctrl.AssetAttribute = attribute;
                    break;

                case Enumerators.DataType.DynList:
                    ctrl = (new Page()).LoadControl("~/Controls/DynListDDHolder.ascx") as IAssetAttributeControl;
                    ctrl.Editable = editable;
                    ctrl.AssetAttribute = attribute;
                    break;

                case Enumerators.DataType.DynLists:
                    ctrl = (new Page()).LoadControl("~/Controls/DynListListBox.ascx") as IAssetAttributeControl;
                    ctrl.Editable = editable;
                    ctrl.AssetAttribute = attribute;
                    break;

                case Enumerators.DataType.Password:
                    ctrl = new PasswordControl(attribute, mysettingpage);
                    ctrl.Editable = editable;
                    break;

                case Enumerators.DataType.Permission:
                    ctrl = new PermissionsControl(attribute);
                    // deny to change the permissions for built-in administrator
                    if (Roles.IsUserInRole(PredefinedRoles.Administrators.ToString()) 
                        && PredefinedAsset.Contains(attribute.ParentAsset))
                    {
                        ctrl.Editable = false;
                    }
                    else
                    {
                        ctrl.Editable = editable;
                    }
                    break;

                case Enumerators.DataType.File:
                    if (editable)
                    {
                        ctrl = new FileUploadControl(attribute);
                        ctrl.Editable = true;
                    }
                    else
                    {
                        ctrl = new UrlControl(attribute);
                    }
                    break;

                case Enumerators.DataType.Image:
                    if (editable)
                    {
                        ctrl = new FileUploadControl(attribute);
                        ctrl.Editable = true;
                    }
                    else
                    {
                        ctrl = (new Page()).LoadControl("~/Controls/ImageControl.ascx") as IAssetAttributeControl;
                        ctrl.Editable = false;
                        ctrl.AssetAttribute = attribute;
                    }
                    break;

                case Enumerators.DataType.Document:
                    if (editable)
                    {
                        ctrl = new DocumentControl(attribute)
                        {
                            Editable = true
                        };
                    }
                    else
                    {
                        ctrl = new AssetAttributeHyperlink(attribute);
                    }
                    break;

                case Enumerators.DataType.Role:
                    ctrl = new RolesDropdown(attribute);
                    // deny to change the role for built-in administrator
                    if (Roles.IsUserInRole(PredefinedRoles.Administrators.ToString()) 
                        && PredefinedAsset.Contains(attribute.ParentAsset))
                    {
                        ctrl.Editable = false;
                    }
                    else
                    {
                        ctrl.Editable = editable;
                    }
                    break;

                case Enumerators.DataType.Bool:
                    ctrl = new CheckboxControl(attribute);
                    ctrl.Editable = editable;
                    break;

                case Enumerators.DataType.Url:
                    ctrl = editable
                        ? (IAssetAttributeControl) new AssetAttributeTextbox(attribute) {Editable = true}
                        : new UrlControl(attribute);
                    break;
                case Enumerators.DataType.GoogleMaps:
                    if (editable)
                    {
                        ctrl = (new Page()).LoadControl("~/Controls/GoogleMapsControl.ascx") as IAssetAttributeControl;
                        ctrl.AssetAttribute = attribute;
                        ctrl.Editable = true;
                    }
                    else
                    {
                        ctrl = (new Page()).LoadControl("~/Controls/GoogleMapsControl.ascx") as IAssetAttributeControl;
                        ctrl.AssetAttribute = attribute;
                    }
                    break;

                case Enumerators.DataType.Text:
                    ctrl = editable
                        ? (IAssetAttributeControl) new TextControl(attribute) {Editable = true}
                        : new AssetAttributeLabel(attribute);
                    break;

                case Enumerators.DataType.Place:
                    if (editable)
                    {
                        ctrl = (new Page()).LoadControl("~/Controls/PlaceAndZipControl.ascx") as IAssetAttributeControl;
                        ctrl.AssetAttribute = attribute;
                    }
                    else
                    {
                        ctrl = new AssetAttributeLabel(attribute);
                    }
                    break;

                case Enumerators.DataType.CurrentDate:
                case Enumerators.DataType.DateTime:
                    ctrl = (new Page()).LoadControl("~/Controls/Calendar.ascx") as IAssetAttributeControl;
                    ctrl.Editable = editable;
                    ctrl.AssetAttribute = attribute;
                    break;

                case Enumerators.DataType.Richtext:
                    if (editable)
                    {
                        ctrl = (new Page()).LoadControl("~/Controls/RichtextControl.ascx") as IAssetAttributeControl;
                        ctrl.AssetAttribute = attribute;
                    }
                    else
                    {
                        ctrl = new AssetAttributeLabel(attribute);
                    }
                    break;

                case Enumerators.DataType.Barcode:
                    ctrl = (new Page()).LoadControl("~/Controls/Barcode.ascx") as IAssetAttributeControl;
                    ctrl.Editable = editable;
                    ctrl.AssetAttribute = attribute;
                    break;

                case Enumerators.DataType.Euro:
                case Enumerators.DataType.Money:
                case Enumerators.DataType.Int:
                case Enumerators.DataType.Float:
                case Enumerators.DataType.Long:
                case Enumerators.DataType.USD:
                    ctrl = (new Page()).LoadControl("~/Controls/NumericControl.ascx") as IAssetAttributeControl;
                    ctrl.Editable = editable;
                    ctrl.AssetAttribute = attribute;
                    break;

                default:
                    ctrl = editable && attribute.GetConfiguration().DataType.Editable
                        ? (IAssetAttributeControl) new AssetAttributeTextbox(attribute) {Editable = true}
                        : new AssetAttributeLabel(attribute);
                    break;
            }
            _resolver.InitializeControlTree(ctrl as Control);
            return ctrl;
        }
    }
}
