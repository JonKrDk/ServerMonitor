using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace ServerMonitor.Web.Services;

public class CurrentUserAccessor(AuthenticationStateProvider authenticationStateProvider)
{
    public async Task<string> GetIdAsync() =>
        await GetIdOrDefaultAsync() ?? throw new InvalidOperationException("No authenticated user.");

    public async Task<string?> GetIdOrDefaultAsync()
    {
        var state = await authenticationStateProvider.GetAuthenticationStateAsync();
        return state.User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
