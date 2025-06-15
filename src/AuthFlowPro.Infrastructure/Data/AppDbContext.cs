using Microsoft.EntityFrameworkCore;
using AuthFlowPro.Domain;

namespace AuthFlowPro.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
}
