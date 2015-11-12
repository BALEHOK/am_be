namespace AssetManager.Infrastructure.Models
{
    public class TaskModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public long DynEntityConfigId { get; set; }

        public string DynEntityConfigName { get; set; }
    }
}
