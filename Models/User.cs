using System;
using System.Collections.Generic;

namespace Carpool_DB_Proj.Models;

public partial class User
{
    public int UserId { get; set; }

    public string NuId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<LoginSession> LoginSessions { get; set; } = new List<LoginSession>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Review> ReviewDrivers { get; set; } = new List<Review>();

    public virtual ICollection<Review> ReviewUsers { get; set; } = new List<Review>();

    public virtual ICollection<RideDetail> RideDetails { get; set; } = new List<RideDetail>();

    public virtual ICollection<RideRequest> RideRequestDrivers { get; set; } = new List<RideRequest>();

    public virtual ICollection<RideRequest> RideRequestRequesters { get; set; } = new List<RideRequest>();

    public virtual ICollection<RideDetail> Rides { get; set; } = new List<RideDetail>();

    public virtual ICollection<DriverProfile> DriverProfiles { get; set; } = new List<DriverProfile>();
}
