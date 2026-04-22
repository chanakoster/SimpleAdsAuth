using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdsAuth.Data
{
    public class AdsViewModel
    {
        public List<Ad> Ads { get; set; } = new List<Ad>();
        public int UserId { get; set; }
    }
}
