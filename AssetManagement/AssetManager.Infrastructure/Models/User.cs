using System;

namespace AssetManager.Infrastructure.Models
{
    public class User
    {
        public string UserName { get; set; }

        public DateTime LastLogin { get; set; }

        public string Email { get; set; }

        public string UserRights { get; set; }
        public string UserRole { get; set; }
    }
}