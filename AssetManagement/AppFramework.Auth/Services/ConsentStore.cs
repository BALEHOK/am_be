using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppFramework.Auth.Data;
using AppFramework.Auth.Data.Models;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;

namespace AppFramework.Auth.Services
{
    public class ConsentStore : IConsentStore
    {
        private readonly AuthContext _context;

        public ConsentStore(AuthContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
        }

        public async Task<Consent> LoadAsync(string subject, string client)
        {
            var found = await Task.FromResult(_context.Consents.Find(subject, client));
            if (found == null)
            {
                return null;
            }

            var result = new Consent
            {
                Subject = found.Subject,
                ClientId = found.ClientId,
                Scopes = ParseScopes(found.Scopes)
            };

            return result;
        }

        public async Task UpdateAsync(Consent consent)
        {
            var item = await Task.FromResult(_context.Consents.Find(consent.Subject, consent.ClientId));
            if (item == null)
            {
                item = new ConsentModel
                {
                    Subject = consent.Subject,
                    ClientId = consent.ClientId
                };
                _context.Consents.Add(item);
            }

            if (consent.Scopes == null || !consent.Scopes.Any())
            {
                _context.Consents.Remove(item);
            }

            item.Scopes = StringifyScopes(consent.Scopes);

            _context.SaveChanges();
        }

        public async Task<IEnumerable<Consent>> LoadAllAsync(string subject)
        {
            var found = await Task.FromResult(_context.Consents.Where(x => x.Subject == subject).ToArray());

            var results = found.Select(x => new Consent
            {
                Subject = x.Subject,
                ClientId = x.ClientId,
                Scopes = ParseScopes(x.Scopes)
            });

            return results.ToArray().AsEnumerable();
        }

        private static IEnumerable<string> ParseScopes(string scopes)
        {
            if (string.IsNullOrWhiteSpace(scopes))
            {
                return Enumerable.Empty<string>();
            }

            return scopes.Split(',');
        }

        private static string StringifyScopes(IEnumerable<string> scopes)
        {
            if (scopes == null || !scopes.Any())
            {
                return null;
            }

            return scopes.Aggregate((s1, s2) => s1 + "," + s2);
        }

        public async Task RevokeAsync(string subject, string client)
        {
            var found = await Task.FromResult(_context.Consents.Find(subject, client));

            if (found != null)
            {
                _context.Consents.Remove(found);
                _context.SaveChanges();
            }
        }
    }
}
