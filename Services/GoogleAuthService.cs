using Microsoft.AspNetCore.Components;
using SCSService.Services;
using System;

namespace GASService.Services
{
    public class GoogleAuthService
    {
        private readonly SupabaseClientService _supabaseClientService;
        private readonly NavigationManager _navigationManager;

        public GoogleAuthService(SupabaseClientService supabaseClientService, NavigationManager navigationManager)
        {
            _supabaseClientService = supabaseClientService;
            _navigationManager = navigationManager;
        }

        public string BuildGoogleOAuthUrl()
        {
            // Supabase hosted authorize endpoint
            var supabaseUrl = "https://buvihfrcbabpgtxyepfy.supabase.co";

            // Redirect back to our own app
            var redirectUri = $"{_navigationManager.BaseUri}auth-callback";
            var encodedRedirect = Uri.EscapeDataString(redirectUri);

            return $"{supabaseUrl}/auth/v1/authorize?provider=google&redirect_to={encodedRedirect}";
        }
    }
}
