namespace SimpleAdsAuth.Data
{
    public class Ad
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public User User { get; set; }
    }
}
