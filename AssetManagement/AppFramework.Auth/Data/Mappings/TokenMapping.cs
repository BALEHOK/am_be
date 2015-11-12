using System.Data.Entity.ModelConfiguration;
using AppFramework.Auth.Data.Models;

namespace AppFramework.Auth.Data.Mappings
{
    public class TokenMapping : EntityTypeConfiguration<TokenModel>
    {
        public TokenMapping()
        {
            HasKey(t => new
            {
                t.Key,
                t.TokenType
            });

            Property(t => t.SubjectId).HasMaxLength(200).IsRequired();
            Property(t => t.ClientId).HasMaxLength(200).IsRequired();

            Property(t => t.JsonCode).IsMaxLength().IsRequired();
        }
    }
}