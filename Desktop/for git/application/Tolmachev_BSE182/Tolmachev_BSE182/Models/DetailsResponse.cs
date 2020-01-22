

namespace Tolmachev_BSE182.Models
{
    public class DetailsResponse
    {
        public string name { get; set; }
        public Location location { get; set; }
        public string average_cost_for_two { get; set; }
        public string currency { get; set; }
        public string featured_image { get; set; }
        public string menu_url { get; set; }
        public UserRating user_rating { get; set; }
        public string has_online_delivery { get; set; }
        public string has_table_booking { get; set; }
        public string cuisines { get; set; }
        public string all_reviews_count { get; set; }
        public string phone_numbers { get; set; }
    }
}