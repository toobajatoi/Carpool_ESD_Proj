using System;
using System.Collections.Generic;

namespace Carpool_DB_Proj.Models;

public partial class Rating
{
    public int RatingId { get; set; }

    public int RideId { get; set; }

    public int RatedBy { get; set; }

    public int RatedUser { get; set; }

    public int RatingValue { get; set; } // 1-5 stars

    public string Category { get; set; } = null!; // Punctuality, Safety, Cleanliness, Communication, Overall

    public DateTime CreatedDate { get; set; }

    public virtual RideDetail Ride { get; set; } = null!;

    public virtual User RatedByUser { get; set; } = null!;

    public virtual User RatedUserNavigation { get; set; } = null!;
}
