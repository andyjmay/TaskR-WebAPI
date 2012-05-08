using System.Data.Entity;

namespace TaskR.Models {
  public class TaskEntities : DbContext {
    public DbSet<Task> Tasks { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder) {
      modelBuilder.Entity<Task>().HasKey(t => t.TaskID);
      modelBuilder.Entity<Task>().Property(t => t.Title).IsRequired();
      modelBuilder.Entity<Task>().Property(t => t.Details).IsRequired();
      base.OnModelCreating(modelBuilder);
    }
  }
}