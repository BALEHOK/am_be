using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace AssetSite.Controls
{
    public partial class WizardMenu : System.Web.UI.UserControl
    {
        private int currentStepIndex;

        public int CurrentStepIndex {
            get
            {
                return this.currentStepIndex;
            }
            set
            {
                // 4 and 8 - edge steps for big Schermweergave step, at this steps actually active 4 step, but different substeps. 1 substerp for 4 step, 2 - for 5 step, (n-3) for n step
                if (value > 4 && value <= 8)
                {
                    CurrentSubIndex = value - 3;
                    currentStepIndex = 4;
                }
                else if (value == 4)
                {
                    CurrentSubIndex = 1;
                    currentStepIndex = 3;
                }
                else
                {
                    // this also needed - because substeps now made as steps in wizard, so we just shift back for 4 step - which is substeps of Schermweergave
                    currentStepIndex = value > 8 ? value - 4 : value;
                }
            }
        }
        public int CurrentSubIndex { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {     

            foreach (Control item in itemsContainer.Controls)
            {
                if (item.GetType() == typeof(Panel))
                {                    
                    int itemIndex = 0;
                    Regex re = new Regex("\\d+");
                    Match m = re.Match(item.ID);
                    if (m.Success)
                    {
                        int.TryParse(m.Value, out itemIndex);
                    }
                    if (itemIndex > 0)
                    {
                        Panel title = item.FindControl(string.Format("title_{0}", itemIndex)) as Panel;
                        Panel description = item.FindControl(string.Format("desc_{0}", itemIndex)) as Panel;

                        if (title != null && description != null)
                        {
                            title.CssClass = itemIndex == CurrentStepIndex ? "active" : "item";
                            description.Visible = itemIndex == CurrentStepIndex;

                            if (CurrentSubIndex > 0)
                            {
                                LinkButton lnk = item.FindControl(string.Format("substep_{0}_{1}", itemIndex, CurrentSubIndex)) as LinkButton;
                                if (lnk != null)
                                {
                                    lnk.CssClass = "active_lnk";
                                }
                            }
                        }
                    }                    
                }
            }            
        }        
    }
}