using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class ListboxForAttributeAdding : System.Web.UI.UserControl
{
    /// <summary>
    /// Unique ID of related panel object
    /// </summary>
    public long PanelUID
    {
        get { return this._panelUID; }
        set { this._panelUID = value; }
    }

    /// <summary>
    /// List of all assigned items
    /// </summary>
    public ListItemCollection ListItems
    {
        get { return this.listboxControl.Items; }
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

    public delegate void LeftButtonHandler(ListItemCollection item);
    public event LeftButtonHandler PushLeft;

    public delegate ListItemCollection RightButtonHandler();
    public event RightButtonHandler PushRight;

    private long _panelUID;    

    protected void Page_Load(object sender, EventArgs e)
    {
        PanelTitle.Text = Server.HtmlEncode(PanelName);
    }

    protected void btnRight_Click(object sender, EventArgs e)
    {
        ListItemCollection items = PushRight();
        foreach (ListItem item in items)
        {
            listboxControl.Items.Add(item);
        }             
    }

    protected void btnLeft_Click(object sender, EventArgs e)
    {
        int ItemsCount = listboxControl.Items.Count;
        int i = 0;
        ListItemCollection movingItems = new ListItemCollection();

        while (i < listboxControl.Items.Count && listboxControl.Items.Count != 0)
        {
            if (listboxControl.Items[i].Selected)
            {
                movingItems.Add(listboxControl.Items[i]);
                listboxControl.Items.Remove(listboxControl.Items[i]);
            }
            else
            {
                i++;
            }
        }
        PushLeft(movingItems);        
    }

}
