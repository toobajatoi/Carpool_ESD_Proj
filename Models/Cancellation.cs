using System;
using System.Collections.Generic;

namespace Carpool_DB_Proj.Models;

public partial class Cancellation
{
    public int CancellationId { get; set; }

    public int RideRequestId { get; set; }

    public int CancelledBy { get; set; }

    public string Reason { get; set; } = null!;

    public DateTime CancellationDate { get; set; }

    public string RefundStatus { get; set; } = "Pending"; // Pending, Processed, Denied

    public decimal? RefundAmount { get; set; }

    public virtual RideRequest RideRequest { get; set; } = null!;

    public virtual User CancelledByUser { get; set; } = null!;
}
