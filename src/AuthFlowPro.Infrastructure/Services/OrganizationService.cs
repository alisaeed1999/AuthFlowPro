using AuthFlowPro.Application.DTOs.Organization;
using AuthFlowPro.Application.Interfaces;
using AuthFlowPro.Domain.Entities;
using AuthFlowPro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace AuthFlowPro.Infrastructure.Services;

public class OrganizationService : IOrganizationService
{
    private readonly AppDbContext _context;
    private readonly IAuditService _auditService;
    private readonly INotificationService _notificationService;

    public OrganizationService(AppDbContext context, IAuditService auditService, INotificationService notificationService)
    {
        _context = context;
        _auditService = auditService;
        _notificationService = notificationService;
    }

    public async Task<List<OrganizationDto>> GetUserOrganizationsAsync(Guid userId)
    {
        return await _context.OrganizationMembers
            .Where(om => om.UserId == userId && om.IsActive)
            .Include(om => om.Organization)
            .ThenInclude(o => o.Subscription)
            .ThenInclude(s => s!.Plan)
            .Select(om => new OrganizationDto
            {
                Id = om.Organization.Id,
                Name = om.Organization.Name,
                Slug = om.Organization.Slug,
                Description = om.Organization.Description,
                Website = om.Organization.Website,
                Logo = om.Organization.Logo,
                CreatedAt = om.Organization.CreatedAt,
                IsActive = om.Organization.IsActive,
                MemberCount = om.Organization.Members.Count(m => m.IsActive),
                Subscription = om.Organization.Subscription != null ? new SubscriptionDto
                {
                    Id = om.Organization.Subscription.Id,
                    PlanId = om.Organization.Subscription.PlanId,
                    PlanName = om.Organization.Subscription.Plan.Name,
                    Price = om.Organization.Subscription.Plan.Price,
                    Currency = om.Organization.Subscription.Plan.Currency,
                    Interval = om.Organization.Subscription.Plan.Interval,
                    Status = om.Organization.Subscription.Status,
                    CurrentPeriodStart = om.Organization.Subscription.CurrentPeriodStart,
                    CurrentPeriodEnd = om.Organization.Subscription.CurrentPeriodEnd,
                    AutoRenew = om.Organization.Subscription.AutoRenew
                } : null
            })
            .ToListAsync();
    }

    public async Task<OrganizationDto?> GetOrganizationAsync(Guid organizationId)
    {
        return await _context.Organizations
            .Where(o => o.Id == organizationId)
            .Include(o => o.Subscription)
            .ThenInclude(s => s!.Plan)
            .Select(o => new OrganizationDto
            {
                Id = o.Id,
                Name = o.Name,
                Slug = o.Slug,
                Description = o.Description,
                Website = o.Website,
                Logo = o.Logo,
                CreatedAt = o.CreatedAt,
                IsActive = o.IsActive,
                MemberCount = o.Members.Count(m => m.IsActive),
                Subscription = o.Subscription != null ? new SubscriptionDto
                {
                    Id = o.Subscription.Id,
                    PlanId = o.Subscription.PlanId,
                    PlanName = o.Subscription.Plan.Name,
                    Price = o.Subscription.Plan.Price,
                    Currency = o.Subscription.Plan.Currency,
                    Interval = o.Subscription.Plan.Interval,
                    Status = o.Subscription.Status,
                    CurrentPeriodStart = o.Subscription.CurrentPeriodStart,
                    CurrentPeriodEnd = o.Subscription.CurrentPeriodEnd,
                    AutoRenew = o.Subscription.AutoRenew
                } : null
            })
            .FirstOrDefaultAsync();
    }

    public async Task<OrganizationDto?> GetOrganizationBySlugAsync(string slug)
    {
        return await _context.Organizations
            .Where(o => o.Slug == slug)
            .Include(o => o.Subscription)
            .ThenInclude(s => s!.Plan)
            .Select(o => new OrganizationDto
            {
                Id = o.Id,
                Name = o.Name,
                Slug = o.Slug,
                Description = o.Description,
                Website = o.Website,
                Logo = o.Logo,
                CreatedAt = o.CreatedAt,
                IsActive = o.IsActive,
                MemberCount = o.Members.Count(m => m.IsActive),
                Subscription = o.Subscription != null ? new SubscriptionDto
                {
                    Id = o.Subscription.Id,
                    PlanId = o.Subscription.PlanId,
                    PlanName = o.Subscription.Plan.Name,
                    Price = o.Subscription.Plan.Price,
                    Currency = o.Subscription.Plan.Currency,
                    Interval = o.Subscription.Plan.Interval,
                    Status = o.Subscription.Status,
                    CurrentPeriodStart = o.Subscription.CurrentPeriodStart,
                    CurrentPeriodEnd = o.Subscription.CurrentPeriodEnd,
                    AutoRenew = o.Subscription.AutoRenew
                } : null
            })
            .FirstOrDefaultAsync();
    }

