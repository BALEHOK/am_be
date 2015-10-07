using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace AssetSite.Controls
{
    public partial class AssetTypeAttribList : System.Web.UI.UserControl
    {
        public long ReferencingDynEntityAttribConfigId { get; set; }

        public string ReferencingDynEntityAttribConfigName { get; set; }

        public string Name { get; set; }

        public AppFramework.Entities.DynEntityConfig DynEntityConfig { get; set; }

        public Dictionary<long, List<long>> LinkedAttributesUids { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblAT.Text = Name;
                lstAttributes.Items.Clear();

                foreach (var attribute in DynEntityConfig.DynEntityAttribConfigs.Where(a => a.Active && a.IsShownOnPanel))
                {
                    // attribute already assigned
                    if (LinkedAttributesUids.ContainsKey(ReferencingDynEntityAttribConfigId)
                            && LinkedAttributesUids[ReferencingDynEntityAttribConfigId].Contains(attribute.DynEntityAttribConfigUid))
                        continue;

                    ListItem li = new ListItem();
                    li.Text = string.Format("{0} ({1})", attribute.Name, ReferencingDynEntityAttribConfigName);
                    li.Value = string.Format("{0}:{1}:{2}",
                        attribute.DynEntityAttribConfigUid,
                        ReferencingDynEntityAttribConfigId,
                        attribute.IsRequired ? "*" : string.Empty);
                    lstAttributes.Items.Add(li);
                }
            }
        }

        public string GetDivID()
        {
            return "containerN" + ReferencingDynEntityAttribConfigId;
        }
    }
}