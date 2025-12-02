using Microsoft.AspNetCore.Components.Authorization;

namespace BD_ProjetBlazor.Services
{
    public class UserSessionService
    {
        private readonly AuthenticationStateProvider _authProvider;

        public UserSessionService(AuthenticationStateProvider authProvider)
        {
            _authProvider = authProvider;
        }

        public async Task<int?> GetUserIdAsync()
        {
            var state = await _authProvider.GetAuthenticationStateAsync();
            var user = state.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                var claim = user.FindFirst("userId");
                if (claim != null)
                    return int.Parse(claim.Value);
            }

            return null;
        }
    }
}
