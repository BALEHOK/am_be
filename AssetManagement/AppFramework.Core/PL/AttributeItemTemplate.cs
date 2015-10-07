using System.Web.UI;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using Microsoft.Practices.Unity;

namespace AppFramework.Core.PL
{
    /// <summary>
    /// GridView item template for output attribute as column of grid. 
    /// DataSource of grid must be list of Asset.
    /// </summary>
    public class AttributeItemTemplate : ITemplate
    {
        private readonly AssetAttribute _attribute;
        private readonly AttributeFieldFactory _factory;
        private Control _control;

        public AttributeItemTemplate(
            AssetAttribute attribute, 
            IAuthenticationService authenticationService,
            IUnityContainer container)
        {
            _attribute = attribute;
            _factory = new AttributeFieldFactory(authenticationService, container);
        }

        public void InstantiateIn(Control container)
        {
            _control = _factory.GetControl(_attribute, false, false) as Control;
            container.Controls.Add(_control);
        }
    }
}
