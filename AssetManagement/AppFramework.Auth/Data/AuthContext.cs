using System.Data.Entity;
using System.Reflection;
using AppFramework.Auth.Data.Models;

namespace AppFramework.Auth.Data
{
    public class AuthContext : DbContext
    {
        public AuthContext(string connectionString)
            : base(connectionString)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<AuthContext>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.AddFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<TokenModel> Tokens { get; set; }
        public DbSet<ConsentModel> Consents{ get; set; }
    }
}