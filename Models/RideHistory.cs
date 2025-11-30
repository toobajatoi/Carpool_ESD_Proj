using System;
using System.Collections.Generic;

namespace Carpool_DB_Proj.Models;

public partial class RideHistory
{
    public int HistoryId { get; set; }

    public int RideId { get; set; }

    public int DriverId { get; set; }

    public int PassengerId { get; set; }

    public int? RideRequestId { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public decimal? Distance { get; set; }

    public string Status { get; set; } = null!; // Completed, Cancelled, InProgress

    public DateTime CompletedDate { get; set; }

    public virtual RideDetail Ride { get; set; } = null!;

    public virtual User Driver { get; set; } = null!;

    public virtual User Passenger { get; set; } = null!;

    public virtual RideRequest? RideRequest { get; set; }
}
