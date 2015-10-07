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
    using InventScanner.Core;
    using AppFramework.ConstantsEnumerators;
    using System.Threading;

    public partial class LocationImport : Form
    {
        private XmlAdapters xmlAdapters;
        private List<Asset> selectedLocationsList;
        private LocationImporterCtrl ctrl;
        private LocationExportCtrl exporter;

        public LocationImport()
        {
            InitializeComponent();
            xmlAdapters = new XmlAdapters();
            selectedLocationsList = new List<Asset>();

            ctrl = new LocationImporterCtrl();
            ctrl.Name = "LocationsLoader1";
            ctrl.Location = new Point(6, 176);
            ctrl.OnLocationsLoaded += new LocationImporterCtrl.LoadingCompliteDelegate(ctrl_OnLocationsLoaded);

            exporter = new LocationExportCtrl();
            exporter.Name = "LocationsExporter1";
            exporter.Location = new Point(6, 176);
            exporter.OnExportComplite += new LocationExportCtrl.ExportCompliteDelegate(exporter_OnExportComplite);
        }

        internal XmlAdapters XmlAdapters
        {
            get { return this.xmlAdapters; }
        }

        private void LocationImport_Load(object sender, EventArgs e)
        {
        }

        void exporter_OnExportComplite(object sender, EventArgs e)
        {
            xmlAdapters = exporter.Adapters;

            tableLayoutPanel1.Controls.Remove(exporter);
            Cursor.Current = Cursors.Default;
            button3.Enabled = selectedListBox.Visible = true;

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        void ctrl_OnLocationsLoaded(object sender, EventArgs e)
        {
            existingListBox.BeginUpdate();

            IEnumerable<Asset> result = ctrl.GetLications();
            foreach (Asset one in result)
            {
                existingListBox.Items.Add(one);
            }
            //this.bindingSource1.DataSource = ctrl.GetLications();

            tableLayoutPanel1.Controls.Remove(ctrl);
            existingListBox.Visible = true;
            existingListBox.EndUpdate();
        }

        /// <summary>
        /// Gets the selected location.
        /// </summary>
        /// <returns>Return <see cref="AssetType"/> value</returns>
        private IEnumerable<Asset> GetSelectedLocation()
        {
            return this.selectedListBox.Items.Cast<Asset>();
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the locationComboBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void locationComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        #region Item moving
        private void button5_Click(object sender, EventArgs e)
        {
            this.moveAllItems(ref selectedListBox, ref existingListBox);
        }

        private void moveSelToRight_Click(object sender, EventArgs e)
        {
            this.moveSelectedItems(ref existingListBox, ref selectedListBox);
        }

        private void moveAllToRight_Click(object sender, EventArgs e)
        {
            this.moveAllItems(ref existingListBox, ref selectedListBox);
        }

        private void moveSelToLeft_Click(object sender, EventArgs e)
        {
            this.moveSelectedItems(ref selectedListBox, ref existingListBox);
        }

        private void moveSelectedItems(ref ListBox fromSource, ref ListBox toSource)
        {
            object[] sel = new object[fromSource.SelectedItems.Count];
            fromSource.SelectedItems.CopyTo(sel, 0);
            foreach (var item in sel)
            {
                toSource.Items.Add(item);
                fromSource.Items.Remove(item);
            }
        }

        private void moveAllItems(ref ListBox fromSource, ref ListBox toSource)
        {
            foreach (var item in fromSource.Items)
            {
                toSource.Items.Add(item);
            }
            fromSource.Items.Clear();
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void LocationImport_Shown(object sender, EventArgs e)
        {
            existingListBox.Visible = false;
            tableLayoutPanel1.Controls.Add(ctrl, 0, 1);
        }

        private void LocationImport_FormClosing(object sender, FormClosingEventArgs e)
        {
            ctrl.OnLocationsLoaded -= ctrl_OnLocationsLoaded;
            ctrl.Dispose();

            exporter.OnExportComplite -= exporter_OnExportComplite;
            exporter.Dispose();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            button3.Enabled = selectedListBox.Visible = false;
            exporter.Locations = this.GetSelectedLocation();
            tableLayoutPanel1.Controls.Add(exporter, 2, 1);
        }
    }
}