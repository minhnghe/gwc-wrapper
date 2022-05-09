using GreateRewardsService.Models;
using GreateRewardsService.Models.RequestModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.IO;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GreateRewardsService.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            ViewBag.Title = "Payment";
            decimal amount = Convert.ToDecimal(Request.QueryString["amount"]);
            string type = Convert.ToString(Request.QueryString["type"]);
            MemoryCache cache = MemoryCache.Default;
            if (amount <= 0)
            {
                if (Request.Form.Count > 0)
                {
                    string message = HttpUtility.UrlDecode(Request.Form[0]);
                    TxnRes txnRes = JsonConvert.DeserializeObject<TxnRes>(message);
                    if (type == "vouchers" && txnRes.Msg.NetsTxnStatus == "0")
                    {
                        IssueDigitalVoucherRequestModel model = cache.Get(txnRes.Msg.B2sTxnEndURLParam) as IssueDigitalVoucherRequestModel;
                        object res = await RequestHelper<IssueDigitalVoucherRequestModel>.Post(model, Constants.Urls.Vendor.IssueDigitalVoucher);
                        JObject obj = JObject.Parse(res.ToString());
                        ViewBag.Status = Convert.ToBoolean(obj["success"]) ? "Success" : "Failed";
                    }

                    return View();
                }
                return View();
            }
            else
            {
                if (type == "vouchers")
                {
                    if (!string.IsNullOrEmpty(Request.QueryString["key"]))
                    {
                        string domain = ConfigurationManager.AppSettings[Constants.AppSettingKeys.ENETS_Domain];
                        ViewBag.Domain = domain;
                        string cacheKey = Request.QueryString["key"];
                        PrepareTxnReq(amount, cacheKey, type);
                    }
                }
                return View();

            }
        }

        private void PrepareTxnReq(decimal amount, string returnUrlParams, string type)
        {
            Enets trans = new Enets
            {
                UMID = ConfigurationManager.AppSettings[Constants.AppSettingKeys.ENETS_UMID],
                SecretKey = ConfigurationManager.AppSettings[Constants.AppSettingKeys.ENETS_Secret],
                KeyID = ConfigurationManager.AppSettings[Constants.AppSettingKeys.ENETS_KeyID],
                Amount = amount,
                MerchantReference = DateTime.Now.ToString("yyyyMMddhhmmssFFF")
            };

            trans.Currency = ConfigurationManager.AppSettings[Constants.AppSettingKeys.ENETS_Currency];
            trans.TimeZone = ConfigurationManager.AppSettings[Constants.AppSettingKeys.ENETS_TimeZone];
            trans.PaymentMode = ConfigurationManager.AppSettings[Constants.AppSettingKeys.ENETS_PaymentMode];
            trans.ReturnURL = ConfigurationManager.AppSettings[Constants.AppSettingKeys.ENETS_ReturnURL] + "?type=" + type;
            trans.NotifyURL = ConfigurationManager.AppSettings[Constants.AppSettingKeys.ENETS_NotifyURL];
            trans.TransactionDate = DateTime.Now.ToString("yyyyMMdd HH:mm:ss.FFF");
            trans.Environment = ConfigurationManager.AppSettings[Constants.AppSettingKeys.ENETS_Environment];
            trans.TID = "";
            trans.ReturnURLParam = returnUrlParams;

            string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = Request.ServerVariables["REMOTE_ADDR"];
            }

            trans.IPAddress = ip;
            string strpayload = trans.GetPayload(trans.GetTransactionRequest());

            string strhmac = trans.GetHmac(strpayload);
            ViewBag.TxnReq = strpayload;
            ViewBag.Hmac = strhmac;
            ViewBag.KeyId = trans.KeyID;

        }

        protected virtual string ToHtml(string viewToRender, ViewDataDictionary viewData)
        {
            ControllerContext controllerContext = ControllerContext;
            ViewEngineResult result = ViewEngines.Engines.FindView(controllerContext, viewToRender, null);

            StringWriter output;
            using (output = new StringWriter())
            {
                ViewContext viewContext = new ViewContext(controllerContext, result.View, viewData, controllerContext.Controller.TempData, output);
                result.View.Render(viewContext, output);
                result.ViewEngine.ReleaseView(controllerContext, result.View);
            }

            return output.ToString();
        }
    }
}
