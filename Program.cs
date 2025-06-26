using CreateUserService.Services;
using ENService.Services;
using ESService.Services;
using ForecastApp;
using HSService.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using RSService.Services;
using SigninService.Services;
using UDService.Services;
using URService.Services; // Assuming this is a valid namespace in your project
using WSService.Services;
using HService.Services;
using USService.Services;
using PSService.Services;

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

await builder.Build().RunAsync();
