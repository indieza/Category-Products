using Blazored.LocalStorage;

using BlazorStrap;

using CategoryProducts.Client;
using CategoryProducts.Client.States;
using CategoryProducts.ExtensionMethods;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var services = builder.Services;

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
services.AddLocalization();

services.AddScoped<UserAppState>();
services.AddScoped<ShopState>();

services.AddOptions();
services.AddAuthorizationCore();
services.AddScoped<AuthStateProvider>();
services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<AuthStateProvider>());

services.AddBlazorStrap();
services.AddBlazoredLocalStorage();

var host = builder.Build();
await host.SetDefaultCulture();
await host.RunAsync();

await builder.Build().RunAsync();