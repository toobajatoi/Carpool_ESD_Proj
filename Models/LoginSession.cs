using System;
using System.Collections.Generic;

namespace Carpool_DB_Proj.Models;

public partial class LoginSession
{
    public int SessionId { get; set; }

    public int UserId { get; set; }

    public DateTime LoginTime { get; set; }

    public DateTime? LogoutTime { get; set; }

    public string Status { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
