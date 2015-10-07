using System.Collections.Generic;
using System.Linq;

namespace AssetManager.Infrastructure.Models.TypeModels
{
    /// <summary>
    /// Asset type screen model
    /// </summary>
    public class AssetTypeScreenModel
    {
        /// <summary>
        /// Screen Id
        /// </summary>        
        public long Id { get; set; }

        /// <summary>
        /// Screen Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A list of screen panels
        /// </summary>
        public List<AssetTypeScreenPanelModel> Panels { get; set; }

        /// <summary>
        /// A list of all screen attributes (without panels)
        /// </summary>
        public List<ScreenPanelAttributeModel> AllAttributes
        {
            get
            {
                return Panels.SelectMany(panel => panel.Attributes.Select(panelAttribute => panelAttribute)).ToList();
            }
        }

        public AssetTypeScreenModel()
        {
            Panels = new List<AssetTypeScreenPanelModel>();
        }
    }
}