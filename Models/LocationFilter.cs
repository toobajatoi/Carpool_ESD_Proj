using System;
using System.Collections.Generic;

namespace Carpool_DB_Proj.Models;

public partial class LocationFilter
{
    public int FilterId { get; set; }

    public string DropLocation { get; set; } = null!;

    public string PickupLocation { get; set; } = null!;

    public virtual ICollection<RideRequest> RideRequests { get; set; } = new List<RideRequest>();
}
