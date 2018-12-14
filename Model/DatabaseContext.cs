using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TimeManager.Model {
  public class DatabaseContext : DbContext {
    public DbSet<User> User { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
      optionsBuilder.UseSqlServer("Server=localhost;Database=TimeManager;Trusted_Connection=True");
      //optionsBuilder.UseSqlServer("Server=localhost\\Home;Database=TimeManager;User Id=sa;Password=vawbuNHGNPKCZ77+");
    }
  }
}
