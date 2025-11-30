using System;
using System.Collections.Generic;

namespace Carpool_DB_Proj.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int UserId { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string? BankAcc { get; set; }

    public string? AccNumber { get; set; }

    public virtual User User { get; set; } = null!;
}
