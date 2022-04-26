using GreateRewardsService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace GreateRewardsService.Controllers
{
    public class PaymentController : ApiController
    {
        [System.Web.Http.HttpPost]
        public async Task<object> SendTxnReq()
        {
            try
            {
                RestClient client = new RestClient("https://uat-api.nets.com.sg/GW2/TxnReqListener");
                RestRequest request = new RestRequest
                {
                    Method = Method.Post
                };
                string reqString = Constants.ENets.TxnReq;
                TxnReqModel model = new TxnReqModel
                {
                    CardHolderName = "NGHE NGOC MINH",
                    CurrencyCode = "SGD",
                    CVV = "132",
                    ExpiryDate = "1234",
                    IpAddress = "127.0.0.1",
                    TimeZone = "+8:00",
                    MerchantTxnDtm = DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff"),
                    MerchantTxnRef = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                    ENetUMID = "MID_877772003",
                    CardNumber = "4111111111111111",
                    Tid = "127.0.0.1",
                    B2sTxnEndURL = "",
                    S2sTxnEndURL = "",
                    Amount = "500"
                };
                reqString = reqString.Replace(Constants.ENets.ReplaceKey.TxnReq.CardHolderName, model.CardHolderName);
                reqString = reqString.Replace(Constants.ENets.ReplaceKey.TxnReq.CurrencyCode, model.CurrencyCode);
                reqString = reqString.Replace(Constants.ENets.ReplaceKey.TxnReq.CVV, model.CVV);
                reqString = reqString.Replace(Constants.ENets.ReplaceKey.TxnReq.ExpiryDate, model.ExpiryDate);
                reqString = reqString.Replace(Constants.ENets.ReplaceKey.TxnReq.IpAddress, model.IpAddress);
                reqString = reqString.Replace(Constants.ENets.ReplaceKey.TxnReq.TimeZone, model.TimeZone);
                reqString = reqString.Replace(Constants.ENets.ReplaceKey.TxnReq.MerchantTxnDtm, model.MerchantTxnDtm);
                reqString = reqString.Replace(Constants.ENets.ReplaceKey.TxnReq.MerchantTxnRef, model.MerchantTxnRef);
                reqString = reqString.Replace(Constants.ENets.ReplaceKey.TxnReq.ENetUMID, model.ENetUMID);
                reqString = reqString.Replace(Constants.ENets.ReplaceKey.TxnReq.CardNumber, model.CardNumber);
                reqString = reqString.Replace(Constants.ENets.ReplaceKey.TxnReq.Tid, model.Tid);
                reqString = reqString.Replace(Constants.ENets.ReplaceKey.TxnReq.Amount, model.Amount);
                reqString = reqString.Replace(Constants.ENets.ReplaceKey.TxnReq.B2sTxnEndURL, model.B2sTxnEndURL);
                reqString = reqString.Replace(Constants.ENets.ReplaceKey.TxnReq.S2sTxnEndURL, model.S2sTxnEndURL);
                string concatPayloadAndSecretKey = reqString + ConfigurationManager.AppSettings[Constants.AppSettingKeys.ENETS_Secret];
                byte[] hashed = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(concatPayloadAndSecretKey));
                string hmac = Convert.ToBase64String(hashed);

                request.AddHeader("keyId", ConfigurationManager.AppSettings[Constants.AppSettingKeys.ENETS_KeyID]);
                request.AddHeader("hmac", hmac);
                request.AddHeader("Content-Type", "application/json");
                string body = reqString;
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                RestResponse response = await client.ExecuteAsync(request);
                Console.WriteLine(response.Content);
                return JsonConvert.DeserializeObject<object>(response.Content);



                //HttpRequestMessage txnReq = new HttpRequestMessage()
                //{
                //    RequestUri = new Uri(ConfigurationManager.AppSettings[Constants.AppSettingKeys.ENETS_EndPointURL]),
                //    Method = HttpMethod.Post,
                //};
                //HttpClient client = new HttpClient();
                //string reqString = Constants.ENets.TxnReq;
                //TxnReqModel model = new TxnReqModel
                //{
                //    CardHolderName = "NGHE NGOC MINH",
                //    CurrencyCode = "SGD",
                //    CVV = "132",
                //    ExpiryDate = "1234",
                //    IpAddress = "127.0.0.1",
                //    TimeZone = "+8:00",
                //    MerchantTxnDtm = DateTime.Now.ToString("yyyyMMdd HH:mm:ss.fff"),
                //    MerchantTxnRef = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                //    ENetUMID = "MID_877772003",
                //    CardNumber = "4111111111111111",
                //    Tid = "127.0.0.1",
                //    B2sTxnEndURL = "",
                //    S2sTxnEndURL = "",
                //    Amount = "500"
                //};
                //reqString = reqString.Replace(Constants.ENets.ReplaceKey.TxnReq.CardHolderName, model.CardHolderName);
                //reqString = reqString.Replace(Constants.ENets.ReplaceKey.TxnReq.CurrencyCode, model.CurrencyCode);
                //reqString = reqString.Replace(Constants.ENets.ReplaceKey.TxnReq.CVV, model.CVV);
                //reqString = reqString.Replace(Constants.ENets.ReplaceKey.TxnReq.ExpiryDate, model.ExpiryDate);
                //reqString = reqString.Replace(Constants.ENets.ReplaceKey.TxnReq.IpAddress, model.IpAddress);
                //reqString = reqString.Replace(Constants.ENets.ReplaceKey.TxnReq.TimeZone, model.TimeZone);
                //reqString = reqString.Replace(Constants.ENets.ReplaceKey.TxnReq.MerchantTxnDtm, model.MerchantTxnDtm);
                //reqString = reqString.Replace(Constants.ENets.ReplaceKey.TxnReq.MerchantTxnRef, model.MerchantTxnRef);
                //reqString = reqString.Replace(Constants.ENets.ReplaceKey.TxnReq.ENetUMID, model.ENetUMID);
                //reqString = reqString.Replace(Constants.ENets.ReplaceKey.TxnReq.CardNumber, model.CardNumber);
                //reqString = reqString.Replace(Constants.ENets.ReplaceKey.TxnReq.Tid, model.Tid);
                //reqString = reqString.Replace(Constants.ENets.ReplaceKey.TxnReq.Amount, model.Amount);
                //reqString = reqString.Replace(Constants.ENets.ReplaceKey.TxnReq.B2sTxnEndURL, model.B2sTxnEndURL);
                //reqString = reqString.Replace(Constants.ENets.ReplaceKey.TxnReq.S2sTxnEndURL, model.S2sTxnEndURL);
                //string concatPayloadAndSecretKey = reqString + ConfigurationManager.AppSettings[Constants.AppSettingKeys.ENETS_Secret];
                //byte[] hashed = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(concatPayloadAndSecretKey));
                //string hmac = Convert.ToBase64String(hashed);
                //client.DefaultRequestHeaders.Add("keyId", ConfigurationManager.AppSettings[Constants.AppSettingKeys.ENETS_KeyID]);
                //client.DefaultRequestHeaders.Add("hmac", hmac);
                //txnReq.Content = JsonContent.Create(reqString, options: new System.Text.Json.JsonSerializerOptions()
                //{
                //    WriteIndented = false,
                //});
                //HttpResponseMessage response = await client.SendAsync(txnReq);
                //string responseBody = await response.Content.ReadAsStringAsync();
                //return JsonConvert.DeserializeObject<object>(responseBody);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
                return null;
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("Endpoint")]
        public async Task GetResponse()
        {
            try
            {
                string body = await Request.Content.ReadAsStringAsync();

                if (!File.Exists("d:\\logs\\sample.txt"))
                {
                    File.Create("d:\\logs\\sample.txt");
                }
                File.AppendAllText("d:\\logs\\sample.txt", body + "\r\n");
                return;
            }
            catch (Exception)
            {

            }
        }


    }
}