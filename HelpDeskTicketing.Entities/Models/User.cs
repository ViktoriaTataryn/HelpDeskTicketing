namespace HelpDeskTicketing.Entities.Models;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public Role Role { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public ICollection<Ticket> CreatedTickets { get; set; } //створенні користувачем
    public ICollection<Ticket> AssignedTickets { get; set; } // призначені адміну
}