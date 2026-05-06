using HelpDeskTicketing.Entities.Models;

namespace HelpDeskTicketing.Core.DTOs;

public class CreateTicketDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Resolution { get; set; }
    public List<CreateCommentsDto> Comments { get; set; }
    
}