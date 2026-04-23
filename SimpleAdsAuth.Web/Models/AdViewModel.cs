using SimpleAdsAuth.Data;

namespace SimpleAdsAuth.Web.Models
{
    public class AdsViewModel
    {
        public List<Ad> Ads { get; set; } = new List<Ad>();
        public int UserId { get; set; }
    }
}
