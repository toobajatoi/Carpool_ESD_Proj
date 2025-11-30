using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Carpool_DB_Proj.Models;

public partial class MySqlProjectContext : DbContext
{
    public MySqlProjectContext()
    {
    }

    public MySqlProjectContext(DbContextOptions<MySqlProjectContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DriverProfile> DriverProfiles { get; set; }

    public virtual DbSet<LocationFilter> LocationFilters { get; set; }

    public virtual DbSet<LoginSession> LoginSessions { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<RideDetail> RideDetails { get; set; }

    public virtual DbSet<RideRequest> RideRequests { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<FavoriteRide> FavoriteRides { get; set; }

    public virtual DbSet<RideHistory> RideHistories { get; set; }

    public virtual DbSet<Cancellation> Cancellations { get; set; }

    public virtual DbSet<Complaint> Complaints { get; set; }

    public virtual DbSet<Route> Routes { get; set; }

    public virtual DbSet<EmergencyContact> EmergencyContacts { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<Waitlist> Waitlists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LocationFilter>(entity =>
        {
            entity.HasKey(e => e.FilterId).HasName("PK__Location__833C443FD74936C7");

            entity.ToTable("LocationFilter");

            entity.Property(e => e.FilterId).HasColumnName("filter_id");
            entity.Property(e => e.DropLocation)
                .HasMaxLength(255)
                .HasColumnName("drop_location");
            entity.Property(e => e.PickupLocation)
                .HasMaxLength(255)
                .HasColumnName("pickup_location");
        });

        modelBuilder.Entity<LoginSession>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("PK__LoginSes__69B13FDC9984366D");

            entity.ToTable("LoginSession");

            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.LoginTime)
                .HasColumnType("datetime")
                .HasColumnName("login_time");
            entity.Property(e => e.LogoutTime)
                .HasColumnType("datetime")
                .HasColumnName("logout_time");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.LoginSessions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LoginSess__user___67DE6983");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__ED1FC9EA7B04494F");

            entity.ToTable("Payment");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.AccNumber)
                .HasMaxLength(20)
                .HasColumnName("acc_number");
            entity.Property(e => e.BankAcc)
                .HasMaxLength(50)
                .HasColumnName("bank_acc");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .HasColumnName("payment_method");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payment__user_id__12C8C788");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__60883D90BD280C4F");

            entity.Property(e => e.ReviewId).HasColumnName("review_id");
            entity.Property(e => e.DriverId).HasColumnName("driver_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.ReviewDate).HasColumnName("review_date");
            entity.Property(e => e.ReviewText).HasColumnName("review_text");
            entity.Property(e => e.RideId).HasColumnName("ride_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Driver).WithMany(p => p.ReviewDrivers)
                .HasForeignKey(d => d.DriverId)
                .HasConstraintName("FK__Reviews__driver___1A69E950");

            entity.HasOne(d => d.Ride).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.RideId)
                .HasConstraintName("FK__Reviews__ride_id__1881A0DE");

            entity.HasOne(d => d.User).WithMany(p => p.ReviewUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Reviews__user_id__1975C517");
        });

