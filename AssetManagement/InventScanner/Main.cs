/*--------------------------------------------------------
* Main.cs
* 
* Copyright: 
* Author: aNesterov
* Created: 
* Purpose: 
* 
* Revisions:
* -------------------------------------------------------*/

namespace InventScanner
{
    using System;
    using System.Data;
    using System.Windows.Forms;
    using InventScanner.Core;
    using AppFramework.Core.Classes;
    using AppFramework.Core.Classes.Stock;
    using System.Drawing;
    using System.IO;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Collections.Generic;
    using AppFramework.ConstantsEnumerators;
    using System.Globalization;
    using System.Configuration;
    using System.Resources;
    using System.Threading;

    public partial class Main : Form
    {
        private XmlAdapter xmlAdapter;
        private XmlAdapters xmlAdapters;
        private string xmlPath;
        private bool asked = false;
        private Connect progress;
        private object sync = new object();
        private ResourceManager resourceManager;

        public Main()
        {
            InitThreadCulture();
            InitializeComponent();

            switch (Properties.Settings.Default.UICulture)
            {
                case "en":
                    englishToolStripMenuItem.Checked = true;
                    break;
                case "nl":
                    dutchToolStripMenuItem.Checked = true;
                    break;
                default:
                    break;
            }

            this.xmlPath = Application.StartupPath + "\\LastState.xml";
            this.progress = new Connect();
            this.resourceManager = new ResourceManager(typeof(InventScanner.Main));
        }

        private void InitThreadCulture()
        {
            var culture = CultureInfo.GetCultureInfo(Properties.Settings.Default.UICulture);
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
        }

        public bool ConnectToDatabase()
        {
            Connect connect = new Connect();
            connect.ShowDialog();
            return connect.Connected;
        }

        private void xmlAdapter_ReportListChanged()
        {
            this.reportRowBindingSource.ResetBindings(true);
        }

        private void scanButton_Click(object sender, EventArgs e)
        {
            if (this.xmlAdapter != null && !string.IsNullOrEmpty(barcodeTextBox.Text))
            {
                if (this.xmlAdapter.IsFinished && !this.xmlAdapter.IsSynchronized)
                {
                    if (MessageBox.Show(Properties.Resources.ContinueScanMessage, Properties.Resources.Question, MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        this.xmlAdapter.IsFinished = false;
                        this.xmlAdapter.ClearReportList();
                    }
                    else
                    {
                        return;
                    }
                }
                this.xmlAdapters.InventAsset(barcodeTextBox.Text, this.xmlAdapter);

                this.barcodeTextBox.Clear();
            }
        }

        private void importForLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool connected = this.ConnectToDatabase();
            if (connected)
            {
                using (var locationForm = new LocationImport())
                {
                    if (locationForm.ShowDialog() == DialogResult.OK && locationForm.XmlAdapters.Count > 0)
                    {
                        bool issame = this.xmlAdapters != null && this.xmlAdapters.Count == locationForm.XmlAdapters.Count &&
                            this.xmlAdapters.Except(locationForm.XmlAdapters, new XmlAdapterComparer()).Count() == 0;
                        if (this.xmlAdapters == null || (!issame && MessageBox.Show(Properties.Resources.ImportLocationMessage, Properties.Resources.Warning,
                            MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes))
                        {
                            this.xmlAdapters = locationForm.XmlAdapters;
                            this.xmlAdapter = locationForm.XmlAdapters[0];                        
                            this.UpdateBindings();
                            this.UpdateXmlAdapterHandlers();
                            this.EnableSync(true);
                        }
                    }
                }
            }
            else
            {
                ShowError();
            }
        }

        private void UpdateXmlAdapterHandlers()
        {
            this.xmlAdapters.ForEach(xmlAdapter =>
            {
                xmlAdapter.ReportListChanged += new XmlAdapter.ReportListChangedHandler(xmlAdapter_ReportListChanged);
                xmlAdapter.ScanStatusChanged += new XmlAdapter.ScanStatusChangedHandler(xmlAdapter_ScanStatusChanged);
            });
        }

        private void xmlAdapter_ScanStatusChanged(bool isFinished)
        {
            this.scanStatusLabel.Text = isFinished ? "Finished" : "In progress";
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.xmlAdapter != null &&
                MessageBox.Show(Properties.Resources.SaveStateMessage, Properties.Resources.Question, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Cursor.Current = Cursors.WaitCursor;
                this.xmlAdapters.SaveAsXml(this.xmlPath);
            }
        }

        private void Main_Load(object sender, EventArgs e)
        {
            #region Form init
            this.okPictureBox.Image = global::LogTable.Grid.ResourceLoader.GetResource("tick") as Image;
            this.warnPictureBox.Image = global::LogTable.Grid.ResourceLoader.GetResource("exclamation") as Image;
            this.errPictureBox.Image = global::LogTable.Grid.ResourceLoader.GetResource("cross") as Image;

            this.reportRowActionBindingSource.DataSource = ReportRowAction.GetActions();
            this.reportAllRowActionBindingSource.DataSource = ReportRowAction.GetActions();
            #endregion
        }

        private void LoadFromXml(string path)
        {
            this.xmlAdapters = XmlAdapters.LoadFromXml(path);
            if (this.xmlAdapters != null)
            {
                this.xmlAdapter = this.xmlAdapters[0];
                this.UpdateBindings();
                this.UpdateXmlAdapterHandlers();
                this.EnableSync(true);
            }
        }

        /// <summary>
        /// Updates the bindings for datasources on form
        /// </summary>
        private void UpdateBindings()
        {
            this.xmlAdapterBindingSource.DataSource = this.xmlAdapters;
            this.assetsBindingSource.DataSource = this.xmlAdapter.AssetsTable;
            this.logBindingSource.DataSource = this.xmlAdapter.LogTable;
            this.reportRowBindingSource.DataSource = this.xmlAdapter.ReportList;
        }

        private void assetsDataGridView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            #region Display asset view form
            long assetId = (long)(this.assetsBindingSource[e.RowIndex] as DataRowView)["AssetId"];
            long assetTypeId = (long)(this.assetsBindingSource[e.RowIndex] as DataRowView)["AssetTypeId"];
            AssetEx asset = this.xmlAdapter.GetAsset(assetId, assetTypeId);
            if (!asset.IsNullOrEmpty())
            {
                AssetView view = new AssetView();
                view.assetSource.DataSource = asset;
                view.assetName.Text = asset["Name"];
                view.ShowDialog();
            }
            #endregion
        }