    public async Task<(bool Success, string Message, OrganizationDto? Organization)> CreateOrganizationAsync(Guid userId, CreateOrganizationRequest request)
    {
        // Check if slug is already taken
        if (await _context.Organizations.AnyAsync(o => o.Slug == request.Slug))
        {
            return (false, "Organization slug is already taken", null);
        }

        var organization = new Organization
        {
            Name = request.Name,
            Slug = request.Slug,
            Description = request.Description,
            Website = request.Website
        };

        _context.Organizations.Add(organization);

        // Add the creator as owner
        var membership = new OrganizationMember
        {
            OrganizationId = organization.Id,
            UserId = userId,
            Role = OrganizationRole.Owner
        };

        _context.OrganizationMembers.Add(membership);

        await _context.SaveChangesAsync();

        await _auditService.LogAsync(organization.Id, userId, "organization.created", "Organization", organization.Id.ToString());

        var organizationDto = new OrganizationDto
        {
            Id = organization.Id,
            Name = organization.Name,
            Slug = organization.Slug,
            Description = organization.Description,
            Website = organization.Website,
            Logo = organization.Logo,
            CreatedAt = organization.CreatedAt,
            IsActive = organization.IsActive,
            MemberCount = 1
        };

        return (true, "Organization created successfully", organizationDto);
    }

    public async Task<(bool Success, string Message)> UpdateOrganizationAsync(Guid organizationId, UpdateOrganizationRequest request)
    {
        var organization = await _context.Organizations.FindAsync(organizationId);
        if (organization == null)
        {
            return (false, "Organization not found");
        }

        organization.Name = request.Name;
        organization.Description = request.Description;
        organization.Website = request.Website;
        organization.Logo = request.Logo;
        organization.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _auditService.LogAsync(organizationId, null, "organization.updated", "Organization", organizationId.ToString());

        return (true, "Organization updated successfully");
    }

    public async Task<(bool Success, string Message)> DeleteOrganizationAsync(Guid organizationId)
    {
        var organization = await _context.Organizations.FindAsync(organizationId);
        if (organization == null)
        {
            return (false, "Organization not found");
        }

        _context.Organizations.Remove(organization);
        await _context.SaveChangesAsync();

        await _auditService.LogAsync(organizationId, null, "organization.deleted", "Organization", organizationId.ToString());

        return (true, "Organization deleted successfully");
    }

    public async Task<List<OrganizationMemberDto>> GetOrganizationMembersAsync(Guid organizationId)
    {
        return await _context.OrganizationMembers
            .Where(om => om.OrganizationId == organizationId && om.IsActive)
            .Include(om => om.User)
            .Select(om => new OrganizationMemberDto
            {
                Id = om.Id,
                UserId = om.UserId,
                Email = om.User.Email!,
                FullName = om.User.FullName,
                Avatar = om.User.Avatar,
                Role = om.Role,
                JoinedAt = om.JoinedAt,
                IsActive = om.IsActive
            })
            .ToListAsync();
    }

    public async Task<(bool Success, string Message)> InviteMemberAsync(Guid organizationId, Guid invitedByUserId, InviteMemberRequest request)
    {
        // Check if user is already a member
        if (await _context.OrganizationMembers.AnyAsync(om => om.OrganizationId == organizationId && om.User.Email == request.Email))
        {
            return (false, "User is already a member of this organization");
        }

        // Check if there's already a pending invitation
        if (await _context.Invitations.AnyAsync(i => i.OrganizationId == organizationId && i.Email == request.Email && !i.IsAccepted && !i.IsRevoked))
        {
            return (false, "There's already a pending invitation for this email");
        }

        var token = GenerateInvitationToken();
        var invitation = new Invitation
        {
            OrganizationId = organizationId,
            Email = request.Email,
            Role = request.Role,
            Token = token,
            InvitedByUserId = invitedByUserId
        };

        _context.Invitations.Add(invitation);
        await _context.SaveChangesAsync();

        await _auditService.LogAsync(organizationId, invitedByUserId, "member.invited", "Invitation", invitation.Id.ToString(), new { Email = request.Email, Role = request.Role });

        // TODO: Send invitation email

        return (true, "Invitation sent successfully");
    }

