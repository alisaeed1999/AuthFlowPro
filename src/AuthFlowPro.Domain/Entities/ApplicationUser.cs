using Microsoft.AspNetCore.Identity;

namespace AuthFlowPro.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
