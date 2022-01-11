using APP.Base.Model.Enum;
using APP.Base.Model.Model;
using APP.Infra.Base.BaseResult;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


namespace APP.Infra.Base.BaseHandler
{
    public class ApiHandler
    {
        // private readonly Action<HttpClient, string> _requestHeader;
        private readonly IHttpClientFactory _httpClientFactory;
        public AuthenticationModel _authenticationModel;
        public Guid? TransactionId { get; set; }
        public string SourceApi { get; set; }
        private const string RequestSource = "APP";
        private readonly string basePath = "https://app-user-service.azurewebsites.net";
        //private readonly string basePath = "http://localhost:44352";
        public ApiHandler()
        {
            _authenticationModel = new AuthenticationModel();
            _httpClientFactory = ServiceLocator.Current?.GetInstance<IHttpClientFactory>();
        }
        private void SetRequestHeader(HttpClient client, string contentType = "application/json")
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
            if (!string.IsNullOrWhiteSpace(_authenticationModel.TokenCipherText))
            {
                if (_authenticationModel.TokenCipherText.ToLower().Contains("bearer"))
                {
                    _authenticationModel.TokenCipherText = _authenticationModel.TokenCipherText.Replace("BEARER", "").Replace("Bearer", "").Trim();
                }
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authenticationModel.TokenCipherText);
            }

            client.DefaultRequestHeaders.Add("RequestSource", RequestSource);
        }

        private async Task<string> Send(string path, RequestType requestType, object data)
        {
            var client = _httpClientFactory.CreateClient();
            SetRequestHeader(client, (requestType == RequestType.PUT ? "application/x-www-form-urlencoded" : "application/json"));

            HttpResponseMessage response = null;
            if (requestType == RequestType.GET)
            {
                response = await client.GetAsync($"{basePath}/{path}");
            }
            else
            {
                var content = JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                });

                var httpContent = new StringContent(content, Encoding.UTF8, "application/json");
                response = requestType switch
                {
                    RequestType.POST => await client.PostAsync($"{basePath}/{path}", httpContent),
                    RequestType.PUT => await client.PutAsync($"{basePath}/{path}", httpContent),
                    RequestType.DELETE => await client.DeleteAsync($"{basePath}/{path}"),
                    _ => response
                };
            }

            var responseString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode) return responseString;
            if (response.StatusCode == HttpStatusCode.NotFound) return responseString;
            var exception = response.StatusCode switch
            {
                System.Net.HttpStatusCode.Forbidden => new Exception("E403"),
                System.Net.HttpStatusCode.Unauthorized => new Exception("E401"),
                _ => new Exception(response.ReasonPhrase)
            };

            exception.Data.Add("ApiExceptionMessage",
                !string.IsNullOrEmpty(responseString)
                    ? JsonConvert.DeserializeObject<ApiResult>(responseString).Message
                    : exception.Message);

            throw exception;

        }

        public async Task<ApiResult<T>> GetApiResult<T>(string url)
        {
            var responseString = await Send(url, RequestType.GET, null);

            var apiResult = JsonConvert.DeserializeObject<ApiResult<T>>(responseString);
            return apiResult;
        }

        public async Task<T> Get<T>(string url)
        {
            var responseString = await Send(url, RequestType.GET, null);

            var p = default(T);

            var apiResult = JsonConvert.DeserializeObject<ApiResult<T>>(responseString);
            if (!apiResult.Result) return p;
            if (apiResult.Data != null)
            {
                p = apiResult.Data;
            }

            return p;
        }

        public async Task<IEnumerable<T>> GetMany<T>(string url)
        {
            var responseString = await Send(url, RequestType.GET, null);

            var p = default(IEnumerable<T>);

            var apiResult = JsonConvert.DeserializeObject<ApiResult<IEnumerable<T>>>(responseString);
            if (!apiResult.Result) return p;
            if (apiResult.Data != null)
            {
                p = apiResult.Data;
            }

            return p;
        }
    }
    
}
