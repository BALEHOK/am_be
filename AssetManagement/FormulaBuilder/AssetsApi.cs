using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using AppFramework.Core.Classes.Validation;
using AppFramework.Core.Validation;
using AssetManager.Infrastructure.Models;
using AssetManager.Infrastructure.Models.TypeModels;

namespace FormulaBuilder
{
    public class AssetsApi
    {
        private readonly string _baseAddress;

        public AssetsApi(string baseAddress)
        {
            _baseAddress = baseAddress;
        }

        private string GetApiUrl(string api)
        {
            var url = string.Format("{0}{1}", _baseAddress, api);
            return url;
        }

        public async Task<TypesInfoModel> GetTypesInfo()
        {
            var tokenResponse = HttpPost(GetApiUrl("/token"),
                new[] {"username", "password", "grant_type"}, new[] {"admin", "admin", "password"});

            var js = new JavaScriptSerializer();
            var token = js.Deserialize<TokenResponse>(tokenResponse);

            TypesInfoModel types;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer {0}", token.access_token));
                await client.GetAsync(GetApiUrl("/api/auth"));

                var json = await client.GetStringAsync(GetApiUrl("/api/typesinfo"));
                types = js.Deserialize<TypesInfoModel>(json);
            }

            return types;
        }

        public async Task<string> SaveFormula(AssetTypeModel assetType, string attributeName, string formula)
        {
            //todo: remove copy-paste
            var tokenResponse = HttpPost(GetApiUrl("/token"),
                new[] {"username", "password", "grant_type"}, new[] {"admin", "admin", "password"});

            var js = new JavaScriptSerializer();
            var token = js.Deserialize<TokenResponse>(tokenResponse);
            
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer {0}", token.access_token));
                await client.GetAsync(GetApiUrl("/api/auth"));

                var apiRequest = string.Format("/api/typesinfo/formula?typeId={0}&attributeName={1}&formula={2}",
                    assetType.Id, HttpUtility.UrlEncode(attributeName), HttpUtility.UrlEncode(formula));
                
                var response = await client.GetStringAsync(GetApiUrl(apiRequest));
                return response;
            }
        }

        public async Task<string> SaveValidation(AssetTypeModel assetType, string attributeName, string expression)
        {
            //todo: remove copy-paste
            var tokenResponse = HttpPost(GetApiUrl("/token"),
                new[] { "username", "password", "grant_type" }, new[] { "admin", "admin", "password" });

            var js = new JavaScriptSerializer();
            var token = js.Deserialize<TokenResponse>(tokenResponse);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer {0}", token.access_token));
                await client.GetAsync(GetApiUrl("/api/auth"));

                var apiRequest = string.Format("/api/typesinfo/validation?typeId={0}&attributeName={1}&expression={2}",
                    assetType.Id, HttpUtility.UrlEncode(attributeName), HttpUtility.UrlEncode(expression));

                var response = await client.GetStringAsync(GetApiUrl(apiRequest));
                return response;
            }
        }

        public async Task<AttributeValidationResultModel> ValidateAttributeAsync(long id, string value, string expression)
        {
            //todo: remove copy-paste
            var tokenResponse = HttpPost(GetApiUrl("/token"),
                new[] { "username", "password", "grant_type" }, new[] { "admin", "admin", "password" });

            var js = new JavaScriptSerializer();
            var token = js.Deserialize<TokenResponse>(tokenResponse);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer {0}", token.access_token));
                await client.GetAsync(GetApiUrl("/api/auth"));

                var apiRequest = string.Format("/api/validation/attribute/{0}?value={1}&expression={2}",
                    id, HttpUtility.UrlEncode(value), HttpUtility.UrlEncode(expression));

                var response = await client.GetStringAsync(GetApiUrl(apiRequest));
                var result = js.Deserialize<AttributeValidationResultModel>(response);
                return result;
            }
        }

        private static string HttpPost(string url, string[] paramName, string[] paramVal)
        {
            var request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            var parameters = new StringBuilder();
            for (int i = 0; i < paramName.Length; i++)
            {
                parameters.Append(paramName[i]);
                parameters.Append("=");
                parameters.Append(HttpUtility.UrlEncode(paramVal[i]));
                parameters.Append("&");
            }
            
            byte[] formData = Encoding.UTF8.GetBytes(parameters.ToString());
            request.ContentLength = formData.Length;

            using (var post = request.GetRequestStream())
            {
                post.Write(formData, 0, formData.Length);
            }
            
            string result;
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                var reader = new StreamReader(response.GetResponseStream());
                result = reader.ReadToEnd();
            }

            return result;
        }
    }
}
