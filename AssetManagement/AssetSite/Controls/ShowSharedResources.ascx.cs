using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

using AppFramework.Core.Classes;
using AssetSite.Controls;

namespace AssetSite.Controls
{
    public partial class ShowSharedResources : System.Web.UI.UserControl
    {
        public string FileName
        {
            get
            {
                return PathResource.Text;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void Browse()
        {
            ShareCatalogTreeView.Nodes.Clear();

            ManagementResources ob = new ManagementResources();
            List<string> listDirectories = new List<string>();
            listDirectories = ob.GetSharedDirectories();
            foreach (string directoryName in listDirectories.Where(d => !string.IsNullOrEmpty(d)))
            {
                TreeNode node = new TreeNode(directoryName);
                node.Expanded = false;
                node.ImageUrl = "~/images/uponelevel.gif";

                string[] files = Directory.GetFiles(directoryName);

                foreach (string file in files)
                {
                    TreeNode nod = new TreeNode();
                    nod.Text = file.Remove(0, file.LastIndexOf(@"\") + 1);

                    nod.ImageUrl = "~/images/file.gif";
                    node.ChildNodes.Add(nod);
                }

                ShareCatalogTreeView.Nodes.Add(node);
                FillTreeview(node, directoryName);
            }
        }


        public void FillTreeview(TreeNode node, string dirpath)
        {
            string[] directories = Directory.GetDirectories(dirpath);
            foreach (string directory in directories)
            {
                string[] files = Directory.GetFiles(directory);

                TreeNode tndirectorynode = new TreeNode();
                tndirectorynode.ImageUrl = "~/images/uponelevel.gif";
                foreach (string file in files)
                {
                    TreeNode nod = new TreeNode();

                    nod.Text = file.Remove(0, file.LastIndexOf(@"\") + 1);
                    nod.ImageUrl = "~/images/file.gif";
                    tndirectorynode.ChildNodes.Add(nod);
                }

                tndirectorynode.Text = directory.Remove(0, directory.LastIndexOf(@"\") + 1);
                node.ChildNodes.Add(tndirectorynode);
                tndirectorynode.Expanded = false;
                FillTreeview(tndirectorynode, directory);
            }
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            ShareCatalogTreeView.Nodes.Clear();
            PathResource.Text = "";
            Browse();
        }

        protected void ShareCatalogTreeView_SelectedNodeChanged(object sender, EventArgs e)
        {
            TreeNode node = ShareCatalogTreeView.SelectedNode;

            string path = node.ValuePath.Replace(@"/", @"\");
            if (File.Exists(path))
            {
                PathResource.Text = path;
            }

            node.Expand();

            while (node.Parent != null)
            {
                node = node.Parent;
                node.Expand();
            }
        }
    }
}