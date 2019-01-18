using Microsoft.EntityFrameworkCore;
using TimeManager.Model;

namespace TimeManager.Database {
  public class DatabaseContext : DbContext {
    public DbSet<AbsenceModel> Absence {get; set;}
    public DbSet<AbsenceDetailModel> AbsenceDetail {get; set;}
    public DbSet<OvertimeModel> Overtime { get; set; }
    public DbSet<OvertimeDetailModel> OvertimeDetail { get; set; }
    public DbSet<PermissionModel> Permission { get; set; }
    public DbSet<UserModel> User { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
      optionsBuilder.EnableSensitiveDataLogging(true);
      optionsBuilder.UseSqlServer("Server=localhost;Database=TimeManager;Trusted_Connection=True");
      //optionsBuilder.UseSqlServer("Server=localhost\\Home;Database=TimeManager;User Id=sa;Password=vawbuNHGNPKCZ77+");
    }
  }
}
