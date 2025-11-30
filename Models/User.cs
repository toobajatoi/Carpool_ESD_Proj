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

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Message> SentMessages { get; set; } = new List<Message>();

    public virtual ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();

    public virtual ICollection<FavoriteRide> FavoriteRides { get; set; } = new List<FavoriteRide>();

    public virtual ICollection<RideHistory> DriverHistories { get; set; } = new List<RideHistory>();

    public virtual ICollection<RideHistory> PassengerHistories { get; set; } = new List<RideHistory>();

    public virtual ICollection<Cancellation> Cancellations { get; set; } = new List<Cancellation>();

    public virtual ICollection<Complaint> ReportedComplaints { get; set; } = new List<Complaint>();

    public virtual ICollection<Complaint> ComplaintsAgainst { get; set; } = new List<Complaint>();

    public virtual ICollection<EmergencyContact> EmergencyContacts { get; set; } = new List<EmergencyContact>();

    public virtual ICollection<Rating> GivenRatings { get; set; } = new List<Rating>();

    public virtual ICollection<Rating> ReceivedRatings { get; set; } = new List<Rating>();

    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    public virtual ICollection<Waitlist> Waitlists { get; set; } = new List<Waitlist>();
}
