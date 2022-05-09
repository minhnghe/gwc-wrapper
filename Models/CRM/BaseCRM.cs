namespace GreateRewardsService.Models
{
    public class BaseCRM
    {
        public BaseCRM(PostDataModel model)
        {
            Command = model.Command;
            EnquiryCode = model.EnquiryCode;
            OutletCode = model.OutletCode;
            PosID = model.PosID;
            CashierID = model.CashierID;
            IgnoreCCNchecking = model.IgnoreCCNchecking;
        }

        public BaseCRM()
        {

        }
        public string Command { get; set; }
        public string EnquiryCode { get; set; }
        public string OutletCode { get; set; }
        public string PosID { get; set; }
        public string CashierID { get; set; }
        public bool IgnoreCCNchecking { get; set; }
    }
}
