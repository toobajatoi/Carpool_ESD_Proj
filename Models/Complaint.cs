using System;
using System.Collections.Generic;

namespace Carpool_DB_Proj.Models;

public partial class Complaint
{
    public int ComplaintId { get; set; }

    public int ReporterId { get; set; }

    public int ReportedUserId { get; set; }

    public int? RideId { get; set; }

    public string ComplaintType { get; set; } = null!; // Harassment, Safety, Payment, Other

    public string Description { get; set; } = null!;

    public string Status { get; set; } = "Pending"; // Pending, UnderReview, Resolved, Dismissed

    public DateTime CreatedDate { get; set; }

    public DateTime? ResolvedDate { get; set; }

    public string? ResolutionNotes { get; set; }

    public virtual User Reporter { get; set; } = null!;

    public virtual User ReportedUser { get; set; } = null!;

    public virtual RideDetail? Ride { get; set; }
}
