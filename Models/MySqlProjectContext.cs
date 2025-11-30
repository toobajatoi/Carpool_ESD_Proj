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

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
