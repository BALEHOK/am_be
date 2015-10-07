namespace StockScanner
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;
    using AppFramework.ConstantsEnumerators;
    using AppFramework.Core.AC.Authentication;
    using AppFramework.Core.Classes;
    using AppFramework.Core.Classes.SearchEngine;
    using AppFramework.Core.Classes.Stock;
    using LogTable;

    public partial class Main : Form
    {
        private volatile LogTable logTable;

        public Main()
        {
            InitializeComponent();
            logTable = new LogTable();
            this.dataGridView1.DataSource = this.logTable;
            this.comboBox1.SelectedIndex = 0;

            UpdateUserList();
            UpdateLocationsList(ToLocationsList, false);
        }

        private void UpdateLocationsList(ComboBox listControl, bool showOnlyAvailable)
        {
            long id = PredefinedAttribute.GetDynEntityConfigId(PredefinedEntity.Location);
            var locationsList = AssetFactory.GetAssetsByAssetType(AssetType.GetByID(id), false).ToList();
            if (showOnlyAvailable)
            {
                var asset = SearchEngine.FindByBarcode(textBox1.Text).FirstOrDefault();
                if (asset != null)
                {
                    var manager = new AppFramework.Core.Classes.Stock.StockTransactionManager();
                    var available = manager.GetAvailableLocationsFor(asset.ID, asset.GetConfiguration().ID);
                    locationsList = locationsList.Where(location => available.ContainsKey(location.ID)).ToList();
                }
                else
                {
                    locationsList = locationsList.Where(l => false).ToList();
                }
            }

            listControl.DataSource = locationsList;
            listControl.DisplayMember = "Name";
            listControl.ValueMember = "ID";
        }

        private void UpdateUserList()
        {
            long id = PredefinedAttribute.GetDynEntityConfigId(PredefinedEntity.User);
            this.endUsersList.DataSource = AssetFactory.GetAssetsByAssetType(AssetType.GetByID(id), false).ToList();
            this.endUsersList.DisplayMember = "Name";
            this.endUsersList.ValueMember = "ID";
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                comboBox1.Focus();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.ProcessByBarcode();
        }

        /// <summary>
        /// Processes the asset by barcode.
        /// </summary>
        /// <param name="barcode">The barcode.</param>
        private void ProcessByBarcode(Asset asset)
        {
            StockTransactionManager manager = new StockTransactionManager();
            TransactionTypeCode code = (TransactionTypeCode)(this.comboBox1.SelectedIndex + 1);
            decimal count = 0, price = 0;
            decimal.TryParse(this.priceTextBox.Text, out price);
            decimal.TryParse(this.countTextBox.Text, out count);
            LogRecord rec;
            long endUser = 0, toLocation = 0, fromLocation = 0;

            if (this.endUsersList.SelectedValue != null)
                long.TryParse(this.endUsersList.SelectedValue.ToString(), out endUser);

            if (this.ToLocationsList.SelectedValue != null)
                long.TryParse(this.ToLocationsList.SelectedValue.ToString(), out toLocation);

            if (this.FromLocationsList.SelectedValue != null)
                long.TryParse(this.FromLocationsList.SelectedValue.ToString(), out fromLocation);

            lock (this.logTable)
            {
                if (asset != null)
                {
                    if (asset.GetConfiguration().IsInStock)
                    {
                        try
                        {
                            rec = manager.CreateTransaction(asset, code, count, price, "", endUser, toLocation, fromLocation);
                        }
                        catch (InvalidOperationException ioe)
                        {
                            rec = LogRecord.GetDefaultRecord();
                            MessageBox.Show(ioe.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        rec = LogRecord.GetDefaultRecord();
                        rec.Title = string.Format("Asset {0} is not in stock", textBox1.Text);
                        rec.Status = LogRecordStatus.Error;
                    }
                }
                else
                {
                    rec = LogRecord.GetDefaultRecord();
                    rec.Title = string.Format("Asset {0} is missed", textBox1.Text);
                    rec.Status = LogRecordStatus.Error;
                }

                this.logTable.AddRow(rec);
                this.textBox2.Text += string.Join(Environment.NewLine, manager.Output.ToArray()) + Environment.NewLine;
            }
            this.FormCleanup();
        }

        /// <summary>
        /// Processes the asset by barcode.
        /// </summary>
        private void ProcessByBarcode()
        {
            if (!string.IsNullOrEmpty(this.textBox1.Text))
            {
                Asset asset = SearchEngine.FindByBarcode(this.textBox1.Text).FirstOrDefault();
                this.ProcessByBarcode(asset);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateParametersSelection(this.comboBox1.SelectedIndex);
        }

        private void UpdateParametersSelection(int modeSelectionIndex)
        {
            this.pricePanel.Enabled = false;
            this.userSelectionPanel.Enabled = false;
            this.toLocationSelectionPanel.Enabled = false;
            this.fromLocationSelectionPanel.Enabled = false;

            switch (modeSelectionIndex)
            {
                case 0:
                    this.pricePanel.Enabled = true;
                    this.toLocationSelectionPanel.Enabled = true;
                    break;
                case 1:
                    this.fromLocationSelectionPanel.Enabled = true;
                    this.userSelectionPanel.Enabled = true;
                    UpdateLocationsList(FromLocationsList, true);
                    break;
                case 2:
                    this.toLocationSelectionPanel.Enabled = true;
                    this.fromLocationSelectionPanel.Enabled = true;
                    UpdateLocationsList(FromLocationsList, true);
                    break;
                default:
                    break;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex] is DataGridViewLinkColumn)
            {
                ViewInfo viewer = new ViewInfo();
                viewer.InfoMessage = this.logTable.Rows[e.RowIndex]["InfoMessage"].ToString();
                viewer.ShowDialog();
            }
        }
      
        /// <summary>
        /// Cleanups the form - text fields and comboboxes
        /// </summary>
        private void FormCleanup()
        {
            this.textBox1.Clear();
            this.priceTextBox.Text = "0";
            this.countTextBox.Text = "0";
        }

        private void Main_Load(object sender, EventArgs e)
        {
            Login login = new Login();
            if ((login.ShowDialog() != DialogResult.OK))
            {
                this.Close();
                return;
            }
            else if (login.resumeLastSession.Checked)
            {
                this.logTable.LoadLog(@"LastSession.xml");
            }

            string cName = Thread.CurrentThread.CurrentUICulture.Name;
            if (cName == "en")
                englishToolStripMenuItem1.Checked = true;
            else
                dutchToolStripMenuItem1.Checked = true;

            if (!AccessManager.Instance.IsActual)
            {
                AccessManager.Instance.InitRights();
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.logTable.SaveLog("LastSession.xml");
        }

        private void ChangeUICulture(string culture)
        {
            Properties.Settings.Default.UICulture = culture;
            Properties.Settings.Default.Save();

            MessageBox.Show(Properties.Resources.LanguageChangeMessage, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void englishToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ChangeUICulture("en");
        }

        private void dutchToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ChangeUICulture("nl");
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            UpdateParametersSelection(this.comboBox1.SelectedIndex);
        }
    }
}