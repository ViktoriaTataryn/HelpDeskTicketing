using HelpDeskTicketing.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskTicketing.Storage;

public class HelpDeskContext : DbContext
{
    public HelpDeskContext()
    {
        
    }
    public HelpDeskContext(DbContextOptions<HelpDeskContext> options) : base(options)
    {
        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=DESKTOP-42S05RP;Database=HelpDeskTicketingDb;Integrated Security=True;TrustServerCertificate=True;");
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Comment> Comments { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Збереження enum як int у БД
        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<int>();

        modelBuilder.Entity<Ticket>()
            .Property(t => t.Status)
            .HasConversion<int>();

        //  зв’язок між Ticket і CreatedBy (User)
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.User) 
            .WithMany(u => u.CreatedTickets)
            .HasForeignKey(t => t.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        //  зв’язок між Ticket і AssignedTo (Admin)
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Admin)
            .WithMany(u => u.AssignedTickets)
            .HasForeignKey(t => t.AssignedTo)
            .OnDelete(DeleteBehavior.Restrict);

        //  зв’язок між Comment і Ticket
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Ticket)
            .WithMany()
            .HasForeignKey(c => c.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        //  зв’язок між Comment і User
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}