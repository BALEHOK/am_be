namespace AssetManager.Infrastructure.Models.TypeModels
{
    public class ScreenPanelAttributeModel
    {
        public long Id { get; set; }
        public long AttributeId { get; set; }
        public string ScreenFormula { get; set; }
        public AttributeTypeModel AttributeModel { get; set; }
    }
}