using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Permissions;

namespace AppFramework.Core.PL.Components
{
    #region Container Class

    // See FormElementContainer and FormElementContainerType Class

    #endregion

    #region EventArgs Class
    /*
    public class MyEventArgs : EventArgs
    { 
        // Variables
        private string prop1 = "";

        // Constructor
        public MyEventArgs(string prop1)
        {
            this.prop1 = prop1;
        }

        // Properties
        public string Prop1 { get { return this.prop1; } set { this.prop1 = value; } }
    }
*/
    #endregion

    #region Delegates
    /*
    public delegate void MyEventHandler (object sender, MyEventArgs e);
*/
    #endregion

    /// <summary>
    /// 
    /// </summary>
    [DefaultProperty("Text")]
    [ToolboxData(@"<{0}:DynPanel runat=""server"" HeaderCssStyle="""" ContentCssStyle=""""></{0}:DynPanel>")]
    [ParseChildren(true)]
    [Designer("WebControls.DialogDesigner, WebControls", typeof(IDesigner))]
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class WizPanAssetGeneral : DynFormPanel
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the component class.
        /// </summary>
        public WizPanAssetGeneral()
            : base()
        { }

        #endregion

        #region Private members

        // Variables to handle PostBack Event

        // Containers
        private FormElementContainer labelNameContainer;
        private FormElementContainer labelNameLang1Container;
        private FormElementContainer labelNameLang2Container;
        private FormElementContainer labelDescriptionContainer;

        private FormElementContainer textNameContainer;
        private FormElementContainer textNameLang1Container;
        private FormElementContainer textNameLang2Container;
        private FormElementContainer textDescriptionContainer;

        #endregion

        #region Enumerators
        #endregion

        #region Styles (DynPanelStyle - Definitions + Properties)

        #endregion