    public async Task<(bool Success, string Message)> UpdateMemberRoleAsync(Guid organizationId, UpdateMemberRoleRequest request)
    {
        var member = await _context.OrganizationMembers
            .FirstOrDefaultAsync(om => om.Id == request.MemberId && om.OrganizationId == organizationId);

        if (member == null)
        {
            return (false, "Member not found");
        }

        var oldRole = member.Role;
        member.Role = request.Role;

        await _context.SaveChangesAsync();

        await _auditService.LogAsync(organizationId, null, "member.role_updated", "OrganizationMember", member.Id.ToString(), new { OldRole = oldRole, NewRole = request.Role });

        return (true, "Member role updated successfully");
    }

    public async Task<(bool Success, string Message)> RemoveMemberAsync(Guid organizationId, Guid memberId)
    {
        var member = await _context.OrganizationMembers
            .FirstOrDefaultAsync(om => om.Id == memberId && om.OrganizationId == organizationId);

        if (member == null)
        {
            return (false, "Member not found");
        }

        // Don't allow removing the last owner
        if (member.Role == OrganizationRole.Owner)
        {
            var ownerCount = await _context.OrganizationMembers
                .CountAsync(om => om.OrganizationId == organizationId && om.Role == OrganizationRole.Owner && om.IsActive);

            if (ownerCount <= 1)
            {
                return (false, "Cannot remove the last owner of the organization");
            }
        }

        _context.OrganizationMembers.Remove(member);
        await _context.SaveChangesAsync();

        await _auditService.LogAsync(organizationId, null, "member.removed", "OrganizationMember", member.Id.ToString());

        return (true, "Member removed successfully");
    }

    public async Task<List<InvitationDto>> GetPendingInvitationsAsync(Guid organizationId)
    {
        return await _context.Invitations
            .Where(i => i.OrganizationId == organizationId && !i.IsAccepted && !i.IsRevoked && i.ExpiresAt > DateTime.UtcNow)
            .Include(i => i.InvitedBy)
            .Include(i => i.Organization)
            .Select(i => new InvitationDto
            {
                Id = i.Id,
                Email = i.Email,
                Role = i.Role,
                InvitedByName = i.InvitedBy.FullName,
                OrganizationName = i.Organization.Name,
                CreatedAt = i.CreatedAt,
                ExpiresAt = i.ExpiresAt,
                IsAccepted = i.IsAccepted,
                IsRevoked = i.IsRevoked
            })
            .ToListAsync();
    }

    public async Task<(bool Success, string Message)> AcceptInvitationAsync(string token, Guid userId)
    {
        var invitation = await _context.Invitations
            .Include(i => i.Organization)
            .FirstOrDefaultAsync(i => i.Token == token && !i.IsAccepted && !i.IsRevoked && i.ExpiresAt > DateTime.UtcNow);

        if (invitation == null)
        {
            return (false, "Invalid or expired invitation");
        }

        // Check if user is already a member
        if (await _context.OrganizationMembers.AnyAsync(om => om.OrganizationId == invitation.OrganizationId && om.UserId == userId))
        {
            return (false, "You are already a member of this organization");
        }

        // Create membership
        var membership = new OrganizationMember
        {
            OrganizationId = invitation.OrganizationId,
            UserId = userId,
            Role = invitation.Role
        };

        _context.OrganizationMembers.Add(membership);

        // Mark invitation as accepted
        invitation.IsAccepted = true;
        invitation.AcceptedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _auditService.LogAsync(invitation.OrganizationId, userId, "member.joined", "OrganizationMember", membership.Id.ToString());

        return (true, $"Successfully joined {invitation.Organization.Name}");
    }

    public async Task<(bool Success, string Message)> RevokeInvitationAsync(Guid invitationId)
    {
        var invitation = await _context.Invitations.FindAsync(invitationId);
        if (invitation == null)
        {
            return (false, "Invitation not found");
        }

        invitation.IsRevoked = true;
        await _context.SaveChangesAsync();

        await _auditService.LogAsync(invitation.OrganizationId, null, "invitation.revoked", "Invitation", invitationId.ToString());

        return (true, "Invitation revoked successfully");
    }

    private static string GenerateInvitationToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }
}