namespace Carpool_DB_Proj.Models
{
    public class UserRideDetailViewModel
    {
        public IEnumerable<User>? myUsers { get; set; }
        public IEnumerable<RideDetail>? myRideDetails { get; set; }
        public IEnumerable<LocationFilter>? myLocationFilter { get; set; }

        public IEnumerable<RideRequest>? myRideRequest { get; set; }
        public string? SearchString { get; set; }
        public string? toFast { get; set; }

    }
}
