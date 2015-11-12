using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppFramework.Auth.Data;
using AppFramework.Auth.Data.Models;
using AppFramework.Auth.Serialization;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using Newtonsoft.Json;

namespace AppFramework.Auth.Services
{
    // ToDo use async context methods after updating to EF6
    /// <summary>
    /// Token handle store
    /// </summary>
    public class TokenHandleStore : ITokenHandleStore
    {
        private const TokenType TokenType = Data.Models.TokenType.TokenHandle;

        private readonly AuthContext _context;

        private static JsonSerializerSettings _jsonSerializerSettings;
        private static JsonSerializerSettings JsonSerializerSettings
        {
            get
            {
                if (_jsonSerializerSettings == null)
                {
                    _jsonSerializerSettings = new JsonSerializerSettings();
                    _jsonSerializerSettings.Converters.Add(new ClaimConverter());
                    _jsonSerializerSettings.Converters.Add(new ClaimsPrincipalConverter());
                    _jsonSerializerSettings.Converters.Add(new ClientConverter());
                    _jsonSerializerSettings.Converters.Add(new ScopeConverter());
                }
                return _jsonSerializerSettings;
            }
        }

        public TokenHandleStore(AuthContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Stores the data.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public Task StoreAsync(string key, Token value)
        {
            var token = new TokenModel
            {
                Key = key,
                SubjectId = value.SubjectId,
                ClientId = value.ClientId,
                JsonCode = ConvertToJson(value),
                Expiry = DateTimeOffset.UtcNow.AddSeconds(value.Lifetime),
                TokenType = TokenType
            };
            var added = _context.Tokens.Add(token);

            _context.SaveChanges();
            return Task.FromResult<object>(added);
        }

        /// <summary>
        /// Retrieves the data.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public Task<Token> GetAsync(string key)
        {
            var token = _context.Tokens.Find(key, TokenType);
            return
                Task.FromResult(
                    token == null || token.Expiry < DateTimeOffset.UtcNow
                        ? default(Token)
                        : ConvertFromJson(token.JsonCode));
        }

        /// <summary>
        /// Removes the data.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public Task RemoveAsync(string key)
        {
            var token = _context.Tokens.Find(key, TokenType);
            var num = 0;
            if (token != null)
            {
                _context.Tokens.Remove(token);
                num = _context.SaveChanges();
            }

            return Task.FromResult(num);
        }

        /// <summary>
        /// Retrieves all data for a subject identifier.
        /// </summary>
        /// <param name="subject">The subject identifier.</param>
        /// <returns>
        /// A list of token metadata
        /// </returns>
        public Task<IEnumerable<ITokenMetadata>> GetAllAsync(string subject)
        {
            var tokens = _context.Tokens
                .Where(t => t.SubjectId == subject && t.TokenType == TokenType)
                .Select(t => ConvertFromJson(t.JsonCode))
                .ToArray();

            return Task.FromResult(tokens.Cast<ITokenMetadata>());
        }

        /// <summary>
        /// Revokes all data for a client and subject id combination.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="client">The client.</param>
        /// <returns></returns>
        public Task RevokeAsync(string subject, string client)
        {
            foreach (var token in _context.Tokens
                .Where(t => t.SubjectId == subject && t.ClientId == client && t.TokenType == TokenType))
            {
                _context.Tokens.Remove(token);
            }
            
            var num = _context.SaveChanges();
            return Task.FromResult(num);
        }

        protected string ConvertToJson(Token value)
        {
            return JsonConvert.SerializeObject(value, JsonSerializerSettings);
        }

        protected Token ConvertFromJson(string json)
        {
            return JsonConvert.DeserializeObject<Token>(json, JsonSerializerSettings);
        }
    }
}