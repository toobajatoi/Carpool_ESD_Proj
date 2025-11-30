using System;
using System.Collections.Generic;

namespace Carpool_DB_Proj.Models;

public partial class FavoriteRide
{
    public int FavoriteId { get; set; }

    public int UserId { get; set; }

    public int RideId { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual RideDetail Ride { get; set; } = null!;
}
