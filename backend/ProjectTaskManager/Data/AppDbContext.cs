using Microsoft.EntityFrameworkCore;
using Projecttaskmanager.Models;

namespace Projecttaskmanager.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Users> User => Set<Users>();
    //can be used to query and save instances of Users.
    //LinQ queries against a DbSet<> will be translated into queries against the database
    //the results of a LINQ query against a DbSet <> will contain the results returned from
    //  the database and may not reflect changes made in the context that have not been persisted to the database
    public DbSet<Project> project => Set<Project>();
    public DbSet<ProjectTasks> tasks => Set<ProjectTasks>();
    public DbSet<TaskDependency> dependent => Set<TaskDependency>();
    public DbSet<Notification> notify => Set<Notification>();
    public DbSet<OtpCode> OtpCodes => Set<OtpCode>();


    //to create composite key...
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        //otp
        modelBuilder.Entity<OtpCode>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e =>e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e =>e.Code).IsRequired().HasMaxLength(6);
            entity.Property(e =>e.Purpose).IsRequired().HasMaxLength(50);
        });

        //  TaskDependency composite PK
        modelBuilder.Entity<TaskDependency>()
            .HasKey(td => new { td.TaskId, td.DependentTaskId });

        // Users → Projects one owner, many projects
        modelBuilder.Entity<Project>()
            .HasOne(p => p.Owner)
            .WithMany(u => u.OwnedProjects)
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict); // don't delete projects when user deleted

        modelBuilder.Entity<ProjectTasks>()
            .HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Restrict); 

        // Users → Tasks one assignee, many tasks
        modelBuilder.Entity<ProjectTasks>()
            .HasOne(t => t.Assignee)
            .WithMany(u => u.AssignedTasks)
            .HasForeignKey(t => t.AssigneeId)
            .OnDelete(DeleteBehavior.Restrict); // don't wipe tasks when user deleted

        //  Users → Notifications one user, many notifications
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade); // delete notifications when user deleted

        //  TaskDependency → Task the task that has a dependency
        modelBuilder.Entity<TaskDependency>()
            .HasOne(td => td.Task)
            .WithMany(t => t.Dependencies)
            .HasForeignKey(td => td.TaskId)
            .OnDelete(DeleteBehavior.Restrict);

        //  TaskDependency → DependentTask the task being depended on
        modelBuilder.Entity<TaskDependency>()
            .HasOne(td => td.DependentTask)
            .WithMany(t => t.Dependents)
            .HasForeignKey(td => td.DependentTaskId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}