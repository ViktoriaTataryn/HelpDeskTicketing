using System.Security.Claims;
using HelpDeskTicketing.Core.DTOs;
using HelpDeskTicketing.Core.Interfaces;
using HelpDeskTicketing.Entities.Models;
using HelpDeskTicketing.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace HelpDeskTicketing.Core.Services;

public class TicketService: ITicketService
{
    private readonly HelpDeskContext _context;
    private readonly IUserService _userService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICacheService _cacheService;

    public TicketService(HelpDeskContext context, IUserService userService, IHttpContextAccessor httpContextAccessor, ICacheService cacheService)
    {
        _context = context;
        _userService = userService;
        _httpContextAccessor = httpContextAccessor;
        _cacheService = cacheService;
    }
    //user
    public async Task<TicketDto> AddTicketAsync(CreateTicketDto createTicketDto, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            throw new UnauthorizedAccessException("You are not logged in");
        }
        ValidateTicket(createTicketDto);
        var ticket = new Ticket
        {
            Title = createTicketDto.Title,
            Description = createTicketDto.Description,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = userId.Value,
            Status = Status.Open,
            Resolution = createTicketDto.Resolution,
            AssignedTo = null
            
        };
        _context.Tickets.Add(ticket);
        _cacheService.DeleteCachedData(CacheService.TICKET_KEY);
        await _context.SaveChangesAsync(cancellationToken);
        if (createTicketDto.Comments != null && createTicketDto.Comments.Any())
        {
            foreach (var commentDto in createTicketDto.Comments)
            {
                var comment = new Comment
                {
                    TicketId = ticket.Id,
                    UserId = userId.Value,  
                    Text = commentDto.Text,
                    CreatedDate = DateTime.UtcNow
                };
                _context.Comments.Add(comment);
            }
            
            await _context.SaveChangesAsync(cancellationToken);
        }
        
        return new TicketDto
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            CreatedDate = ticket.CreatedDate,
            CreatedBy = ticket.CreatedBy,
            Status = ticket.Status,
        };
    }
    
    //admin
    public async Task<IEnumerable<TicketDto>> GetAllTicketAsync(int page, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var data = await _cacheService.GetCachedDataAsync(CacheService.TICKET_KEY,
            async () => await GetAllTicketFromDBAsync(cancellationToken));
        return data.Skip(page * pageSize)
            .Take(pageSize);
    }

    private async Task<IEnumerable<TicketDto>> GetAllTicketFromDBAsync( CancellationToken cancellationToken = default)
    {
       
     return   await  _context.Tickets
         .AsNoTracking()
         .Select(x=> new TicketDto
         {
             Id = x.Id,
             Title = x.Title,
             Description = x.Description,
             Status = x.Status,
             CreatedDate = x.CreatedDate,
             CreatedBy = x.CreatedBy,
         })
         .ToArrayAsync(cancellationToken);
    }
    
    //logged-in user
    public async Task<IEnumerable<TicketDto>> GetUserTicketsAsync(CancellationToken cancellationToken = default)
    {
        var data = await _cacheService.GetCachedDataAsync(CacheService.TICKET_KEY,
            async () => await GetUserTicketsFromDBAsync(cancellationToken));
        return data;
    }

    private async Task<IEnumerable<TicketDto>> GetUserTicketsFromDBAsync(CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
       if (userId == null)
       {
           throw new UnauthorizedAccessException("You are not logged in");
       }
        return await  _context.Tickets
            .AsNoTracking()
            .Where(x=>x.CreatedBy == userId.Value)
            .Select(x=> new TicketDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Status = x.Status,
                CreatedDate = x.CreatedDate,
                CreatedBy = x.CreatedBy,
            })
            .ToArrayAsync(cancellationToken);
    }

    //logged-in admin
    public async Task<TicketDto> AssignTicketAsync(int ticketId, int adminId, CancellationToken cancellationToken = default)
    {
       var ticket = await _context.Tickets.FindAsync([ticketId], cancellationToken);
       if (ticket == null)
       {
           throw new KeyNotFoundException("Ticket not found");
       }

       if (!_userService.IsAdmin(adminId))
       {
           throw new UnauthorizedAccessException("User is not admin");
           
       }

       if (ticket.Status != Status.Open)
       {
        throw new ArgumentException("Ticket is not open");   
       }

       if (ticket.AssignedTo != null)
       {
           throw new ArgumentException("Ticket is already assigned to admin");
       }
       ticket.AssignedTo=adminId;
       _cacheService.DeleteCachedData(CacheService.TICKET_KEY);
       await _context.SaveChangesAsync(cancellationToken);
       return new TicketDto
       {
           Id = ticket.Id,
           Title = ticket.Title,
           Description = ticket.Description,
           Status = ticket.Status,
           CreatedDate = ticket.CreatedDate,
           CreatedBy = ticket.CreatedBy,
           AssignedTo = ticket.AssignedTo
       };
    }
    //admin
    public async Task<TicketDto> UpdateTicketStatusAsync(int ticketId,Status status, CancellationToken cancellationToken = default)
    {
        var ticket = await _context.Tickets.FindAsync([ticketId], cancellationToken);
        if (ticket == null)
        {
            throw new KeyNotFoundException("Ticket not found");
        }

        ticket.Status = status;
        _cacheService.DeleteCachedData(CacheService.TICKET_KEY);
        await _context.SaveChangesAsync(cancellationToken);
        return new TicketDto
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            Status = ticket.Status,
            CreatedDate = ticket.CreatedDate,
            CreatedBy = ticket.CreatedBy,
        };
    }
    //admin
    public async Task DeleteTicketByIdAsync(int ticketId, CancellationToken cancellationToken = default)
    {
      var ticket=  await _context.Tickets.FindAsync([ticketId], cancellationToken);
      if (ticket == null)
      {
          throw new KeyNotFoundException("Ticket not found");
      }
      _context.Tickets.Remove(ticket);
      _cacheService.DeleteCachedData(CacheService.TICKET_KEY);
      await _context.SaveChangesAsync(cancellationToken);
    }

    private void ValidateTicket(CreateTicketDto ticket)
    {
        if (string.IsNullOrEmpty(ticket.Title))
        {
            throw new ArgumentException("Title is required");
        }

        if (string.IsNullOrEmpty(ticket.Description))
        {
            throw new ArgumentException("Description is required");
        }
        if (string.IsNullOrEmpty(ticket.Resolution))
        {
            throw new ArgumentException("Resolution is required");
        }
    }
    private int? GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId) ? userId : null;
    }
}


