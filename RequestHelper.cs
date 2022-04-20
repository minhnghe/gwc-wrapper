﻿using GreateRewardsService.Models.RequestModels;
using GreateRewardsService.Models.ResponseModels;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GreateRewardsService
{
    public static class RequestHelper<T> where T : class
    {
        public static async Task<object> Post(T model, string uri, bool isNeedAppName = false, bool isVendorRequest = true, string accessToken = "")
        {
            HttpRequestMessage msg = new HttpRequestMessage { Method = HttpMethod.Post, RequestUri = GetUri(uri) };
            HttpClient client = new HttpClient();
            try
            {
                if (model != null)
                {
                    msg.Content = JsonContent.Create(model);
                }

                if (!string.IsNullOrEmpty(accessToken))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }
                else
                {
                    await CheckAndRenewAccessToken(isVendorRequest);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", isVendorRequest ? AuthenticationInfo.Instance.VendorAccessToken : AuthenticationInfo.Instance.MemberAccessToken);
                }

                client.DefaultRequestHeaders.Add("Asc.PosID", ConfigurationManager.AppSettings[Constants.AppSettingKeys.MMS_AscPosID]);
                if (isNeedAppName)
                {
                    client.DefaultRequestHeaders.Add("Asc.AppName", ConfigurationManager.AppSettings[Constants.AppSettingKeys.MMS_AppName]);
                }

                HttpResponseMessage res = await client.SendAsync(msg);
                string responseBody = await res.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<object>(responseBody);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<object> PostCarPark(T model, string uri)
        {
            HttpRequestMessage msg = new HttpRequestMessage { Method = HttpMethod.Post, RequestUri = new Uri(uri) };
            HttpClient client = new HttpClient();
            try
            {
                if (model != null)
                {
                    msg.Content = JsonContent.Create(model);
                }

                HttpResponseMessage res = await client.SendAsync(msg);
                string responseBody = await res.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<object>(responseBody);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<object> Put(T model, string uri, bool isNeedAppName = false, bool isVendorRequest = true)
        {
            HttpRequestMessage msg = new HttpRequestMessage { Method = HttpMethod.Put, RequestUri = GetUri(uri) };
            HttpClient client = new HttpClient();
            await CheckAndRenewAccessToken(isVendorRequest);
            try
            {
                if (model != null)
                {
                    msg.Content = JsonContent.Create(model);
                }

                client.DefaultRequestHeaders.Add("Asc.PosID", ConfigurationManager.AppSettings[Constants.AppSettingKeys.MMS_AscPosID]);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", isVendorRequest ? AuthenticationInfo.Instance.VendorAccessToken : AuthenticationInfo.Instance.MemberAccessToken);
                if (isNeedAppName)
                {
                    client.DefaultRequestHeaders.Add("Asc.AppName", ConfigurationManager.AppSettings[Constants.AppSettingKeys.MMS_AppName]);
                }

                HttpResponseMessage res = await client.SendAsync(msg);
                string responseBody = await res.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<object>(responseBody);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<object> Get(string uri, bool isNeedAppName = false, bool isVendorRequest = true, string accessToken = "")
        {
            HttpRequestMessage msg = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = GetUri(uri)
            };
            HttpClient client = new HttpClient();
            try
            {
                if (!string.IsNullOrEmpty(accessToken))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }
                else
                {
                    await CheckAndRenewAccessToken(isVendorRequest);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", isVendorRequest ? AuthenticationInfo.Instance.VendorAccessToken : AuthenticationInfo.Instance.MemberAccessToken);
                }
                client.DefaultRequestHeaders.Add("Asc.PosID", ConfigurationManager.AppSettings[Constants.AppSettingKeys.MMS_AscPosID]);
                if (isNeedAppName)
                {
                    client.DefaultRequestHeaders.Add("Asc.AppName", ConfigurationManager.AppSettings[Constants.AppSettingKeys.MMS_AppName]);
                }
                HttpResponseMessage res = await client.SendAsync(msg);
                string responseBody = await res.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<object>(responseBody);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<object> Delete(string uri, bool isNeedAppName = false, bool isVendorRequest = true)
        {
            HttpRequestMessage msg = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = GetUri(uri)
            };
            HttpClient client = new HttpClient();
            await CheckAndRenewAccessToken(isVendorRequest);
            try
            {
                client.DefaultRequestHeaders.Add("Asc.PosID", ConfigurationManager.AppSettings[Constants.AppSettingKeys.MMS_AscPosID]);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", isVendorRequest ? AuthenticationInfo.Instance.VendorAccessToken : AuthenticationInfo.Instance.MemberAccessToken);
                if (isNeedAppName)
                {
                    client.DefaultRequestHeaders.Add("Asc.AppName", ConfigurationManager.AppSettings[Constants.AppSettingKeys.MMS_AppName]);
                }
                HttpResponseMessage res = await client.SendAsync(msg);
                string responseBody = await res.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<object>(responseBody);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static Uri GetUri(string target)
        {
            return new Uri(ConfigurationManager.AppSettings[Constants.AppSettingKeys.MMS_InstanceURL] + "/" + target);
        }

        private static async Task CheckAndRenewAccessToken(bool isVendor)
        {
            HttpRequestMessage msg = new HttpRequestMessage
            {
                Method = HttpMethod.Post,

            };
            HttpClient client = new HttpClient();

            try
            {
                if (isVendor)
                {
                    msg.RequestUri = GetUri(Constants.Urls.Vendor.Authenticate);

                }
                else
                {
                    msg.RequestUri = GetUri(Constants.Urls.Member.Authenticate);
                }

                VendorAuthenticateRequestModel authInfo = new VendorAuthenticateRequestModel(isVendor);
                msg.Content = JsonContent.Create(authInfo);

                client.DefaultRequestHeaders.Add("Asc.PosID", ConfigurationManager.AppSettings[Constants.AppSettingKeys.MMS_AscPosID]);
                HttpResponseMessage res = await client.SendAsync(msg);
                string responseBody = await res.Content.ReadAsStringAsync();

                BaseResponseModel<VendorAuthenticateResponseModel> result = JsonConvert.DeserializeObject<BaseResponseModel<VendorAuthenticateResponseModel>>(responseBody);
                if (isVendor)
                {
                    AuthenticationInfo.Instance.VendorAccessToken = result.Result.AccessToken;
                    AuthenticationInfo.Instance.VendorExpiryTime = DateTime.Now.AddSeconds(result.Result.ExpireInSeconds);
                }
                else
                {
                    AuthenticationInfo.Instance.MemberAccessToken = result.Result.AccessToken;
                    AuthenticationInfo.Instance.MemberExpiryTime = DateTime.Now.AddSeconds(result.Result.ExpireInSeconds);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}