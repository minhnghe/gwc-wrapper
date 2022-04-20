using GreateRewardsService.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace GreateRewardsService.Controllers
{
    public class CarParkController : ApiController
    {
        [HttpPost]
        [Route(Constants.Urls.CarPark.AddTicket)]
        public async Task<object> AddTicket(AddTicketRequestModel model)
        {
            string url = ConfigurationManager.AppSettings[Constants.AppSettingKeys.CarPark_EndPointPrefix] + Constants.Urls.CarPark.AddTicket + ".php";
            return await RequestHelper<AddTicketRequestModel>.PostCarPark(model, url);
        }

        [HttpPost]
        [Route(Constants.Urls.CarPark.GetTicket)]
        public async Task<object> GetTicket(CheckTicketRequestModel model)
        {
            string url = ConfigurationManager.AppSettings[Constants.AppSettingKeys.CarPark_EndPointPrefix] + Constants.Urls.CarPark.GetTicket + ".php";
            return await RequestHelper<CheckTicketRequestModel>.PostCarPark(model, url);
        }
    }
}