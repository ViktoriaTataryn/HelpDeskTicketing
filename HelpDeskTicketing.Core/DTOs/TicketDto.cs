using HelpDeskTicketing.Entities.Models;

namespace HelpDeskTicketing.Core.DTOs;

public class TicketDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Status Status { get; set; }
    public int? AssignedTo { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? Resolution { get; set; }
    
}