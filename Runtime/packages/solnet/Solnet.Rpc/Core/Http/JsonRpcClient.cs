using Newtonsoft.Json;
using Solnet.Rpc.Messages;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Solnet.Rpc.Core.Http
{
    public abstract class JsonRpcClient
    {        
        private readonly HttpClient _httpClient;
        private string url;
        protected JsonRpcClient(string url, HttpClient httpClient = default)
        {
            this.url = url;
            _httpClient = httpClient ?? new HttpClient { BaseAddress = new Uri(url) };
        }

        protected async Task<RequestResult<T>> SendRequest<T>(JsonRpcRequest req)
        {
            var requestJson = JsonConvert.SerializeObject(req);

            //UnityEngine.Debug.Log($"\tRequest: {requestJson}");
            HttpResponseMessage response = null;
            RequestResult<T> result = null;


            using (var request = new UnityWebRequest(url, "POST"))
            {
                byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(requestJson);
                request.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                //Send the request then wait here until it returns
                request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError)
                {
                    UnityEngine.Debug.Log("Error While Sending: " + request.error);
                    //result = new RequestResult<T>(400, request.error.ToString());
                }
                else
                {
                    while (!request.isDone)
                    {
                        //UnityEngine.Debug.Log("Delay");
                        await Task.Yield();
                    }

                    var res = JsonConvert.DeserializeObject<JsonRpcResponse<T>>(request.downloadHandler.text);
                    result = new RequestResult<T>(response);
                    if (res.Result != null)
                    {
                        result.Result = res.Result;
                    }
                    else
                    {
                        var errorRes = JsonUtility.FromJson<JsonRpcErrorResponse>("");
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
            }
           


            //try
            //{
            //    response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, "/") { Content = new StringContent(requestJson, Encoding.UTF8, "application/json") });
            //}
            //catch (HttpRequestException e)
            //{
            //    result = new RequestResult<T>(System.Net.HttpStatusCode.BadRequest, e.Message);
            //}
            //catch (Exception e)
            //{
            //    result = new RequestResult<T>(System.Net.HttpStatusCode.BadRequest, e.Message);
            //}

            //result = new RequestResult<T>(response);
            //if (result.WasHttpRequestSuccessful)
            //{
            //    try
            //    {
            //        var requestRes = await response.Content.ReadAsStringAsync();

            //        //#TODO: replace with proper logging
            //        //Console.WriteLine($"\tResult: {requestRes}");
            //        //UnityEngine.Debug.Log(requestRes);
            //        var res = JsonConvert.DeserializeObject<JsonRpcResponse<T>>(requestRes);//JsonUtility.FromJson<JsonRpcResponse<T>>(requestRes);

            //        if (res.Result != null)
            //        {
            //            result.Result = res.Result;
            //        }
            //        else
            //        {
            //            var errorRes = JsonUtility.FromJson<JsonRpcErrorResponse>(requestRes);
            //            if (errorRes != null && errorRes.Error != null)
            //            {
            //                result.Reason = errorRes.Error.Message;
            //                result.ServerErrorCode = errorRes.Error.Code;
            //            }
            //            else
            //            {
            //                result.Reason = "Something wrong happened.";
            //            }
            //        }
            //    }
            //    catch (JsonException e)
            //    {
            //        result.WasRequestSuccessfullyHandled = false;
            //        result.Reason = "Unable to parse json.";
            //    }
            //}
            return result;
        }
    }
}
