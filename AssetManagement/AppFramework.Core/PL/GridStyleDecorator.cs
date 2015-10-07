using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Drawing;

namespace AppFramework.Core.PL
{
    /// <summary>
    /// Implements the decorator for GridView element.
    /// Applies a displayed style to this element.
    /// </summary>
    public class GridStyleDecorator : GridView
    {
        public GridStyleDecorator(GridView decoratedGrid)
        {              
            #region Setting the UI
            CssClass = "w100p";
            AutoGenerateColumns = false;
            CellPadding = 4;
            ForeColor = ColorTranslator.FromHtml("#333333");
            GridLines = GridLines.None;
            AllowPaging = false;
            PageSize = 1000;
            RowStyle.BackColor = ColorTranslator.FromHtml("#ffffff");
            FooterStyle.BackColor = ColorTranslator.FromHtml("#507CD1");
            FooterStyle.Font.Bold = true;
            FooterStyle.ForeColor = ColorTranslator.FromHtml("#ffffff");
            PagerStyle.BackColor = ColorTranslator.FromHtml("#2461BF");
            PagerStyle.ForeColor = ColorTranslator.FromHtml("#ffffff");
            PagerStyle.HorizontalAlign = HorizontalAlign.Center;
            SelectedRowStyle.BackColor = ColorTranslator.FromHtml("#D1DDF1");
            SelectedRowStyle.Font.Bold = true;
            SelectedRowStyle.ForeColor = ColorTranslator.FromHtml("#333333");
            HeaderStyle.BackColor = ColorTranslator.FromHtml("#DFDFDF");
            HeaderStyle.Font.Bold = true;
            HeaderStyle.ForeColor = ColorTranslator.FromHtml("#1E1E1E");
            HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
            AlternatingRowStyle.BackColor = ColorTranslator.FromHtml("#E0FFC1");
            EmptyTemplate tmpl = new EmptyTemplate();
            tmpl.Controls.Add(new Label() { Text = "list is empty", CssClass = "italic" });
            EmptyDataTemplate = tmpl;
            #endregion         
        }
        
    }
}
