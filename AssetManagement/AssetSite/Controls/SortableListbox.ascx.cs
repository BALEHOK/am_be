using System;
using System.Web.UI.WebControls;

public partial class SortableListbox : System.Web.UI.UserControl
{
    /// <summary>
    /// Unique ID of related panel object
    /// </summary>
    public long PanelUID
    {
        get { return ViewState["PANEL_UID"] == null ? 0 : (long)ViewState["PANEL_UID"]; }
        set { ViewState["PANEL_UID"] = value; }
    }

    /// <summary>
    /// List of all assigned items
    /// </summary>
    public ListItemCollection ListItems
    {
        get { return this.listAttributes.Items; }
    }

    /// <summary>
    /// Gets or sets the name of the panel.
    /// </summary>
    /// <value>The name of the panel.</value>
    public string PanelName
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the CSS class.
    /// </summary>
    /// <value>The CSS class.</value>
    public string CssClass
    {
        get;
        set;
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        PanelTitle.Text = PanelName;
    }

    public string[] GetAttributesUidsList()
    {
        return hfSortAttributes.Value.Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries);
    }
}
