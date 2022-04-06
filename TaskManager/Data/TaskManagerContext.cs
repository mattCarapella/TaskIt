using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Areas.Identity.Data;
using TaskManager.Models;

namespace TaskManager.Data;

public class TaskManagerContext : IdentityDbContext<ApplicationUser>
{
    public TaskManagerContext(DbContextOptions<TaskManagerContext> options)
        : base(options)
    {
    }

    public DbSet<Project>? Projects { get; set; }
    public DbSet<ProjectAssignment>? ProjectAssignments { get; set; }
    public DbSet<Ticket>? Tickets { get; set; }
    public DbSet<TicketAssignment>? TicketAssignments { get; set; }
    //public DbSet<Note> Notes { get; set; }
    public DbSet<PNote> PNote { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new ApplicationUserEntityConfiguration());


        builder.Entity<ProjectAssignment>()
            .HasOne(u => u.ApplicationUser)
            .WithMany(p => p.Projects)
            .HasForeignKey(pa => pa.ApplicationUserId);

        builder.Entity<ProjectAssignment>()
            .HasOne(p => p.Project)
            .WithMany(u => u.Contributers)
            .HasForeignKey(pa => pa.ProjectId);

        builder.Entity<ProjectAssignment>().HasKey(pa => new { pa.ProjectId, pa.ApplicationUserId });


        builder.Entity<TicketAssignment>()
            .HasOne(u => u.ApplicationUser)
            .WithMany(t => t.Tickets)
            .HasForeignKey(ta => ta.ApplicationUserId);

        builder.Entity<TicketAssignment>()
            .HasOne(t => t.Ticket)
            .WithMany(u => u.AssignedTo)
            .HasForeignKey(pa => pa.TicketId);

        builder.Entity<TicketAssignment>().HasKey(ta => new { ta.TicketId, ta.ApplicationUserId });


        builder.Entity<Ticket>()
            .HasOne(p => p.Project)
            .WithMany(t => t.Tickets)
            .OnDelete(DeleteBehavior.Cascade);


        builder.Entity<PNote>()
            .HasOne(p => p.Project)
            .WithMany(n => n.Notes)
            .OnDelete(DeleteBehavior.Cascade);

    }



    

}

internal class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FirstName).HasMaxLength(255);
        builder.Property(u => u.LastName).HasMaxLength(255);
    }

}   