namespace InventScanner
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using AppFramework.Core.Classes;
    
    public partial class MoveAssets : Form
    {
        public MoveAssets()
        {
            InitializeComponent();
        }

        private void MoveAssets_Load(object sender, EventArgs e)
        {
            this.comboBox1.DataSource = AssetFactory.GetAllOnlyIdName(PredefinedAttribute.Get(AppFramework.ConstantsEnumerators.PredefinedEntity.Location).DynEntityConfigID).ToList();
        }

        public long SelectedLocationID
        {
            get
            {
                return (long)this.comboBox1.SelectedValue;
            }
        }
    }
}
