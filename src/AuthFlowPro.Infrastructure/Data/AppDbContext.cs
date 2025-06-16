using Microsoft.EntityFrameworkCore;
using AuthFlowPro.Domain;
using AuthFlowPro.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace AuthFlowPro.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { 
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Example of correct usage
    modelBuilder.Entity<RefreshToken>(entity =>
    {
        entity.HasKey(rt => rt.Id);
        entity.HasOne(rt => rt.User)
              .WithMany()
              .HasForeignKey(rt => rt.UserId);
    });
}

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    
}
