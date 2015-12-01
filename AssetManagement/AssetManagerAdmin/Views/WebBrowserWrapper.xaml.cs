using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using mshtml;

namespace AssetManagerAdmin.View
{
    public class PageLoadedEventArgs : EventArgs
    {
        public HTMLDocument Document { get; private set; }
        public WebBrowser WebBrowserControl { get; private set; }

        public PageLoadedEventArgs(WebBrowser browser)
        {
            Document = (HTMLDocument)browser.Document;
            WebBrowserControl = browser;
        }
    }

    /// <summary>
    /// Interaction logic for WebBrowserWrapper.xaml
    /// </summary>
    public partial class WebBrowserWrapper : UserControl
    {
        public event EventHandler<EventArgs> OnError;
        public event EventHandler<PageLoadedEventArgs> OnPageLoaded;

        public static readonly DependencyProperty BrowserSourceProperty = DependencyProperty.Register(
            "BrowserSource", typeof (string), typeof (WebBrowserWrapper), new PropertyMetadata(default(string)));

        public string BrowserSource
        {
            get { return (string) GetValue(BrowserSourceProperty); }
            set { SetValue(BrowserSourceProperty, value); }
        }

        public WebBrowserWrapper()
        {
            InitializeComponent();

            WebBrowserCtl.Navigating += (sender, args) => { WebBrowserCtl.Visibility = Visibility.Collapsed; };
            WebBrowserCtl.Navigated += (sender, args) =>
            {                
                Thread.Sleep(50);
                WebBrowserCtl.Visibility = Visibility.Visible;
            };
            WebBrowserCtl.LoadCompleted += (sender, args) =>
            {
                CutPage();
                if (OnPageLoaded != null)
                {
                    var navigatedArgs = new PageLoadedEventArgs(WebBrowserCtl);
                    OnPageLoaded(this, navigatedArgs);
                }                
            };
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == BrowserSourceProperty && !string.IsNullOrEmpty(BrowserSource))
            {                
                WebBrowserCtl.Source = new Uri(BrowserSource);
            }
        }

        private void CutPage()
        {            
            var script = GetDeleteScript(_elementsToDelete);
            try
            {
                WebBrowserCtl.InvokeScript("eval", script);
            }
            catch (Exception e)
            {
                if (OnError != null)
                    OnError(this, new EventArgs());
            }
            finally
            {
                WebBrowserCtl.Visibility = Visibility.Visible;                
            }
        }

        private readonly string[] _elementsToDelete =
        {
            "#masthead",
            "#menubar",
            "#breadcrumb",
            "$('.login-area-block').last()",
            "#footer",
//            ".panel.configusers",
//            "$('a[href$=\"Batch/Default.aspx\"]').parent()",
//            "$('a[href$=\"Import/Default.aspx\"]').parent()",
//            "$('a[href$=\"Export/Default.aspx\"]').parent()",
//            "$('a[href$=\"LocationMove.aspx\"]').parent()",
//            "$('a[href$=\"FAQItems.aspx\"]').parent()",
//            "$('a[href$=\"ZipsAndPlaces.aspx\"]').parent()",
//            "$('a[href$=\"ServiceOps.aspx\"]').parent()",
//            "$('a[href$=\"../Mobile/Default.aspx\"]').parent()",
//            "$('a[href$=\"Reports/Default.aspx\"]').parent()",
        };
        
        private string GetDeleteScript(IEnumerable<string> elements)
        {
            var script = new StringBuilder();
            elements.ToList().ForEach(e =>
            {
                var line = e.StartsWith("$")
                    ? string.Format("{0}.remove();", e)
                    : string.Format("$('{0}').remove();", e);
                script.Append(line);
            });
            return script.ToString();
        }
    }
}