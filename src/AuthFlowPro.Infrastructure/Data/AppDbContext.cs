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

        // RefreshToken configuration
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.Id);
            entity.HasOne(rt => rt.User)
                  .WithMany(u => u.RefreshTokens)
                  .HasForeignKey(rt => rt.UserId)
                  .IsRequired();
        });

        // Organization configuration
        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.HasIndex(o => o.Slug).IsUnique();
            entity.Property(o => o.Name).IsRequired().HasMaxLength(100);
            entity.Property(o => o.Slug).IsRequired().HasMaxLength(50);
        });

        // OrganizationMember configuration
        modelBuilder.Entity<OrganizationMember>(entity =>
        {
            entity.HasKey(om => om.Id);
            entity.HasIndex(om => new { om.OrganizationId, om.UserId }).IsUnique();
            
            entity.HasOne(om => om.Organization)
                  .WithMany(o => o.Members)
                  .HasForeignKey(om => om.OrganizationId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(om => om.User)
                  .WithMany(u => u.OrganizationMemberships)
                  .HasForeignKey(om => om.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Invitation configuration
        modelBuilder.Entity<Invitation>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.HasIndex(i => i.Token).IsUnique();
            entity.Property(i => i.Email).IsRequired().HasMaxLength(255);
            
            entity.HasOne(i => i.Organization)
                  .WithMany(o => o.Invitations)
                  .HasForeignKey(i => i.OrganizationId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(i => i.InvitedBy)
                  .WithMany(u => u.SentInvitations)
                  .HasForeignKey(i => i.InvitedByUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Plan configuration
        modelBuilder.Entity<Plan>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
        });

        // Subscription configuration
        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.HasIndex(s => s.StripeSubscriptionId).IsUnique();
            
            entity.HasOne(s => s.Organization)
                  .WithOne(o => o.Subscription)
                  .HasForeignKey<Subscription>(s => s.OrganizationId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(s => s.Plan)
                  .WithMany(p => p.Subscriptions)
                  .HasForeignKey(s => s.PlanId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // AuditLog configuration
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(al => al.Id);
            entity.HasIndex(al => al.CreatedAt);
            entity.Property(al => al.Action).IsRequired().HasMaxLength(100);
            entity.Property(al => al.EntityType).IsRequired().HasMaxLength(50);
            
            entity.HasOne(al => al.Organization)
                  .WithMany(o => o.AuditLogs)
                  .HasForeignKey(al => al.OrganizationId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(al => al.User)
                  .WithMany(u => u.AuditLogs)
                  .HasForeignKey(al => al.UserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Notification configuration
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(n => n.Id);
            entity.HasIndex(n => new { n.UserId, n.IsRead });
            entity.Property(n => n.Title).IsRequired().HasMaxLength(200);
            
            entity.HasOne(n => n.User)
                  .WithMany(u => u.Notifications)
                  .HasForeignKey(n => n.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(n => n.Organization)
                  .WithMany()
                  .HasForeignKey(n => n.OrganizationId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }

    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<OrganizationMember> OrganizationMembers { get; set; }
    public DbSet<Invitation> Invitations { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<Notification> Notifications { get; set; }
}