using System;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI.WebControls;
using AppFramework.DataProxy;
using Microsoft.Practices.Unity;

namespace AssetSite.Controls
{
    public partial class Translations : System.Web.UI.UserControl
    {
        [Dependency]
        public IUnitOfWork UnitOfWork {
            get { return _unitOfWork ?? (_unitOfWork = new UnitOfWork()); }
            set { _unitOfWork = value; } 
        }
        private IUnitOfWork _unitOfWork;

        /// <summary>
        /// DOM selector of control, which contains the key which have to be translated
        /// </summary>
        [Browsable(true)]
        public string ControlSelector;

        protected void Page_Load(object sender, EventArgs e)
        {
            var table = new Table {Width = new Unit("100%")};
            foreach (var language in UnitOfWork.LanguagesRepository.Get())
            {
                TableRow row = new TableRow();
                TableCell cell1 = new TableCell();
                TableCell cell2 = new TableCell();

                // TODO add default language in config
                if (language.CultureName.Contains("en-US"))
                {
                    cell1.Controls.Add(new Label() { Text = language.LongName + " (default): " });
                    cell2.Controls.Add(new TextBox() { Enabled = false, CssClass = "key", ClientIDMode = System.Web.UI.ClientIDMode.AutoID });
                }
                else
                {
                    cell1.Controls.Add(new Label() { Text = language.LongName + ": " });
                    cell2.Controls.Add(new TextBox() { ClientIDMode = System.Web.UI.ClientIDMode.AutoID });
                }
                cell2.Controls.Add(new HiddenField() { Value = language.CultureName });

                row.Cells.Add(cell1);
                row.Cells.Add(cell2);
                table.Rows.Add(row);
            }
            phTranslations.Controls.Add(table);
        }
    }
}