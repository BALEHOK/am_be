using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using AssetManager.Infrastructure.Models;
using AssetManager.Infrastructure.Models.TypeModels;
using AssetManagerAdmin.Model;
using RestSharp;

namespace AssetManagerAdmin.WebApi
{
    public class AssetsApi : IAssetsApi
    {
        private readonly UserInfo _user;
        private readonly string _baseAddress;
        private readonly JavaScriptSerializer _js = new JavaScriptSerializer();

        public AssetsApi(string baseAddress, UserInfo user)
        {
            _user = user;
            _baseAddress = baseAddress;
        }

        private string GetApiUrl(string api)
        {
            var url = string.Format("{0}{1}", _baseAddress, api);
            return url;
        }

        private void Authorize(HttpClient client)
        {
            // ToDo [Alexandr Shukletsov] refresh token when required
            client.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer {0}", _user.AccessToken));
        }

        private readonly SemaphoreSlim _typesInfoModelSemaphore = new SemaphoreSlim(1);
        private bool _typesInfoModelRequestInProgress;
        private TypesInfoModel _typesInfoModelResponse;

        public virtual async Task<TypesInfoModel> GetTypesInfo()
        {
            // if no request in progress, go to API server
            // otherwise
            // wait for request to finish and return its result
            if (!_typesInfoModelRequestInProgress)
            {
                _typesInfoModelRequestInProgress = true;
                await _typesInfoModelSemaphore.WaitAsync();

                try
                {
                    string json;
                    using (var client = new HttpClient())
                    {
                        Authorize(client);
                        json = await client.GetStringAsync(GetApiUrl("/api/typesinfo"));
                    }

                    _typesInfoModelResponse = _js.Deserialize<TypesInfoModel>(json);

                    _typesInfoModelRequestInProgress = false;

                    return _typesInfoModelResponse;
                }
                finally
                {
                    _typesInfoModelSemaphore.Release();
                }
            }

            await _typesInfoModelSemaphore.WaitAsync();
            _typesInfoModelSemaphore.Release();

            return _typesInfoModelResponse;
        }

        public async Task<List<CustomReportModel>> GetReportsList()
        {
            using (var client = new HttpClient())
            {
                Authorize(client);

                var apiRequest = GetApiUrl("/api/reports/custom/list");

                var response = await client.GetStringAsync(apiRequest);
                
                return _js.Deserialize<List<CustomReportModel>>(response);
            }
        }

        public async Task<long> CreateReport(string reportName, long assetTypeId)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(_baseAddress);
            client.AddDefaultHeader("Authorization", string.Format("Bearer {0}", _user.AccessToken));

            var request = new RestRequest();
            request.Resource = "api/reports/custom/create";
            request.Method = Method.PUT;
            request.AddParameter("reportName", reportName, ParameterType.QueryString);
            request.AddParameter("assetTypeId", assetTypeId, ParameterType.QueryString);
        
            var response = await client.ExecuteTaskAsync<long>(request);
            return response.Data;
        }

        public async Task DeleteReport(long reportId)
        {
            var client = new RestClient();
            client.BaseUrl = new Uri(_baseAddress);
            client.AddDefaultHeader("Authorization", string.Format("Bearer {0}", _user.AccessToken));

            var request = new RestRequest();
            request.Resource = "api/reports/custom/delete";
            request.Method = Method.DELETE;
            request.AddParameter("reportId", reportId, ParameterType.QueryString);

            await client.ExecuteTaskAsync<long>(request);
        }

        public async Task<string> SaveFormula(AssetTypeModel assetType, string attributeName, string formula)
        {
            using (var client = new HttpClient())
            {
                Authorize(client);

                var apiRequest = !string.IsNullOrWhiteSpace(formula)
                    ? string.Format("/api/typesinfo/formula?typeId={0}&attributeName={1}&formula={2}",
                        assetType.Id, HttpUtility.UrlEncode(attributeName), HttpUtility.UrlEncode(formula))
                    : string.Format("/api/typesinfo/formula/clear?typeId={0}&attributeName={1}",
                        assetType.Id, HttpUtility.UrlEncode(attributeName));

                var response = await client.GetStringAsync(GetApiUrl(apiRequest));
                return response;
            }
        }

        public async Task<string> SaveScreenFormula(ScreenPanelAttributeModel panelAttribute)
        {
            using (var client = new HttpClient())
            {
                Authorize(client);

                var apiRequest = !string.IsNullOrWhiteSpace(panelAttribute.ScreenFormula)
                    ? string.Format("/api/typesinfo/screens/formula?panelAttributeId={0}&formula={1}",
                        panelAttribute.Id, HttpUtility.UrlEncode(panelAttribute.ScreenFormula))
                    : string.Format("/api/typesinfo/screens/formula/clear?panelAttributeId={0}",
                        panelAttribute.Id);

                var response = await client.GetStringAsync(GetApiUrl(apiRequest));
                return response;
            }
        }

        public async Task<string> SaveValidation(AssetTypeModel assetType, string attributeName, string expression)
        {
            using (var client = new HttpClient())
            {
                Authorize(client);

                var apiRequest = !string.IsNullOrWhiteSpace(expression)
                    ? string.Format("/api/typesinfo/validation?typeId={0}&attributeName={1}&expression={2}",
                        assetType.Id, HttpUtility.UrlEncode(attributeName), HttpUtility.UrlEncode(expression))
                    : string.Format("/api/typesinfo/validation/clear?typeId={0}&attributeName={1}",
                        assetType.Id, HttpUtility.UrlEncode(attributeName));

                var response = await client.GetStringAsync(GetApiUrl(apiRequest));
                return response;
            }
        }

        public async Task<AttributeValidationResultModel> ValidateAttributeAsync(long id, string value,
            string expression)
        {
            using (var client = new HttpClient())
            {
                Authorize(client);

                var apiRequest = string.Format("/api/validation/attribute/{0}?value={1}&expression={2}",
                    id, HttpUtility.UrlEncode(value), HttpUtility.UrlEncode(expression));

                var response = await client.GetStringAsync(GetApiUrl(apiRequest));
                var result = _js.Deserialize<AttributeValidationResultModel>(response);
                return result;
            }
        }

        public async Task UploadFile(byte[] file, string fileName)
        {
            using (var client = new HttpClient())
            {
                Authorize(client);

                var apiRequest = string.Format("/api/uploads?assetTypeId={0}&attributeId={1}", 0, 0);
                var url = GetApiUrl(apiRequest);

                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StreamContent(new MemoryStream(file)), "report_template", fileName);

                    using (var response = await client.PostAsync(url, content))
                    {
                        await response.Content.ReadAsStringAsync();
                    }
                }
            }
        }
    }
}