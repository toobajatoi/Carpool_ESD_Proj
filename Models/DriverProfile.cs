using System;
using System.Collections.Generic;

namespace Carpool_DB_Proj.Models;

public partial class DriverProfile
{
    public int DriverId { get; set; }

    public int RideId { get; set; }

    public virtual User Driver { get; set; } = null!;

    public virtual RideDetail RideDetail { get; set; } = null!;
}
