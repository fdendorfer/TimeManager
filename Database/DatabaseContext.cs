using Microsoft.EntityFrameworkCore;
using TimeManager.Model;

namespace TimeManager.Database {
  public class DatabaseContext : DbContext {
    public DbSet<User> User { get; set; }
    public DbSet<Permission> Permission { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
      optionsBuilder.UseSqlServer("Server=localhost\\Home;Database=TimeManager;Trusted_Connection=True");
      //optionsBuilder.UseSqlServer("Server=localhost\\Home;Database=TimeManager;User Id=sa;Password=vawbuNHGNPKCZ77+");
    }
  }
}
