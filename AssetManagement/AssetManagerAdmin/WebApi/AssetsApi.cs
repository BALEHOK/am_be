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
        private readonly Uri _baseAddress;
        private readonly JavaScriptSerializer _js = new JavaScriptSerializer();

        public AssetsApi(string baseAddress, UserInfo user)
        {
            _user = user;
            _baseAddress = new Uri(baseAddress);
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
                        client.BaseAddress = _baseAddress;
                        Authorize(client);
                        var response = await client.GetAsync(ApiUrls.TypesInfo);
                        response.EnsureSuccessStatusCode();
                        json = await response.Content.ReadAsStringAsync();
                    }

                    _typesInfoModelResponse = _js.Deserialize<TypesInfoModel>(json);
                    return _typesInfoModelResponse;
                }
                catch
                {
                    throw;
                }
                finally
                {
                    _typesInfoModelRequestInProgress = false;
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
                client.BaseAddress = _baseAddress;
                Authorize(client);
                var response = await client.GetAsync(ApiUrls.CustomReportsList);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                return _js.Deserialize<List<CustomReportModel>>(result);
            }
        }

        public async Task<long> CreateReport(string reportName, long assetTypeId)
        {
            var client = new RestClient();
            client.BaseUrl = _baseAddress;
            client.AddDefaultHeader("Authorization", string.Format("Bearer {0}", _user.AccessToken));

            var request = new RestRequest();
            request.Resource = ApiUrls.CustomReportsCreate;
            request.Method = Method.PUT;
            request.AddParameter("reportName", reportName, ParameterType.QueryString);
            request.AddParameter("assetTypeId", assetTypeId, ParameterType.QueryString);
        
            var response = await client.ExecuteTaskAsync<long>(request);
            return response.Data;
        }

        public async Task DeleteReport(long reportId)
        {
            var client = new RestClient();
            client.BaseUrl = _baseAddress;
            client.AddDefaultHeader("Authorization", string.Format("Bearer {0}", _user.AccessToken));

            var request = new RestRequest();
            request.Resource = ApiUrls.CustomReportsDelete;
            request.Method = Method.DELETE;
            request.AddParameter("reportId", reportId, ParameterType.QueryString);

            await client.ExecuteTaskAsync<long>(request);
        }

        public async Task<string> SaveFormula(AssetTypeModel assetType, string attributeName, string formula)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = _baseAddress;
                Authorize(client);

                var apiRequest = !string.IsNullOrWhiteSpace(formula)
                    ? ApiUrls.TypesInfoFormula + string.Format("?typeId={0}&attributeName={1}&formula={2}",
                        assetType.Id, HttpUtility.UrlEncode(attributeName), HttpUtility.UrlEncode(formula))
                    : ApiUrls.TypesInfoFormulaClear + string.Format("?typeId={0}&attributeName={1}",
                        assetType.Id, HttpUtility.UrlEncode(attributeName));

                var response = await client.GetAsync(apiRequest);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
        }

        public async Task<string> SaveScreenFormula(ScreenPanelAttributeModel panelAttribute)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = _baseAddress;
                Authorize(client);

                var apiRequest = !string.IsNullOrWhiteSpace(panelAttribute.ScreenFormula)
                    ? ApiUrls.TypesInfoScreensFormula + string.Format("?panelAttributeId={0}&formula={1}",
                        panelAttribute.Id, HttpUtility.UrlEncode(panelAttribute.ScreenFormula))
                    : ApiUrls.TypesInfoScreensFormulaClear + string.Format("?panelAttributeId={0}",
                        panelAttribute.Id);

                var response = await client.GetAsync(apiRequest);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
        }

        public async Task<string> SaveValidation(AssetTypeModel assetType, string attributeName, string expression)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = _baseAddress;
                Authorize(client);

                var apiRequest = !string.IsNullOrWhiteSpace(expression)
                    ? ApiUrls.TypesInfoValidation + string.Format("?typeId={0}&attributeName={1}&expression={2}",
                        assetType.Id, HttpUtility.UrlEncode(attributeName), HttpUtility.UrlEncode(expression))
                    : ApiUrls.TypesInfoValidationClear + string.Format("?typeId={0}&attributeName={1}",
                        assetType.Id, HttpUtility.UrlEncode(attributeName));

                var response = await client.GetAsync(apiRequest);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
        }

        public async Task<AttributeValidationResultModel> ValidateAttributeAsync(long id, string value,
            string expression)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = _baseAddress;
                Authorize(client);

                var apiRequest = string.Format("{0}?value={1}&expression={2}",
                    string.Format(ApiUrls.ValidationAttributeFormat, id), 
                    HttpUtility.UrlEncode(value), 
                    HttpUtility.UrlEncode(expression));

                var response = await client.GetAsync(apiRequest);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                return _js.Deserialize<AttributeValidationResultModel>(result);
            }
        }

        public async Task UploadFile(byte[] file, string fileName)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = _baseAddress;
                Authorize(client);

                var apiRequest = ApiUrls.Uploads + string.Format("?assetTypeId={0}&attributeId={1}", 0, 0);

                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StreamContent(new MemoryStream(file)), "report_template", fileName);

                    using (var response = await client.PostAsync(apiRequest, content))
                    {
                        response.EnsureSuccessStatusCode();
                    }
                }
            }
        }
    }
}