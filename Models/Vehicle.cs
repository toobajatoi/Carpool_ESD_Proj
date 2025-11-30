using System;
using System.Collections.Generic;

namespace Carpool_DB_Proj.Models;

public partial class Vehicle
{
    public int VehicleId { get; set; }

    public int UserId { get; set; }

    public string Make { get; set; } = null!;

    public string Model { get; set; } = null!;

    public int Year { get; set; }

    public string LicensePlate { get; set; } = null!;

    public string? Color { get; set; }

    public int Capacity { get; set; }

    public string? RegistrationNumber { get; set; }

    public string? InsuranceNumber { get; set; }

    public string? VehicleType { get; set; }

    public DateTime CreatedDate { get; set; }

    public bool IsActive { get; set; } = true;

    public virtual User User { get; set; } = null!;
}
