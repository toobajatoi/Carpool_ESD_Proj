using System;
using System.Collections.Generic;

namespace Carpool_DB_Proj.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string Type { get; set; } = null!; // Info, Success, Warning, Error

    public bool IsRead { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? RelatedEntityType { get; set; }

    public int? RelatedEntityId { get; set; }

    public virtual User User { get; set; } = null!;
}
