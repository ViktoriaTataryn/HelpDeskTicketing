using HelpDeskTicketing.Core.DTOs;
using HelpDeskTicketing.Entities.Models;

namespace HelpDeskTicketing.Core.Interfaces;

public interface ITicketService
{
    Task<TicketDto>AddTicketAsync(CreateTicketDto createTicketDto, CancellationToken cancellationToken = default); //user
    Task<IEnumerable<TicketDto>> GetAllTicketAsync(int page, int pageSize, CancellationToken cancellationToken = default); //admin
    Task<IEnumerable<TicketDto>>GetUserTicketsAsync( CancellationToken cancellationToken = default);
    Task<TicketDto> AssignTicketAsync(int ticketId, int adminId,CancellationToken cancellationToken = default);
    Task<TicketDto>UpdateTicketStatusAsync(int ticketId,Status status, CancellationToken cancellationToken = default); //admin
    Task DeleteTicketByIdAsync(int ticketId, CancellationToken cancellationToken = default);//admin

}