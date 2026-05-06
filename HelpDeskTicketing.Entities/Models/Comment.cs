using System.ComponentModel.DataAnnotations.Schema;

namespace HelpDeskTicketing.Entities.Models;

public class Comment
{
    public int Id { get; set; }
    [ForeignKey(nameof(Ticket))]
    public int TicketId { get; set; }
    public Ticket Ticket { get; set; }
    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    public User User { get; set; }
    
    public string Text { get; set; }
    public DateTime CreatedDate { get; set; }
}