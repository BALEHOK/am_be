using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppFramework.Core.Classes;
using AppFramework.Core.Classes.DynLists;
using AppFramework.Core.ConstantsEnumerators;
using Microsoft.Practices.Unity;
using AppFramework.ConstantsEnumerators;

namespace AssetSite.Controls
{
    public delegate void SelectionChangedDelegate(object sender, EventArgs e);

    /// <summary>
    /// Temporary control for storing info about source Dynamic list Id and index in stored states collection
    /// </summary>
    internal class DLDropDownList : DropDownList
    {
        public long DynamicListUid
        {
            get;
            set;
        }

        public int StateIndex
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Control builds collection of DropDownLists depend of selecting values of Dynamic List id
    /// </summary>
    public partial class DynListDropDown : UserControl
    {
        public IDynamicListsService DynamicListsService
        {
            get
            {
                if (_service == null)
                {
                    // hack: get service via reflection and convention
                    var serviceProp = Page.GetType().GetProperty("DynamicListsService");
                    if (serviceProp == null)
                        throw new NullReferenceException(
                            "Page must have DynamicListsService property of type IDynamicListsService");
                    _service = serviceProp.GetValue(Page) as IDynamicListsService;
                }
                return _service;
            }
        }

        public event SelectionChangedDelegate SelectionChanged;
        private IDynamicListsService _service;

        public long ListUID { get; set; }
        public string ItemName { get; set; }
        public long SelectedValue { get; set; }
        public string SelectedText { get; private set; }
        public int StateIndex { get; set; }

        public bool Editable { get; set; }

        public AssetAttribute AssetAttribute
        {
            get;
            set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the DropDown control. 
        /// For selected item info add to stored states and new control creates.
        /// In this case not needed to use CreateControl, because event handles ONLY in edit mode
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void dd_SelectedIndexChanged(object sender, EventArgs e)
        {
            DLDropDownList d = sender as DLDropDownList;
            if (d != null)
            {
                long uid;
                if (long.TryParse(d.SelectedValue, out uid))
                {
                    this.SelectedValue = uid;
                    this.SelectedText = d.SelectedItem.Text;
                }
                else
                {
                    this.SelectedValue = default(long);
                }

                if (SelectionChanged != null)
                    SelectionChanged((object)this, new EventArgs());
            }
        }

        private List<DynamicList> _getAllDynamicLists()
        {
            var list = Cache.Get("DynLists") as List<DynamicList>;
            if (list == null)
            {
                list = DynamicListsService.GetAll();
                // Add cache for one minute
                Cache.Add("DynLists",
                            list,
                            null,
                            DateTime.Now.AddMinutes(1),
                            System.Web.Caching.Cache.NoSlidingExpiration,
                            System.Web.Caching.CacheItemPriority.Low,
                            null);
            }
            return list;
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            RepeaterItem itm = this.NamingContainer as RepeaterItem;
            SavedState currentState = itm.DataItem as SavedState;
            var list = DynamicListsService.GetByUid(currentState.ListId);
            if (list == null)
                throw new NullReferenceException("DynamicList");

            DynListControls.Controls.Clear();
            Control dd = CreateControl(currentState.Index, list, currentState.Value, this.Editable);
            if (dd != null)
            {
                DynListControls.Controls.Add(dd);
                if (Editable && HttpContext.Current.User.IsInRole(PredefinedRoles.Administrators.ToString()))
                {
                    EditDialog.DynamicList = list;
                    EditDialog.DynamicLists = _getAllDynamicLists();
                    EditDialog.Visible = true;
                }
            }
        }

        /// <summary>
        /// Creates the control for display and / or editing value
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="listUid">The list uid.</param>
        /// <param name="itemId">The item id.</param>
        /// <param name="editable">if set to <c>true</c> dropdown will be created, and literal othercase.</param>
        /// <returns></returns>
        private Control CreateControl(int index, DynamicList list, string value, bool editable)
        {
            Control dd = null;
            if (editable)
            {
                if (list != null)
                {
                    dd = new DLDropDownList()
                    {
                        ID = "DLDropDownList_" + list.UID.ToString(),
                        DataSource = list.Items,
                        DataTextField = "Value",
                        DataValueField = "UID",
                        AutoPostBack = true,
                        DynamicListUid = list.UID,
                        StateIndex = index,
                        AppendDataBoundItems = true
                    };

                    try
                    {
                        (dd as DropDownList).Width = Unit.Parse(ApplicationSettings.ControlsWidth);
                    }
                    catch { }

                    (dd as DLDropDownList).Items.Add(AppFramework.Core.Properties.Resources.SelectText);
                    dd.DataBind();
                    (dd as DLDropDownList).DataSource = null;

                    ListItem itm = (dd as DropDownList).Items.FindByText(value);
                    if (itm != null)
                    {
                        (dd as DLDropDownList).SelectedValue = itm.Value;
                    }

                    (dd as DLDropDownList).SelectedIndexChanged += new EventHandler(dd_SelectedIndexChanged);
                }
            }
            else
            {
                dd = new Literal();
                if (!string.IsNullOrEmpty(value))
                    (dd as Literal).Text = value;
            }

            return dd;
        }

        #region Saving control state
        protected override void OnInit(EventArgs e)
        {
            Page.RegisterRequiresControlState(this);    // register this control as control needed to save its state
            base.OnInit(e);
        }

        protected override object SaveControlState()
        {
            return this.Editable;
        }

        protected override void LoadControlState(object savedState)
        {
            this.Editable = (bool)savedState;
        }
        #endregion
    }
}