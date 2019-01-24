using Microsoft.EntityFrameworkCore;
using TimeManager.Model;

namespace TimeManager.Database
{
  public class DatabaseContext : DbContext
  {
    public DbSet<AbsenceModel> Absence { get; set; }
    public DbSet<AbsenceDetailModel> AbsenceDetail { get; set; }
    public DbSet<OvertimeModel> Overtime { get; set; }
    public DbSet<OvertimeDetailModel> OvertimeDetail { get; set; }
    public DbSet<PermissionModel> Permission { get; set; }
    public DbSet<UserModel> User { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.EnableSensitiveDataLogging(true);
      optionsBuilder.UseSqlServer("Server=localhost;Database=TimeManager;User Id=WebAdmin;Password=DunEWWFL28AEtI");
    }
  }
}
