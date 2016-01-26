using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using AppFramework.Core.PL;
using AppFramework.DataProxy;
using Common.Logging;
using Microsoft.Practices.Unity;

namespace AssetSite
{
    public class BasePage : System.Web.UI.Page
    {
        [Dependency]
        public IAuthenticationService AuthenticationService { get; set; }
        [Dependency]
        public ILinkedEntityFinder LinkedEntityFinder { get; set; }
        [Dependency]
        public IAttributeValueFormatter AttributeValueFormatter { get; set; }
        [Dependency]
        public IUnitOfWork UnitOfWork { get; set; }
        [Dependency]
        public IAssetTypeRepository AssetTypeRepository { get; set; }
        [Dependency]
        public IAttributeRepository AttributeRepository { get; set; }
        [Dependency]
        public IAssetsService AssetsService { get; set; }
        [Dependency]
        public IUserService UserService { get; set; }
        [Dependency]
        public IAttributeFieldFactory AttributeFieldFactory { get; set; }
        [Dependency]
        public ILayoutRepository LayoutRepository { get; set; }
        [Dependency]
        public ILog Logger { get; set; }
        [Dependency]
        public IDynamicListsService DynamicListsService { get; set; }

        protected override void OnInit(System.EventArgs e)
        {
            base.OnInit(e);

            if (!Request.IsAuthenticated)
            {
                Response.Redirect("/Login.aspx?ReturnUrl=" 
                    + Server.UrlEncode(Request.Url.PathAndQuery));
                return;
            }

            if (!AccessManager.Instance.IsActual)
                 AccessManager.Instance.InitRights();
        }

        protected override void InitializeCulture()
        {
            if (Request.Url.PathAndQuery.StartsWith("/admin"))
            {
                var culture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
                System.Threading.Thread.CurrentThread.CurrentCulture = culture;
                System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
            }
            else
            {
                Helpers.Culture.InitCulture();    
            }
        }
    }
}