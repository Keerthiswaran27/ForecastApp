using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CAPService.Services
{
    public class CustomAuthProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime js;

        public CustomAuthProvider(IJSRuntime jsRuntime)
        {
            js = jsRuntime;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // Step 1: Try to read token from sessionStorage
            var token = await js.InvokeAsync<string>("sessionStorage.getItem", "accessToken");
            var uid = await js.InvokeAsync<string>("sessionStorage.getItem", "uid");

            // Step 2: If token is missing → anonymous
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(uid))
            {
                var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
                return new AuthenticationState(anonymous);
            }

            // Step 3: Otherwise, create identity from uid
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, uid),
        };

            var identity = new ClaimsIdentity(claims, "sessionStorage");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }

        // Optional: Call this after login
        public void MarkUserAsAuthenticated(string uid)
        {
            var identity = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, uid),
        }, "sessionStorage");

            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        // Optional: Call this on logout
        public void MarkUserAsLoggedOut()
        {
            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
        }
    }

}
