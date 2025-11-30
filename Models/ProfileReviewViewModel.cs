namespace Carpool_DB_Proj.Models
{
    public class ProfileReviewViewModel
    {
        public User? myUsers { get; set; }
        public IEnumerable<Review>? myReview { get; set; }
    }
}
