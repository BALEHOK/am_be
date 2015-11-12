using System;

namespace AppFramework.Auth.Data.Models
{
    public class TokenModel
    {
        public string Key { get; set; }

        public TokenType TokenType { get; set; }

        public string SubjectId { get; set; }

        public string ClientId { get; set; }

        public string JsonCode { get; set; }

        public DateTimeOffset Expiry { get; set; }
    }
}