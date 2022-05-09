namespace GreateRewardsService.Models.ResponseModels
{
    public class AuthResponse
    {
        public string Access_token { get; set; }
        public int Expires_in { get; set; }
        public string Token_Type { get; set; }
        public string Scope { get; set; }
    }
}