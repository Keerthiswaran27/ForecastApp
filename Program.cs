using CreateUserService.Services;
using ENService.Services;
using ESService.Services;
using ForecastApp;
using GASService.Services;
using GSService.Services;
using HService.Services;
using HSService.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using PSService.Services;
using RSService.Services;
using SCSService.Services;
using SigninService.Services;
using Supabase;
using UDService.Services;
using URService.Services; // Assuming this is a valid namespace in your project
using USService.Services;
using WAService.Services;
using WSService.Services;
using CAPService.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add root components
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient for API calls
builder.Services.AddScoped(sp => new HttpClient 
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

// Add MudBlazor UI services
builder.Services.AddMudServices();
builder.Services.AddScoped<Loginservice>();
builder.Services.AddScoped<SignupService>();

// Register your custom service (singleton)
builder.Services.AddScoped<UnregisterService>();
builder.Services.AddScoped<Userdata>();
builder.Services.AddScoped<RegisterService>();  
builder.Services.AddScoped<HourService>();
builder.Services.AddScoped<WeatherService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<EmailNotificationService>();
builder.Services.AddScoped<HistoryService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<WeatherApiService>();
builder.Services.AddScoped<GeminiService>();
builder.Services.AddSingleton<SupabaseClientService>();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthProvider>();
    


// ? Add GoogleAuthService
builder.Services.AddScoped<GoogleAuthService>();


await builder.Build().RunAsync();
