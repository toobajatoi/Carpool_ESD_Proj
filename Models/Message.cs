using System;
using System.Collections.Generic;

namespace Carpool_DB_Proj.Models;

public partial class Message
{
    public int MessageId { get; set; }

    public int SenderId { get; set; }

    public int ReceiverId { get; set; }

    public int? RideRequestId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime SentDate { get; set; }

    public bool IsRead { get; set; }

    public virtual User Sender { get; set; } = null!;

    public virtual User Receiver { get; set; } = null!;

    public virtual RideRequest? RideRequest { get; set; }
}
