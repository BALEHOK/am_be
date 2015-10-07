using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AppFramework.Core.Classes;
using AppFramework.ConstantsEnumerators;
using System.Threading;

namespace InventScanner
{
    public partial class LocationImporterCtrl : UserControl, IDisposable
    {
        public delegate void LoadingCompliteDelegate(object sender, EventArgs e);
        public event LoadingCompliteDelegate OnLocationsLoaded;

        BackgroundWorker _loader = new BackgroundWorker();

        private List<Asset> _assets;

        public LocationImporterCtrl()
        {
            InitializeComponent();

            _assets = new List<Asset>();

            _loader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_loader_RunWorkerCompleted);
            _loader.DoWork += new DoWorkEventHandler(_loader_DoWork);
        }

        private void LocationImporterCtrl_Load(object sender, EventArgs e)
        {
            pbLoad.MarqueeAnimationSpeed = 100;
            _loader.RunWorkerAsync();
        }

        void _loader_DoWork(object sender, DoWorkEventArgs e)
        {
            _assets = AssetFactory.GetAssetsByAssetType(
               AssetType.GetByID(PredefinedAttribute.GetDynEntityConfigId(PredefinedEntity.Location)),
               false).ToList();

        }

        void _loader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (OnLocationsLoaded != null)
            {
                pbLoad.MarqueeAnimationSpeed = 0;
                OnLocationsLoaded(new object(), new EventArgs());
            }
        }

        public List<Asset> GetLications()
        {
            return _assets;
        }

        public void Dispose()
        {
            if (_assets != null)
            {
                if (_assets.Count > 0)
                    _assets.Clear();

                _assets = null;
            }

            _loader.Dispose();

            base.Dispose(true);
        }
    }
}
