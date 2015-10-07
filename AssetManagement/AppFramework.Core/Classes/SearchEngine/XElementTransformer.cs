using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace AppFramework.Core.Classes.SearchEngine
{
    public static class XElementTransformer
    {
        public static TreeNode GenerateTreeView(this XElement toTransform)
        {
            TreeNode node = new TreeNode();
            node.ShowCheckBox = true;
            node.Text = toTransform.Attribute("name").Value;
            node.Value = toTransform.Attribute("id").Value;
            node.Checked = false;
            node.SelectAction = TreeNodeSelectAction.None;
            node.NavigateUrl = "javascript:void(0);";
            foreach (XElement element in toTransform.Nodes().OfType<XElement>())
            {
                node.ChildNodes.Add(element.GenerateTreeView());
            }
            return node;
        }
    }
}
