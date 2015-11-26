using System.Data.Entity.ModelConfiguration;
using AppFramework.Auth.Data.Models;

namespace AppFramework.Auth.Data.Mappings
{
    public class ConsentMapping : EntityTypeConfiguration<ConsentModel>
    {
        public ConsentMapping()
        {
            HasKey(t => new
            {
                t.Subject,
                t.ClientId
            });

            Property(t => t.Subject).HasMaxLength(200).IsRequired();
            Property(t => t.ClientId).HasMaxLength(200).IsRequired();
            Property(t => t.Scopes).HasMaxLength(2000).IsRequired();
        }
    }
}