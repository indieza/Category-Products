using AutoMapper;

using CategoryProducts.Profiles;

using CategoryProducts.Constraints;
using CategoryProducts.Data;
using CategoryProducts.Data.Models.User;
using CategoryProducts.Server.Hubs;
using CategoryProducts.Services.User;
using CategoryProducts.Shared;
using CategoryProducts.ViewModels.System;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using Newtonsoft.Json;

using System.Globalization;
using System.Text;
using CategoryProducts.Services.Shop;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddMemoryCache();
services.Configure<JwtOptions>(builder.Configuration.GetSection("JWTSettings"));

services.AddDbContext<Context>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

services.AddIdentity<User, Role>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<Context>();

services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = ModelConstraints.PasswordMinLength;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = false;
    options.User.AllowedUserNameCharacters = "aAbBcCdDeEfFgGhHiIjJkKlLmMnNoOpPqQrRsStTuUvVwWxXyYzZ0123456789-_.";
});

services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.Zero;
});

services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = false;
    options.LoginPath = "/Login";
    options.ExpireTimeSpan = TimeSpan.FromDays(15);
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
});

var jwtSettings = builder.Configuration.GetSection("JWTSettings");
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = AppConstants.AppAuhtHeader;
    opt.DefaultChallengeScheme = AppConstants.AppAuhtHeader;
}).AddScheme<AppAuthSchemeOptions, AppAuthHandler>(AppConstants.AppAuhtHeader, options =>
{
    // TODO
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["ValidIssuer"],
        ValidAudience = jwtSettings["ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecurityKey"])),
        ClockSkew = TimeSpan.Zero,
    };
});

services.AddTransient<IUserService, UserService>();
services.AddTransient<IShopService, ShopService>();

services.AddAuthorization();

services.AddLocalization();

services.AddCors(policy =>
{
    policy.AddPolicy("CorsPolicy", options =>
    {
        options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

services.AddControllers().AddNewtonsoftJson();
services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });

services.AddSignalR();
services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

services.AddAutoMapper(typeof(Program).Assembly);
services.AddScoped(provider => new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new UserProfile());
    cfg.AddProfile(new ShopProfile());
}).CreateMapper());

services.AddRazorPages();

var app = builder.Build();

app.UseResponseCompression();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");

// app.UseMiddleware<JwtMiddleware>();
app.UseBlazorFrameworkFiles();

app.UseStaticFiles();

app.UseRequestLocalization(new RequestLocalizationOptions()
{
    DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US"),
    SupportedCultures = new List<CultureInfo>() { new CultureInfo("en-US"), new CultureInfo("bg-BG") },
    SupportedUICultures = new List<CultureInfo>() { new CultureInfo("en-US"), new CultureInfo("bg-BG") },
});

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapFallbackToFile("Profile/{username?}", "index.html");
});

app.MapRazorPages();
app.MapControllers();
app.MapHub<UserHub>("/userhub");
app.MapHub<ShopHub>("/shophub");
app.MapFallbackToFile("index.html");
app.Run();