using System;
using System.Collections.Generic;

namespace Carpool_DB_Proj.Models;

public partial class RideRequest
{
    public int RequestId { get; set; }

    public int RideId { get; set; }

    public int DriverId { get; set; }

    public int RequesterId { get; set; }

    public int LocationId { get; set; }

    public string Status { get; set; } = null!;

    public virtual User Driver { get; set; } = null!;

    public virtual LocationFilter Location { get; set; } = null!;

    public virtual User Requester { get; set; } = null!;

    public virtual RideDetail Ride { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual Cancellation? Cancellation { get; set; }

    public virtual ICollection<RideHistory> RideHistories { get; set; } = new List<RideHistory>();
}
