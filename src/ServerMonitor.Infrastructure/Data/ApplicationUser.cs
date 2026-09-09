using Microsoft.AspNetCore.Identity;

namespace ServerMonitor.Infrastructure.Data;

public class ApplicationUser : IdentityUser
{
    public int DefaultCheckIntervalSeconds { get; set; } = 60;
}
