using Newtonsoft.Json;
using Solnet.Rpc.Messages;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Solnet.Rpc.Core.Http
{
    public abstract class JsonRpcClient
    {        
        private readonly HttpClient _httpClient;

        protected JsonRpcClient(string url, HttpClient httpClient = default)
        {
            _httpClient = httpClient ?? new HttpClient { BaseAddress = new Uri(url) };
        }

        protected async Task<RequestResult<T>> SendRequest<T>(JsonRpcRequest req)
        {
            var requestJson = JsonConvert.SerializeObject(req);

            //UnityEngine.Debug.Log($"\tRequest: {requestJson}");
            HttpResponseMessage response = null;
            RequestResult<T> result = null;

            try
            {
                response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, "/") { Content = new StringContent(requestJson, Encoding.UTF8, "application/json") });
            }
            catch (HttpRequestException e)
            {
                result = new RequestResult<T>(System.Net.HttpStatusCode.BadRequest, e.Message);
            }
            catch (Exception e)
            {
                result = new RequestResult<T>(System.Net.HttpStatusCode.BadRequest, e.Message);
            }

            result = new RequestResult<T>(response);
            if (result.WasHttpRequestSuccessful)
            {
                try
                {
                    var requestRes = await response.Content.ReadAsStringAsync();

                    //#TODO: replace with proper logging
                    //Console.WriteLine($"\tResult: {requestRes}");
                    //UnityEngine.Debug.Log(requestRes);
                    var res = JsonConvert.DeserializeObject<JsonRpcResponse<T>>(requestRes);//JsonUtility.FromJson<JsonRpcResponse<T>>(requestRes);

                    if (res.Result != null)
                    {
                        result.Result = res.Result;
                    }
                    else
                    {
                        var errorRes = JsonUtility.FromJson<JsonRpcErrorResponse>(requestRes);
                        if (errorRes != null && errorRes.Error != null)
                        {
                            result.Reason = errorRes.Error.Message;
                            result.ServerErrorCode = errorRes.Error.Code;
                        }
                        else
                        {
                            result.Reason = "Something wrong happened.";
                        }
                    }
                }
                catch (JsonException e)
                {
                    result.WasRequestSuccessfullyHandled = false;
                    result.Reason = "Unable to parse json.";
                }
            }
            return result;
        }
    }
}
