using System;

namespace AppFramework.Entities
{
    public interface ISearchCounter
    {
        int? Count { get; set; }
        long? id { get; set; }
        //string Name { get; set; }
        Guid? SearchId { get; set; }
        string Type { get; set; }
        long? UserId { get; set; }
    }
}
