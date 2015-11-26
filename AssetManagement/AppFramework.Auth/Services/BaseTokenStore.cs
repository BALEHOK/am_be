using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppFramework.Auth.Data;
using AppFramework.Auth.Data.Models;
using AppFramework.Auth.Serialization;
using IdentityServer3.Core.Models;
using Newtonsoft.Json;

namespace AppFramework.Auth.Services
{
    public class BaseTokenStore
    {
        private static JsonSerializerSettings _jsonSerializerSettings;
        protected static JsonSerializerSettings JsonSerializerSettings
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
    }

    // ToDo use async context methods after updating to EF6
    public abstract class BaseTokenStore<TToken> : BaseTokenStore where TToken : class
    {
        protected readonly TokenType TokenType;

        protected readonly AuthContext Context;

        protected BaseTokenStore(AuthContext context, TokenType tokenType)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            Context = context;

            TokenType = tokenType;
        }

        /// <summary>
        /// Retrieves the data.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public Task<TToken> GetAsync(string key)
        {
            var token = Context.Tokens.Find(key, TokenType);
            return
                Task.FromResult(
                    token == null || token.Expiry < DateTimeOffset.UtcNow
                        ? default(TToken)
                        : ConvertFromJson(token.JsonCode));
        }

        /// <summary>
        /// Removes the data.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public Task RemoveAsync(string key)
        {
            var token = Context.Tokens.Find(key, TokenType);
            var num = 0;
            if (token != null)
            {
                Context.Tokens.Remove(token);
                num = Context.SaveChanges();
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
            var tokens = Context.Tokens
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
            foreach (var token in Context.Tokens
                .Where(t => t.SubjectId == subject && t.ClientId == client && t.TokenType == TokenType))
            {
                Context.Tokens.Remove(token);
            }
            
            var num = Context.SaveChanges();
            return Task.FromResult(num);
        }

        protected string ConvertToJson(TToken value)
        {
            return JsonConvert.SerializeObject(value, JsonSerializerSettings);
        }

        protected TToken ConvertFromJson(string json)
        {
            return JsonConvert.DeserializeObject<TToken>(json, JsonSerializerSettings);
        }

        public abstract Task StoreAsync(string key, TToken value);
    }
}
