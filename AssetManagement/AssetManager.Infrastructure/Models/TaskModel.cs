namespace AssetManager.Infrastructure.Models
{
    public class TaskModel
    {
        public object Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public long DynEntityConfigId { get; set; }

        public string DynEntityConfigName { get; set; }

        public bool IsPredefined { get; set; }
    }
}
