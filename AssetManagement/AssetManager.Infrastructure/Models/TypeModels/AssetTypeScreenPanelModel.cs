using System.Collections.Generic;

namespace AssetManager.Infrastructure.Models.TypeModels
{
    /// <summary>
    /// Asset type screen panel model
    /// </summary>
    public class AssetTypeScreenPanelModel
    {
        /// <summary>
        /// Panel Id
        /// </summary>
        public long Id { get; set; }
        public AssetTypeScreenModel ScreenModel { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
        
        /// <summary>
        /// A list of panel attributes
        /// </summary>
        public List<ScreenPanelAttributeModel> Attributes { get; set; }

        public AssetTypeScreenPanelModel()
        {
            Attributes = new List<ScreenPanelAttributeModel>();
        }
    }
}