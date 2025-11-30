using System;
using System.Collections.Generic;

namespace Carpool_DB_Proj.Models;

public partial class Waitlist
{
    public int WaitlistId { get; set; }

    public int RideId { get; set; }

    public int UserId { get; set; }

    public int Position { get; set; }

    public string Status { get; set; } = "Waiting"; // Waiting, Notified, Booked, Cancelled

    public DateTime CreatedDate { get; set; }

    public DateTime? NotifiedDate { get; set; }

    public virtual RideDetail Ride { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