        modelBuilder.Entity<RideDetail>(entity =>
        {
            entity.HasKey(e => e.RideId).HasName("PK__RideDeta__C7E4D077FDECC34F");

            entity.HasIndex(e => e.CarId, "UQ__RideDeta__4C9A0DB2D95555D8").IsUnique();

            entity.Property(e => e.RideId).HasColumnName("ride_id");
            entity.Property(e => e.AvailableSeats).HasColumnName("available_seats");
            entity.Property(e => e.CarId).HasColumnName("car_id");
            entity.Property(e => e.ClassTime).HasColumnName("class_time");
            entity.Property(e => e.Fare)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("fare");
            entity.Property(e => e.LeavingTime).HasColumnName("leaving_time");
            entity.Property(e => e.Routes)
                .HasMaxLength(255)
                .HasColumnName("routes");
            entity.Property(e => e.ToFast)
                .HasMaxLength(10)
                .HasColumnName("to_fast");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.RideDetails)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RideDetai__user___55BFB948");
        });

        modelBuilder.Entity<RideRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__RideRequ__18D3B90FCD492E06");

            entity.ToTable("RideRequest");

            entity.Property(e => e.RequestId).HasColumnName("request_id");
            entity.Property(e => e.DriverId).HasColumnName("driver_id");
            entity.Property(e => e.LocationId).HasColumnName("location_id");
            entity.Property(e => e.RequesterId).HasColumnName("requester_id");
            entity.Property(e => e.RideId).HasColumnName("ride_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");

            entity.HasOne(d => d.Driver).WithMany(p => p.RideRequestDrivers)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RideReque__drive__7DCDAAA2");

            entity.HasOne(d => d.Location).WithMany(p => p.RideRequests)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RideReque__locat__7FB5F314");

            entity.HasOne(d => d.Requester).WithMany(p => p.RideRequestRequesters)
                .HasForeignKey(d => d.RequesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RideReque__reque__7EC1CEDB");

            entity.HasOne(d => d.Ride).WithMany(p => p.RideRequests)
                .HasForeignKey(d => d.RideId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RideReque__ride___7CD98669");
        });

        modelBuilder.Entity<DriverProfile>(entity =>
        {
            entity.HasKey(e => new { e.DriverId, e.RideId }).HasName("PK_DriverProfile");
            entity.ToTable("DriverProfile");

            entity.Property(e => e.DriverId).HasColumnName("driver_id");
            entity.Property(e => e.RideId).HasColumnName("ride_id");

            entity.HasOne(d => d.Driver).WithMany(p => p.DriverProfiles)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DriverProfile_User");

            entity.HasOne(d => d.RideDetail).WithMany(p => p.DriverProfiles)
                .HasForeignKey(d => d.RideId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DriverProfile_RideDetail");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370F45CB5362");

            entity.HasIndex(e => e.Email, "UQ__Users__AB6E616479EBE292").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.NuId)
                .HasMaxLength(10)
                .HasColumnName("nu_id");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .HasColumnName("phone_number");

        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.VehicleId).HasName("PK__Vehicle__476B54B2A1B2C3D4");

            entity.ToTable("Vehicle");

            entity.Property(e => e.VehicleId).HasColumnName("vehicle_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Make)
                .HasMaxLength(50)
                .HasColumnName("make");
            entity.Property(e => e.Model)
                .HasMaxLength(50)
                .HasColumnName("model");
            entity.Property(e => e.Year).HasColumnName("year");
            entity.Property(e => e.LicensePlate)
                .HasMaxLength(20)
                .HasColumnName("license_plate");
            entity.Property(e => e.Color)
                .HasMaxLength(30)
                .HasColumnName("color");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.RegistrationNumber)
                .HasMaxLength(50)
                .HasColumnName("registration_number");
            entity.Property(e => e.InsuranceNumber)
                .HasMaxLength(50)
                .HasColumnName("insurance_number");
            entity.Property(e => e.VehicleType)
                .HasMaxLength(30)
                .HasColumnName("vehicle_type");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.IsActive).HasColumnName("is_active");

            entity.HasOne(d => d.User).WithMany(p => p.Vehicles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Vehicle__user_id__Vehicle_User");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__NotificationId");
            entity.ToTable("Notification");
            entity.Property(e => e.NotificationId).HasColumnName("notification_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Title).HasMaxLength(200).HasColumnName("title");
            entity.Property(e => e.Message).HasMaxLength(1000).HasColumnName("message");
            entity.Property(e => e.Type).HasMaxLength(50).HasColumnName("type");
            entity.Property(e => e.IsRead).HasColumnName("is_read");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime").HasColumnName("created_date");
            entity.Property(e => e.RelatedEntityType).HasMaxLength(50).HasColumnName("related_entity_type");
            entity.Property(e => e.RelatedEntityId).HasColumnName("related_entity_id");
            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notification__user_id");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Message__MessageId");
            entity.ToTable("Message");
            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.SenderId).HasColumnName("sender_id");
            entity.Property(e => e.ReceiverId).HasColumnName("receiver_id");
            entity.Property(e => e.RideRequestId).HasColumnName("ride_request_id");
            entity.Property(e => e.Content).HasMaxLength(2000).HasColumnName("content");
            entity.Property(e => e.SentDate).HasColumnType("datetime").HasColumnName("sent_date");
            entity.Property(e => e.IsRead).HasColumnName("is_read");
            entity.HasOne(d => d.Sender).WithMany(p => p.SentMessages)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Message__sender_id");
            entity.HasOne(d => d.Receiver).WithMany(p => p.ReceivedMessages)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Message__receiver_id");
            entity.HasOne(d => d.RideRequest).WithMany(p => p.Messages)
                .HasForeignKey(d => d.RideRequestId)
                .HasConstraintName("FK__Message__ride_request_id");
        });

        modelBuilder.Entity<FavoriteRide>(entity =>
        {
            entity.HasKey(e => e.FavoriteId).HasName("PK__FavoriteRide__FavoriteId");
            entity.ToTable("FavoriteRide");
            entity.Property(e => e.FavoriteId).HasColumnName("favorite_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.RideId).HasColumnName("ride_id");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime").HasColumnName("created_date");
            entity.HasOne(d => d.User).WithMany(p => p.FavoriteRides)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FavoriteRide__user_id");
            entity.HasOne(d => d.Ride).WithMany(p => p.FavoriteRides)
                .HasForeignKey(d => d.RideId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__FavoriteRide__ride_id");
        });

        modelBuilder.Entity<RideHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__RideHistory__HistoryId");
            entity.ToTable("RideHistory");
            entity.Property(e => e.HistoryId).HasColumnName("history_id");
            entity.Property(e => e.RideId).HasColumnName("ride_id");
            entity.Property(e => e.DriverId).HasColumnName("driver_id");
            entity.Property(e => e.PassengerId).HasColumnName("passenger_id");
            entity.Property(e => e.RideRequestId).HasColumnName("ride_request_id");
            entity.Property(e => e.StartTime).HasColumnType("datetime").HasColumnName("start_time");
            entity.Property(e => e.EndTime).HasColumnType("datetime").HasColumnName("end_time");
            entity.Property(e => e.Distance).HasColumnType("decimal(10,2)").HasColumnName("distance");
            entity.Property(e => e.Status).HasMaxLength(50).HasColumnName("status");
            entity.Property(e => e.CompletedDate).HasColumnType("datetime").HasColumnName("completed_date");
            entity.HasOne(d => d.Ride).WithMany(p => p.RideHistories)
                .HasForeignKey(d => d.RideId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RideHistory__ride_id");
            entity.HasOne(d => d.Driver).WithMany(p => p.DriverHistories)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RideHistory__driver_id");
            entity.HasOne(d => d.Passenger).WithMany(p => p.PassengerHistories)
                .HasForeignKey(d => d.PassengerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RideHistory__passenger_id");
            entity.HasOne(d => d.RideRequest).WithMany(p => p.RideHistories)
                .HasForeignKey(d => d.RideRequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RideHistory__ride_request_id");
        });

        modelBuilder.Entity<Cancellation>(entity =>
        {
            entity.HasKey(e => e.CancellationId).HasName("PK__Cancellation__CancellationId");
            entity.ToTable("Cancellation");
            entity.Property(e => e.CancellationId).HasColumnName("cancellation_id");
            entity.Property(e => e.RideRequestId).HasColumnName("ride_request_id");
            entity.Property(e => e.CancelledBy).HasColumnName("cancelled_by");
            entity.Property(e => e.Reason).HasMaxLength(500).HasColumnName("reason");
            entity.Property(e => e.CancellationDate).HasColumnType("datetime").HasColumnName("cancellation_date");
            entity.Property(e => e.RefundStatus).HasMaxLength(50).HasColumnName("refund_status");
            entity.Property(e => e.RefundAmount).HasColumnType("decimal(10,2)").HasColumnName("refund_amount");
            entity.HasOne(d => d.RideRequest).WithOne(p => p.Cancellation)
                .HasForeignKey<Cancellation>(d => d.RideRequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cancellation__ride_request_id");
            entity.HasOne(d => d.CancelledByUser).WithMany(p => p.Cancellations)
                .HasForeignKey(d => d.CancelledBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cancellation__cancelled_by");
        });

        modelBuilder.Entity<Complaint>(entity =>
        {
            entity.HasKey(e => e.ComplaintId).HasName("PK__Complaint__ComplaintId");
            entity.ToTable("Complaint");
            entity.Property(e => e.ComplaintId).HasColumnName("complaint_id");
            entity.Property(e => e.ReporterId).HasColumnName("reporter_id");
            entity.Property(e => e.ReportedUserId).HasColumnName("reported_user_id");
            entity.Property(e => e.RideId).HasColumnName("ride_id");
            entity.Property(e => e.ComplaintType).HasMaxLength(50).HasColumnName("complaint_type");
            entity.Property(e => e.Description).HasMaxLength(2000).HasColumnName("description");
            entity.Property(e => e.Status).HasMaxLength(50).HasColumnName("status");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime").HasColumnName("created_date");
            entity.Property(e => e.ResolvedDate).HasColumnType("datetime").HasColumnName("resolved_date");
            entity.Property(e => e.ResolutionNotes).HasMaxLength(1000).HasColumnName("resolution_notes");
            entity.HasOne(d => d.Reporter).WithMany(p => p.ReportedComplaints)
                .HasForeignKey(d => d.ReporterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Complaint__reporter_id");
            entity.HasOne(d => d.ReportedUser).WithMany(p => p.ComplaintsAgainst)
                .HasForeignKey(d => d.ReportedUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Complaint__reported_user_id");
            entity.HasOne(d => d.Ride).WithMany(p => p.Complaints)
                .HasForeignKey(d => d.RideId)
                .HasConstraintName("FK__Complaint__ride_id");
        });

        modelBuilder.Entity<Route>(entity =>
        {
            entity.HasKey(e => e.RouteId).HasName("PK__Route__RouteId");
            entity.ToTable("Route");
            entity.Property(e => e.RouteId).HasColumnName("route_id");
            entity.Property(e => e.RideId).HasColumnName("ride_id");
            entity.Property(e => e.StartLocation).HasMaxLength(255).HasColumnName("start_location");
            entity.Property(e => e.EndLocation).HasMaxLength(255).HasColumnName("end_location");
            entity.Property(e => e.Waypoints).HasMaxLength(2000).HasColumnName("waypoints");
            entity.Property(e => e.Distance).HasColumnType("decimal(10,2)").HasColumnName("distance");
            entity.Property(e => e.EstimatedDuration).HasColumnName("estimated_duration");
            entity.Property(e => e.RoutePolyline).HasMaxLength(5000).HasColumnName("route_polyline");
            entity.Property(e => e.StartLatitude).HasColumnType("decimal(10,8)").HasColumnName("start_latitude");
            entity.Property(e => e.StartLongitude).HasColumnType("decimal(11,8)").HasColumnName("start_longitude");
            entity.Property(e => e.EndLatitude).HasColumnType("decimal(10,8)").HasColumnName("end_latitude");
            entity.Property(e => e.EndLongitude).HasColumnType("decimal(11,8)").HasColumnName("end_longitude");
            entity.HasOne(d => d.Ride).WithOne(p => p.Route)
                .HasForeignKey<Route>(d => d.RideId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Route__ride_id");
        });

        modelBuilder.Entity<EmergencyContact>(entity =>
        {
            entity.HasKey(e => e.ContactId).HasName("PK__EmergencyContact__ContactId");
            entity.ToTable("EmergencyContact");
            entity.Property(e => e.ContactId).HasColumnName("contact_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.ContactName).HasMaxLength(100).HasColumnName("contact_name");
            entity.Property(e => e.ContactPhone).HasMaxLength(20).HasColumnName("contact_phone");
            entity.Property(e => e.Relationship).HasMaxLength(50).HasColumnName("relationship");
            entity.Property(e => e.IsPrimary).HasColumnName("is_primary");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime").HasColumnName("created_date");
            entity.HasOne(d => d.User).WithMany(p => p.EmergencyContacts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__EmergencyContact__user_id");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.RatingId).HasName("PK__Rating__RatingId");
            entity.ToTable("Rating");
            entity.Property(e => e.RatingId).HasColumnName("rating_id");
            entity.Property(e => e.RideId).HasColumnName("ride_id");
            entity.Property(e => e.RatedBy).HasColumnName("rated_by");
            entity.Property(e => e.RatedUser).HasColumnName("rated_user");
            entity.Property(e => e.RatingValue).HasColumnName("rating_value");
            entity.Property(e => e.Category).HasMaxLength(50).HasColumnName("category");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime").HasColumnName("created_date");
            entity.HasOne(d => d.Ride).WithMany(p => p.Ratings)
                .HasForeignKey(d => d.RideId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Rating__ride_id");
            entity.HasOne(d => d.RatedByUser).WithMany(p => p.GivenRatings)
                .HasForeignKey(d => d.RatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Rating__rated_by");
            entity.HasOne(d => d.RatedUserNavigation).WithMany(p => p.ReceivedRatings)
                .HasForeignKey(d => d.RatedUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Rating__rated_user");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__Schedule__ScheduleId");
            entity.ToTable("Schedule");
            entity.Property(e => e.ScheduleId).HasColumnName("schedule_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.DayOfWeek).HasMaxLength(20).HasColumnName("day_of_week");
            entity.Property(e => e.Time).HasColumnName("time");
            entity.Property(e => e.Route).HasMaxLength(255).HasColumnName("route");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.StartDate).HasColumnType("datetime").HasColumnName("start_date");
            entity.Property(e => e.EndDate).HasColumnType("datetime").HasColumnName("end_date");
            entity.Property(e => e.ToFast).HasMaxLength(10).HasColumnName("to_fast");
            entity.Property(e => e.Fare).HasColumnType("decimal(10,2)").HasColumnName("fare");
            entity.Property(e => e.AvailableSeats).HasColumnName("available_seats");
            entity.HasOne(d => d.User).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Schedule__user_id");
        });

        modelBuilder.Entity<Waitlist>(entity =>
        {
            entity.HasKey(e => e.WaitlistId).HasName("PK__Waitlist__WaitlistId");
            entity.ToTable("Waitlist");
            entity.Property(e => e.WaitlistId).HasColumnName("waitlist_id");
            entity.Property(e => e.RideId).HasColumnName("ride_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Position).HasColumnName("position");
            entity.Property(e => e.Status).HasMaxLength(50).HasColumnName("status");
            entity.Property(e => e.CreatedDate).HasColumnType("datetime").HasColumnName("created_date");
            entity.Property(e => e.NotifiedDate).HasColumnType("datetime").HasColumnName("notified_date");
            entity.HasOne(d => d.Ride).WithMany(p => p.Waitlists)
                .HasForeignKey(d => d.RideId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Waitlist__ride_id");
            entity.HasOne(d => d.User).WithMany(p => p.Waitlists)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Waitlist__user_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
