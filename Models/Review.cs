using System;
using System.Collections.Generic;

namespace Carpool_DB_Proj.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int? RideId { get; set; }

    public int? UserId { get; set; }

    public int? DriverId { get; set; }

    public DateOnly ReviewDate { get; set; }

    public string? ReviewText { get; set; }

    public int Rating { get; set; }

    public virtual User? Driver { get; set; }

    public virtual RideDetail? Ride { get; set; }

    public virtual User? User { get; set; }
}
