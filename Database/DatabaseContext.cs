using Microsoft.EntityFrameworkCore;
using TimeManager.Model;

namespace TimeManager.Database {
  public class DatabaseContext : DbContext {
    public DbSet<AbsenceModel> Absence {get; set;}
    public DbSet<AbsenceDetail> AbsenceDetail {get; set;}
    public DbSet<OvertimeModel> Overtime { get; set; }
    public DbSet<OvertimeDetail> OvertimeDetail { get; set; }
    public DbSet<Permission> Permission { get; set; }
    public DbSet<User> User { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
      optionsBuilder.UseSqlServer("Server=localhost;Database=TimeManager;Trusted_Connection=True");
      //optionsBuilder.UseSqlServer("Server=localhost\\Home;Database=TimeManager;User Id=sa;Password=vawbuNHGNPKCZ77+");
    }
  }
}
