using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AppFramework.Core.Classes;
using InventScanner.Core;

namespace InventScanner
{
    public partial class LocationExportCtrl : UserControl
    {
        public delegate void ExportCompliteDelegate(object sender, EventArgs e);
        public event ExportCompliteDelegate OnExportComplite;

        BackgroundWorker _exporter = new BackgroundWorker();

        public IEnumerable<Asset> Locations { get; set; }
        public XmlAdapters Adapters { get; private set; }

        public LocationExportCtrl()
        {
            InitializeComponent();

            this.Locations = new List<Asset>();
            this.Adapters = new XmlAdapters();

            _exporter.DoWork += new DoWorkEventHandler(_exporter_DoWork);
            _exporter.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_exporter_RunWorkerCompleted);
        }

        void _exporter_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (OnExportComplite != null)
            {
                this.Adapters = (XmlAdapters)e.Result;
                pbExport.MarqueeAnimationSpeed = 0;
                OnExportComplite(new object(), new EventArgs());
            }
        }

        void _exporter_DoWork(object sender, DoWorkEventArgs e)
        {
            XmlAdapters temp = new XmlAdapters();
            string path = @".\App_Data\";
            foreach (var location in (IEnumerable<Asset>)e.Argument)
            {
                XmlAdapter adapter = new XmlAdapter();
                adapter.LoadFromDatabase(location, path);
                temp.Add(adapter);
            }

            e.Result = temp;
        }

        private void LocationExportCtrl_Load(object sender, EventArgs e)
        {
            pbExport.MarqueeAnimationSpeed = 100;
            _exporter.RunWorkerAsync(this.Locations);
        }

        public void Dispose()
        {
            _exporter.Dispose();

            base.Dispose(true);
        }
    }
}
