using System;
using System.ComponentModel.DataAnnotations;

namespace AppFramework.Core.DTO
{
    public class PlainAssetDTO
    {
        public string Name { get; set; }
        
        public long AssetTypeId { get; set; }

        public long Id { get; set; }        

        public int Revision { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
