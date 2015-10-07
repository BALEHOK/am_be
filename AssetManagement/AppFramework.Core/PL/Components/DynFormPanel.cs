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

    public enum FormContainerType
    {
        HeaderContent = 1,
    }

    /// <summary>
    /// 
    /// </summary>
    public class DynFormPanelContainer : Panel, INamingContainer
    {
        private FormContainerType containerType;

        /// <summary>
        /// Container Constructor
        /// </summary>
        public DynFormPanelContainer(FormContainerType containerType)
        {
            this.containerType = containerType;
        }

        public FormContainerType ContainerType
        {
            get { return containerType; }
        }
    }

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

    [DefaultProperty("Text")]
    [ToolboxData(@"<{0}:DynPanel runat=""server"" HeaderCssStyle=""""></{0}:DynPanel>")]
    [ParseChildren(true)]
    [Designer("WebControls.DialogDesigner, WebControls", typeof(IDesigner))]
    [AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal)]
    public class DynFormPanel : CompositeControl 
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the component class.
        /// </summary>
        public DynFormPanel()
            : base()
        { }

        #endregion

        #region Private members

        // Variables to handle PostBack Event

        // Containers
        private DynFormPanelContainer headerContainer;

        #endregion

        #region Enumerators
        #endregion

        #region Styles (DynPanelStyle - Definitions + Properties)

        protected override Style CreateControlStyle()
        {
            return new DynPanelStyle(ViewState);
        }

        public virtual string BackImageUrl 
        { 
            get { return ((DynPanelStyle)ControlStyle).BackImageUrl;}
            set { ((DynPanelStyle)ControlStyle).BackImageUrl = value; }
        }

        public virtual ContentDirection Direction
        {
            get { return ((DynPanelStyle)ControlStyle).Direction; }
            set { ((DynPanelStyle)ControlStyle).Direction = value; }
        }

        public virtual HorizontalAlign HorizontalAlign
        {
            get { return ((DynPanelStyle)ControlStyle).HorizontalAlign; }
            set { ((DynPanelStyle)ControlStyle).HorizontalAlign = value; }
        }

        public virtual ScrollBars ScrollBars
        {
            get { return ((DynPanelStyle)ControlStyle).ScrollBars; }
            set { ((DynPanelStyle)ControlStyle).ScrollBars = value; }
        }

        public virtual bool Wrap
        {
            get { return ((DynPanelStyle)ControlStyle).Wrap; }
            set { ((DynPanelStyle)ControlStyle).Wrap = value; }
        }

        public virtual Float Float
        {
            get { return ((DynPanelStyle)ControlStyle).Float; }
            set { ((DynPanelStyle)ControlStyle).Float = value;}
        }

        public virtual Clear Clear
        {
            get { return ((DynPanelStyle)ControlStyle).Clear; }
            set { ((DynPanelStyle)ControlStyle).Clear = value; }
        }

        public virtual string HeaderCssStyle
        {
            get { return ((DynPanelStyle)ControlStyle).HeaderCssStyle; }
            set { ((DynPanelStyle)ControlStyle).HeaderCssStyle = value; }
        }

        #endregion

        #region Component Properties (except Templates)
/*
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Prop1
        {
            get
            {
                String s = (String)ViewState["Prop1"];
                return ((s == null) ? String.Empty : s);
            }

            set
            {
                ViewState["Prop1"] = value;
            }
        }
*/
        #endregion

        #region Templates

        private ITemplate headerTemplate;
        [TemplateContainer(typeof(DynPanelContentContainer))]
        public ITemplate HeaderTemplate
        {
            get { return headerTemplate;}
            set { headerTemplate = value;}
        }

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
        protected virtual DynFormPanelContainer CreateContainer(FormContainerType containerType)
        {
            return new DynFormPanelContainer(containerType);
        }

        /// <summary>
        /// Add a container, with initiated child controls, to the Control Collection of the Custom (Composite) Control
        /// </summary>
        /// <param name="container"></param>
        protected virtual void AddContainer(DynFormPanelContainer container)
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
            headerContainer = CreateContainer(FormContainerType.HeaderContent);
            CreateContainerChildControls(headerContainer);
            AddContainer(headerContainer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        protected virtual void CreateContainerChildControls(DynFormPanelContainer container)
        {
            switch (container.ContainerType)
            {
                case FormContainerType.HeaderContent:
                    if (headerTemplate != null)
                    {
                        headerTemplate.InstantiateIn(container);
                    }
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Render Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        protected override void RenderContents(HtmlTextWriter writer)
        {
            ApplyContainerStyles();
            RenderContainer(headerContainer, writer);
        }

        /// <summary>
        /// Apply all (custom) styles to the different containers
        /// </summary>
        protected virtual void ApplyContainerStyles()
        {
            ApplyContainerStyle(headerContainer);
        }

        private void ApplyContainerStyle(DynFormPanelContainer container)
        {
            switch (container.ContainerType)
            {
                case FormContainerType.HeaderContent:
                    container.CssClass = this.HeaderCssStyle;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Render the container
        /// </summary>
        /// <param name="container"></param>
        /// <param name="writer"></param>
        protected virtual void RenderContainer(DynFormPanelContainer container, HtmlTextWriter writer)
        {
            container.RenderControl(writer);
        }

        #endregion

        /// <summary>
        /// This method is present just to be able to force correct rendering during design-time
        /// </summary>
        internal void GetDesignTimeHtml()
        {
            this.EnsureChildControls();
        }

        /// <summary>
        /// This method is overriden to ensure that the outer control is rendered as a DIV-tag.
        /// </summary>
        protected override HtmlTextWriterTag TagKey
        {
            get { return HtmlTextWriterTag.Div; }
        }

        #endregion
    }
}
