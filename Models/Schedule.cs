using System;
using System.Collections.Generic;

namespace Carpool_DB_Proj.Models;

public partial class Schedule
{
    public int ScheduleId { get; set; }

    public int UserId { get; set; }

    public string DayOfWeek { get; set; } = null!; // Monday, Tuesday, etc.

    public TimeOnly Time { get; set; }

    public string Route { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? ToFast { get; set; }

    public decimal? Fare { get; set; }

    public int? AvailableSeats { get; set; }

    public virtual User User { get; set; } = null!;
}
