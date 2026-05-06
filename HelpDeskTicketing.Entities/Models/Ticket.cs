namespace HelpDeskTicketing.Entities.Models;

public class Ticket
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Status Status { get; set; }
    public int? AssignedTo { get; set; }
    public User Admin { get; set; }
    public int CreatedBy { get; set; }
    public User User { get; set; }
    public DateTime CreatedDate { get; set; }
    public string Resolution { get; set; }
    public ICollection<Comment> Comments { get; set; }
}