        #region Component Properties (except Templates)

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string ContentBlockCssClass
        {
            get
            {
                String s = (String)ViewState["ContentBlockCssClass"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["ContentBlockCssClass"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string LabelCssClass
        {
            get
            {
                String s = (String)ViewState["LabelCssClass"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["LabelCssClass"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string InputCssClass
        {
            get
            {
                String s = (String)ViewState["InputCssClass"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["InputCssClass"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string ButtonCssClass
        {
            get
            {
                String s = (String)ViewState["ButtonCssClass"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["ButtonCssClass"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string TextLabelName
        {
            get
            {
                String s = (String)ViewState["TextLabelName"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["TextLabelName"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string TextLabelNameLang1
        {
            get
            {
                String s = (String)ViewState["TextLabelNameLang1"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["TextLabelNameLang1"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string TextLabelNameLang2
        {
            get
            {
                String s = (String)ViewState["TextLabelNameLang2"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["TextLabelNameLang2"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string TextLabelDescription
        {
            get
            {
                String s = (String)ViewState["TextLabelDescription"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["TextLabelDescription"] = value;
            }
        }

        #endregion

        #region Templates

        #endregion

        #region Custom Events
        /*
        private static readonly object MyEventKey = new object();
        public event MyEventHandler MyEvent
        {
            add { Events.AddHandler(MyEventKey, value); }
            remove { Events.RemoveHandler(MyEventKey, value); }
        }

        protected virtual void OnMyEvent(MyEventArgs e)
        {
            MyEventHandler handler = Events[MyEventKey] as MyEventHandler;
            if (handler != null) { handler(this, e); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected override bool OnBubbleEvent(object source, EventArgs args)
        {
            bool handled = false;
            CommandEventArgs ce = args as CommandEventArgs;
            if (ce != null && ce.CommandName == "NameOfSubmitButton")
            {
                MyEventArgs e = new MyEventArgs(Prop1);
                OnMyEvent(e);
                handled = true;
            }
            return handled;
        }
*/
        #endregion

        #region Component Events
        /*
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"></see> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
            }

            base.OnLoad(e);
        }
*/
        #endregion

        #region Component Methods (overriden)

        #region Methods concerning Containers

        /// <summary>
        /// Create the container used by the Child Controls of the Custom Composite Control
        /// </summary>
        /// <returns></returns>
        protected virtual FormElementContainer CreateFormElementContainer(FormElementContainerType containerType)
        {
            return new FormElementContainer(containerType);
        }

        /// <summary>
        /// Add a container, with initiated child controls, to the Control Collection of the Custom (Composite) Control
        /// </summary>
        /// <param name="container"></param>
        protected virtual void AddFormElementContainer(FormElementContainer container)
        {
            Controls.Add(container);
        }

        #endregion

        #region Create Controls & Containers
        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation 
        /// to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            try
            {
                this.Controls.Clear();
                base.CreateChildControls();

                CreateControls();

                this.ChildControlsCreated = true;
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Create the controls inside the component (buttons, panels, ...
        /// </summary>
        private void CreateControls()
        {
            // Initialize label containers
            labelNameContainer = new FormElementContainer(FormElementContainerType.Label);
            labelNameLang1Container = new FormElementContainer(FormElementContainerType.Label);
            labelNameLang2Container = new FormElementContainer(FormElementContainerType.Label);
            labelDescriptionContainer = new FormElementContainer(FormElementContainerType.Label);

            // Create Label Controls
            Label labelName = new Label();
            labelName.Text = (!string.IsNullOrEmpty(this.TextLabelName)) ? 
                this.TextLabelName : HttpContext.GetGlobalResourceObject("ConfigAssetWizard", "LabelName").ToString();
            labelNameContainer.Controls.Add(labelName);
            Label labelLang1 = new Label();
            labelLang1.Text = (!string.IsNullOrEmpty(this.TextLabelNameLang1)) ?
                this.TextLabelNameLang1 : HttpContext.GetGlobalResourceObject("ConfigAssetWizard", "LabelNameLang1").ToString();
            labelNameLang1Container.Controls.Add(labelLang1);
            Label LabelLang2 = new Label();
            LabelLang2.Text = (!string.IsNullOrEmpty(this.TextLabelNameLang2)) ?
                this.TextLabelNameLang2 : HttpContext.GetGlobalResourceObject("ConfigAssetWizard", "LabelNameLang2").ToString();
            labelNameLang2Container.Controls.Add(LabelLang2);
            Label LabelDescription = new Label();
            LabelDescription.Text = (!string.IsNullOrEmpty(this.TextLabelDescription)) ?
                this.TextLabelDescription : HttpContext.GetGlobalResourceObject("ConfigAssetWizard", "LabelDescription").ToString();
            labelDescriptionContainer.Controls.Add(LabelDescription);

            // Initialize textBox containers
            textNameContainer = new FormElementContainer(FormElementContainerType.TextBox);
            textNameLang1Container = new FormElementContainer(FormElementContainerType.TextBox);
            textNameLang2Container = new FormElementContainer(FormElementContainerType.TextBox);
            textDescriptionContainer = new FormElementContainer(FormElementContainerType.MultilineTextBox);

            // Create the input fiels
            TextBox textBoxName = new TextBox();
            textNameContainer.Controls.Add(textBoxName);
            TextBox textBoxLang1 = new TextBox();
            textNameLang1Container.Controls.Add(textBoxLang1);
            TextBox textBoxLang2 = new TextBox();
            textNameLang2Container.Controls.Add(textBoxLang2);
            TextBox textBoxDescription = new TextBox();
            textBoxDescription.TextMode = TextBoxMode.MultiLine;
            textBoxDescription.Height = Unit.Pixel(50);
            textDescriptionContainer.Controls.Add(textBoxDescription);
        }

        #endregion

        #region Render Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void RenderContents(HtmlTextWriter writer)
        {
            base.RenderContents(writer);
            EnsureChildControls();

            ApplyFormElementContainerStyles();

            Panel rowpanel;
            Panel contentPanel = new Panel();

            if (this.ContentBlockCssClass != null && this.ContentBlockCssClass is string)
            {
                contentPanel.CssClass = this.ContentBlockCssClass;
            }

            rowpanel = CreateRowPanel();
            rowpanel.Controls.Add(labelNameContainer);
            rowpanel.Controls.Add(textNameContainer);
            contentPanel.Controls.Add(rowpanel);

            rowpanel = CreateRowPanel();
            rowpanel.Controls.Add(labelNameLang1Container);
            rowpanel.Controls.Add(textNameLang1Container);
            contentPanel.Controls.Add(rowpanel);

            rowpanel = CreateRowPanel();
            rowpanel.Controls.Add(labelNameLang2Container);
            rowpanel.Controls.Add(textNameLang2Container);
            contentPanel.Controls.Add(rowpanel);

            rowpanel = CreateRowPanel();
            rowpanel.Controls.Add(labelDescriptionContainer);
            rowpanel.Controls.Add(textDescriptionContainer);
            contentPanel.Controls.Add(rowpanel);


            contentPanel.RenderControl(writer);
        }

        private Panel CreateRowPanel()
        {
            Panel pnl = new Panel();
            pnl.CssClass = "row";
            return pnl;
        }

        /// <summary>
        /// Apply all (custom) styles to the different containers
        /// </summary>
        protected virtual void ApplyFormElementContainerStyles()
        {
            if (this.LabelCssClass != null && this.LabelCssClass is string)
            {
                labelNameContainer.CssClass = this.LabelCssClass;
                labelNameLang1Container.CssClass = this.LabelCssClass;
                labelNameLang2Container.CssClass = this.LabelCssClass;
                labelDescriptionContainer.CssClass = this.LabelCssClass;
            }

            if (this.InputCssClass != null && this.InputCssClass is string)
            {
                textNameContainer.CssClass = this.InputCssClass;
                textNameLang1Container.CssClass = this.InputCssClass;
                textNameLang2Container.CssClass = this.InputCssClass;
                textDescriptionContainer.CssClass = this.InputCssClass;
            }
        }

        #endregion

        #endregion
    }
}
