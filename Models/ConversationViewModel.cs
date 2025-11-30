namespace Carpool_DB_Proj.Models;

public class ConversationViewModel
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public Message LastMessage { get; set; } = null!;
    public int UnreadCount { get; set; }
}