        private void finishedButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Properties.Resources.StopScanMessage, Properties.Resources.Question, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.xmlAdapter.FinishScan();
                this.tabControl1.SelectedTab = this.tabPage3;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SyncWithDatabase();
        }

        private void SyncWithDatabase()
        {
            bool isDatabaseAvailable = this.ConnectToDatabase();

            if (isDatabaseAvailable)
            {
                if (!this.xmlAdapter.IsFinished)
                {
                    MessageBox.Show(Properties.Resources.ScanNotFinishedMessage, Properties.Resources.Information, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    this.xmlAdapter.SyncStart += new EventHandler<LengthlyOperationStartEventArgs>(xmlAdapter_PreviewStart);
                    this.xmlAdapter.SyncStep += new EventHandler<LengthlyOperationStepEventArgs>(xmlAdapter_PreviewStep);
                    this.EnableSync(false);
                    Action a = new Action(this.xmlAdapter.SyncWithDatabase);
                    a.BeginInvoke(new AsyncCallback(Sync_Callback), null);
                }
            }
            else
            {
                ShowError();
            }
        }

        private static void ShowError()
        {
            MessageBox.Show(Properties.Resources.DatabaseNotAvailable, Properties.Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Sync_Callback(IAsyncResult result)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.EnableSync(true);

                this.reportRowBindingSource.ResetBindings(false);
                MessageBox.Show(Properties.Resources.SyncFinishedMessage, Properties.Resources.Information, MessageBoxButtons.OK, MessageBoxIcon.Information);
            });
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.xmlAdapter != null)
                {
                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.AddExtension = true;
                        saveFileDialog.FileName = "Report_" + DateTime.Now.ToString("yyyymmdd") + ".xml";
                        saveFileDialog.Filter = "(XML)|*.xml";
                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            Cursor.Current = Cursors.WaitCursor;
                            this.xmlAdapters.SaveAsXml(saveFileDialog.FileName);
                            Cursor.Current = Cursors.Default;
                        }
                    }
                }
                else
                {
                    MessageBox.Show(Properties.Resources.NothingToSaveMessage, Properties.Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void synchronizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SyncWithDatabase();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.AddExtension = true;
                saveFileDialog.DefaultExt = ".xml";
                saveFileDialog.Filter = "XML|*.xml";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    this.xmlAdapters.SaveAsXml(saveFileDialog.FileName);
                    Cursor.Current = Cursors.Default;
                }

            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.AddExtension = true;
                openFileDialog.DefaultExt = ".xml";
                openFileDialog.Filter = "XML|*.xml";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    this.LoadFromXml(openFileDialog.FileName);
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.xmlAdapter = this.comboBox1.SelectedItem as XmlAdapter;
            if (this.xmlAdapter != null)
            {
                this.UpdateBindings();
                this.EnableSync(true);
            }
        }

        private void logBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.xmlAdapter != null)
            {
                foreach (DataGridViewRow row in this.logGridView.SelectedRows)
                {
                    ReportRow reportRow = row.DataBoundItem as ReportRow;
                    if (reportRow != null)
                    {
                        reportRow.Selected = true;
                        reportRow.Action = (ReportRowActionId)comboBox2.SelectedValue;
                    }
                }
                this.reportRowBindingSource.ResetBindings(false);
            }
        }

        private void moveAssetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.MoveAssets();
        }

        private void MoveAssets()
        {
            MoveAssets moveAssets = new MoveAssets();
            if (moveAssets.ShowDialog() == DialogResult.OK)
            {
                long locationId = moveAssets.SelectedLocationID;
                this.xmlAdapter.MoveAssets(locationId);
            }
        }

        private void Main_Activated(object sender, EventArgs e)
        {
            if (!asked)
            {
                asked = true;
                if (File.Exists(this.xmlPath))
                {
                    if (MessageBox.Show(Properties.Resources.LoadPreviousStateMessage, Properties.Resources.Question, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        this.LoadFromXml(this.xmlPath);
                        Cursor.Current = Cursors.Default;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool isDatabaseAvailable = this.ConnectToDatabase();

            if (isDatabaseAvailable)
            {
                this.xmlAdapter.PreviewStart += new EventHandler<LengthlyOperationStartEventArgs>(xmlAdapter_PreviewStart);
                this.xmlAdapter.PreviewStep += new EventHandler<LengthlyOperationStepEventArgs>(xmlAdapter_PreviewStep);

                this.EnableSync(false);
                Action a = new Action(this.xmlAdapter.Preview);
                a.BeginInvoke(new AsyncCallback(PreviewCallback), null);
            }
            else
            {
                ShowError();
            }
        }

        private void PreviewCallback(IAsyncResult result)
        {
            this.Invoke((MethodInvoker)delegate
            {
                var locations = AssetFactory.GetAllOnlyIdName(PredefinedAttribute.Get(PredefinedEntity.Location).DynEntityConfigID).ToList();
                this.LocationOut.DataSource = locations;
                this.LocationOut.DisplayMember = "Value";
                this.LocationOut.ValueMember = "Key";

                this.outLocationComboBox.DataSource = new List<KeyValuePair<long, string>>(locations);
                this.outLocationComboBox.DisplayMember = "Value";
                this.outLocationComboBox.ValueMember = "Key";

                this.reportRowBindingSource.ResetBindings(true);
                this.EnableSync(true);
                MessageBox.Show(Properties.Resources.PreviewCompletedMessage, Properties.Resources.Information, MessageBoxButtons.OK, MessageBoxIcon.Information);
            });
        }

        void xmlAdapter_PreviewStep(object sender, LengthlyOperationStepEventArgs e)
        {
            lock (sync)
            {
                this.Invoke((MethodInvoker)delegate { this.toolStripProgressBar1.ProgressBar.Value = e.Position; });
            }
        }

        void xmlAdapter_PreviewStart(object sender, LengthlyOperationStartEventArgs e)
        {
            lock (sync)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    this.toolStripProgressBar1.ProgressBar.Maximum = e.StepsCount != 0 ? e.StepsCount - 1 : 0;
                });
            }
        }

        private void EnableSync(bool enable)
        {
            bool tmp = enable && !this.xmlAdapter.IsSynchronized;
            this.syncButton.Enabled = tmp && this.xmlAdapter.IsPreviewed;
            this.outLocationComboBox.Enabled = tmp && this.xmlAdapter.IsPreviewed;
            this.locateButton.Enabled = tmp;
            this.scanButton.Enabled = tmp;
            this.finishedButton.Enabled = tmp;
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void outLocationComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.xmlAdapter != null)
            {
                foreach (DataGridViewRow row in this.logGridView.SelectedRows)
                {
                    ReportRow reportRow = row.DataBoundItem as ReportRow;
                    if (reportRow != null)
                    {
                        reportRow.Selected = true;
                        reportRow.LocationOut = (long)outLocationComboBox.SelectedValue;
                    }
                }
                this.reportRowBindingSource.ResetBindings(false);
            }
        }

        private void logGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void dutchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ChangeUICulture("nl");
        }

        private void ChangeUICulture(string cultureName)
        {
            Properties.Settings.Default.UICulture = cultureName;
            Properties.Settings.Default.Save();

            MessageBox.Show(Properties.Resources.LanguageChangeMessage, Properties.Resources.Information, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeUICulture("en");
        }

        private void barcodeTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) scanButton_Click(sender, e);
        }

        private void logGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow item in logGridView.Rows)
            {
                var data = item.DataBoundItem as ReportRow;
                if (data != null && data.Selected)
                {
                    item.Selected = true;
                    data.Selected = false;
                }
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(string.Format("Asset manager - Invent Scanner version {0}",
                ConfigurationManager.AppSettings["SetupVersion"]), "Info");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}