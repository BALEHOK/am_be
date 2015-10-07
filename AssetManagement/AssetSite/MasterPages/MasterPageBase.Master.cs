using AppFramework.ConstantsEnumerators;
using AppFramework.Core.AC.Authentication;
using AppFramework.Core.Classes;
using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Microsoft.Practices.Unity;
using System.Web.Mvc;

namespace AssetSite.MasterPages
{
    public partial class MasterPageBase : System.Web.UI.MasterPage
    {
        [Dependency]
        public IAuthenticationService AuthenticationService { get; set; }

        protected string UserRoles
        {
            get { return string.Join(", ", AuthenticationService.CurrentUser.Roles); }
        }

        public string BodyClass { get; set; }
        public string BodyOnLoadScript { get; set; }
        public bool DisplayRightColumn { get; set; }
        public bool DisplayLeftColumn { get; set; }
        public string BreadcrumbLabel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInit(EventArgs e)
        {
            // Initialize MasterPageBase properties
            this.BodyClass = "";
            this.BodyOnLoadScript = "";
            this.DisplayLeftColumn = true;
            this.DisplayRightColumn = false;

            HtmlLink cssLayout = new HtmlLink();
            cssLayout.Attributes.Add("rel", "stylesheet");
            cssLayout.Attributes.Add("type", "text/css");

            HtmlLink cssTheme = new HtmlLink();
            cssTheme.Attributes.Add("rel", "stylesheet");
            cssTheme.Attributes.Add("type", "text/css");

            //if (this.Request.Browser.ScreenPixelsWidth >= 1200){ // wide display
            cssLayout.Href = this.ResolveUrl("~/css/layout_1200.css");
            cssTheme.Href = this.ResolveUrl("~/css/theme_1200.css");
            //}else{ // square display
            //    cssLayout.Href = this.ResolveUrl("~/css/layout_950.css");
            //    cssTheme.Href = this.ResolveUrl("~/css/layout_950.css");}

            headerContent.Controls.AddAt(2, cssLayout);
            headerContent.Controls.AddAt(3, cssTheme);

            SetChromeFix();

            logoffblock.Visible = Request.IsAuthenticated;

            base.OnInit(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            string style = string.Format("<style type=\"text/css\">{0}</style>", PageCSSDefinitions());
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "Styles", style);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Page.GetType().Name == "login_aspx")
            {
                addUrl.Visible = false;
            }

            if (Request["AstType"] != null)
            {
                addUrl.HRef = "/Asset/New/Step2.aspx?atid=" + Request["AstType"].ToString();
            }

            if (ApplicationSettings.ApplicationType == ApplicationType.AssetManager)
            {
                lblBuild.Text = "AssetManager Build v. ";
            }
            else if (ApplicationSettings.ApplicationType == ApplicationType.Combined)
            {
                lblBuild.Text = "Combined Build v. ";
            }
            else
            {
                lblBuild.Text = "SOB en BUB Build v. ";
            }
            lblBuild.Text += HtmlHelperExtensions.GetAppBuildVersion();
        }

        /// <summary>
        /// Creates extra CSS Style definitions to control displaying the left and right column
        /// The MasterPageBase properties DisplayRightColumn and DisplayLeftColumn are used to control the view.
        /// </summary>
        /// <returns>string with CSS Class definitions</returns>
        protected string PageCSSDefinitions()
        {
            return PageCSSDefinitions(this.DisplayRightColumn, this.DisplayLeftColumn);
        }

        /// <summary>
        /// Creates extra CSS Style definitions to control displaying the left and right column.
        /// </summary>
        /// <param name="DisplayRightColumn"></param>
        /// <param name="DisplayLeftColumn"></param>
        /// <returns>string with CSS Class definitions</returns>
        protected string PageCSSDefinitions(bool DisplayRightColumn, bool DisplayLeftColumn)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder("");
            if (!DisplayRightColumn)
            {
                sb.AppendLine("#outer-column-container, #inner-column-container { border-right-width: 0; }");
                sb.AppendLine("#right-column {	display: none; }");
            }

            if (!DisplayLeftColumn)
            {
                sb.AppendLine("#outer-column-container, #inner-column-container { border-left-width: 0; }");
                sb.AppendLine("#left-column {	display: none; }");
            }

            return sb.ToString();
        }


        private void SetChromeFix()
        {
            string browser = Context.Request.UserAgent;

            if (!String.IsNullOrEmpty(browser) &&
                 browser.IndexOf("Chrome") > -1 &&
                !Page.ClientScript.IsStartupScriptRegistered("GoogleChromeValidatorHookupEventFix"))
            {

                //// Fix for Google Chrome

                //// weblogs.asp.net/.../bug-with-latest-google-chrome-and-asp-net-validation.aspx

                Page.ClientScript.RegisterStartupScript(typeof(Page), "GoogleChromeValidatorHookupEventFix", @"function redefineValidatorHookupEvent() {

                   if (typeof (ValidatorHookupEvent) == ""function"") {

                       ValidatorHookupEvent = function(control, eventType, functionPrefix) {

                           var ev;

                           eval(""ev = control."" + eventType + "";"");

                           if (typeof(ev) == ""function"") {

                               ev = ev.toString();

                               ev = ev.substring(ev.indexOf(""{"") + 1, ev.lastIndexOf(""}""));

                           }

                           else {

                               ev = """";

                           }

                           var func = new Function(""event"", "" var evt = event; "" + functionPrefix + "" "" + ev);

                           eval(""control."" + eventType + "" = func;"");

                       }

                   }

                }

                redefineValidatorHookupEvent();

                ", true);

            }
        }

        protected void Logout_Click(object sender, EventArgs e)
        {
            Response.Redirect(FormsAuthentication.DefaultUrl);
        }
    }
}
