using System;
using System.Collections.Generic;

namespace Carpool_DB_Proj.Models;

public partial class RideDetail
{
    public int RideId { get; set; }

    public int UserId { get; set; }

    public int CarId { get; set; }

    public string ToFast { get; set; } = null!;

    public string Routes { get; set; } = null!;

    public decimal Fare { get; set; }

    public int AvailableSeats { get; set; }

    public TimeOnly ClassTime { get; set; }

    public TimeOnly LeavingTime { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<RideRequest> RideRequests { get; set; } = new List<RideRequest>();

    public virtual User User { get; set; } = null!;

    public virtual ICollection<User> Drivers { get; set; } = new List<User>();

    public virtual ICollection<DriverProfile> DriverProfiles { get; set; } = new List<DriverProfile>();

    public virtual ICollection<FavoriteRide> FavoriteRides { get; set; } = new List<FavoriteRide>();

    public virtual ICollection<RideHistory> RideHistories { get; set; } = new List<RideHistory>();

    public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();

    public virtual Route? Route { get; set; }

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual ICollection<Waitlist> Waitlists { get; set; } = new List<Waitlist>();
}
