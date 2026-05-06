using System.Collections;
using HelpDeskTicketing.Core.DTOs;
using HelpDeskTicketing.Core.Interfaces;
using HelpDeskTicketing.Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDeskTicketing.Controllers;
[ApiController]
[Route("api/tickets")]
public class TicketController: ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TicketDto>>> GetAllTicketAsync(int page, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var ticket = await _ticketService.GetAllTicketAsync(page, pageSize, cancellationToken);
        return Ok(ticket);
    }
    
    [Authorize(Roles = "User")]
    [HttpPost]
    public async Task<ActionResult<TicketDto>> AddTicketAsync([FromBody]CreateTicketDto createTicketDto,
        CancellationToken cancellationToken = default)
    {
        var ticket = await _ticketService.AddTicketAsync(createTicketDto, cancellationToken);
        return Created($"api/tickets/{ticket.Id}", ticket);
    }
    [Authorize(Roles = "User")]
    [HttpGet("/users")]
    public async Task<ActionResult<IEnumerable<TicketDto>>> GetUserTicketsAsync(CancellationToken cancellationToken = default)
    {
        var ticket = await _ticketService.GetUserTicketsAsync(cancellationToken);
        return Ok(ticket);
    }
    [Authorize(Roles = "Admin")]
    [HttpPut("/assign")]
    public async Task<ActionResult<TicketDto>>AssignTicketAsync(int ticketId, int adminId,
        CancellationToken cancellationToken = default)
    {
        var ticket = await _ticketService.AssignTicketAsync(ticketId, adminId, cancellationToken);
        return Created($"api/tickets/{ticketId}", ticket);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<TicketDto>>UpdateTicketStatusAsync([FromRoute]int id, Status status,
        CancellationToken cancellationToken = default)
    {
        var ticket = await _ticketService.UpdateTicketStatusAsync(id, status, cancellationToken);
        return Created($"api/tickets/{id}", ticket);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTicketAsync([FromRoute]int id,
        CancellationToken cancellationToken = default)
    {
        await _ticketService.DeleteTicketByIdAsync(id, cancellationToken);
        return NoContent();
    }
}