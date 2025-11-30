using System;
using System.Collections.Generic;

namespace Carpool_DB_Proj.Models;

public partial class EmergencyContact
{
    public int ContactId { get; set; }

    public int UserId { get; set; }

    public string ContactName { get; set; } = null!;

    public string ContactPhone { get; set; } = null!;

    public string? Relationship { get; set; } // Family, Friend, Other

    public bool IsPrimary { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual User User { get; set; } = null!;
}
