using System;
using System.Collections.Generic;

namespace Carpool_DB_Proj.Models;

public partial class Route
{
    public int RouteId { get; set; }

    public int RideId { get; set; }

    public string StartLocation { get; set; } = null!;

    public string EndLocation { get; set; } = null!;

    public string? Waypoints { get; set; } // JSON string for waypoints

    public decimal? Distance { get; set; } // in kilometers

    public int? EstimatedDuration { get; set; } // in minutes

    public string? RoutePolyline { get; set; } // For map rendering

    public decimal? StartLatitude { get; set; }

    public decimal? StartLongitude { get; set; }

    public decimal? EndLatitude { get; set; }

    public decimal? EndLongitude { get; set; }

    public virtual RideDetail Ride { get; set; } = null!;
}
