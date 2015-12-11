using System.Collections.Generic;

namespace AssetManager.Infrastructure.Models
{
    public class FaqContainer
    {
        public IEnumerable<FaqModel> Items { get; set; }

        public long FaqAssetTypeId { get; set; }
    }
}
