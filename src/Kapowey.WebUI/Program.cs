using System.Diagnostics;
using System.Text;
using FluentEmail.MailKitSmtp;
using Kapowey.Core.Common.Configuration;
using Kapowey.Core.Common.Interfaces;
using Kapowey.Core.Common.Models;
using Kapowey.Core.Entities;
using Kapowey.Core.Persistance;
using Kapowey.Core.Services;
using Kapowey.Core.Services.Data;
using Kapowey.Core.Services.Identity;
using Kapowey.WebUI.Areas.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MudBlazor.Extensions;
using MudBlazor.Services;
using ScottBrady91.AspNetCore.Identity;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

#region Configuration related

var settings = new AppConfigurationSettings();
builder.Configuration.GetSection("AppConfigurationSettings").Bind(settings);
settings.WebRootPath = builder.Environment.WebRootPath;
settings.EnsureSetup();
builder.Services.AddSingleton(settings);

var smtpClientOptions = new SmtpClientOptions();
builder.Configuration.GetSection("SmtpClientOptions").Bind(smtpClientOptions);
builder.Services.AddSingleton(smtpClientOptions);

builder.Services.AddSingleton<IdentitySettings>(opt =>
{
    var dentitySettings = new IdentitySettings();
    builder.Configuration.GetSection("IdentitySettings").Bind(dentitySettings);
    return dentitySettings;
});

#endregion

#region Loggin related

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
builder.Host.UseSerilog(logger);
Trace.Listeners.Add(new LoggingTraceListener());

#endregion

builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddSingleton<IHttpEncoder, HttpEncoder>();
builder.Services.AddSingleton<IClockProvider, UtcClockProvider>();

builder.Services.AddLazyCache();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
builder.Services.AddMudBlazorDialog();
builder.Services.AddMudExtensions();
builder.Services.AddHealthChecks();

builder.Services.AddDbContext<KapoweyContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("KapoweyConnectionString"), o => o.UseNodaTime())
        .EnableDetailedErrors()
        .EnableSensitiveDataLogging());

builder.Services.AddDbContextFactory<KapoweyContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("KapoweyConnectionString"), o => o.UseNodaTime())
        .EnableDetailedErrors()
        .EnableSensitiveDataLogging(), ServiceLifetime.Scoped);

#region Security Related

builder.Services.AddScoped<KapoweySignInManager>();
builder.Services.AddScoped<KapoweyUserManager>();
builder.Services.AddScoped<KapoweyUserStore>();

builder.Services.AddIdentity<User, UserRole>()
    .AddRoles<UserRole>()
    .AddUserManager<KapoweyUserManager>()
    .AddSignInManager<KapoweySignInManager>()
    .AddUserStore<KapoweyUserStore>()
    .AddEntityFrameworkStores<KapoweyContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(o =>
    {
        o.DefaultScheme = IdentityConstants.ApplicationScheme;
        o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddCookie()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        googleOptions.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
    });

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.Cookie.Name = "KapoweyAuth";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Identity/Account/Login";
    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
    options.SlidingExpiration = true;
});

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential 
    // cookies is needed for a given request.
    options.CheckConsentNeeded = context => true;
    // requires using Microsoft.AspNetCore.Http;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddScoped<IPasswordHasher<User>, BCryptPasswordHasher<User>>();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<User>>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(UserRoleRegistry.AdminRoleName, policy => policy.RequireRole(UserRoleRegistry.AdminRoleName));
    options.AddPolicy(UserRoleRegistry.ManagerRoleName,
        policy => policy.RequireRole(UserRoleRegistry.AdminRoleName, UserRoleRegistry.ManagerRoleName));
    options.AddPolicy(UserRoleRegistry.EditorRoleName,
        policy => policy.RequireRole(UserRoleRegistry.AdminRoleName, UserRoleRegistry.EditorRoleName,
            UserRoleRegistry.ManagerRoleName));
    options.AddPolicy(UserRoleRegistry.ContributorRoleName,
        policy => policy.RequireRole(UserRoleRegistry.AdminRoleName, UserRoleRegistry.ContributorRoleName,
            UserRoleRegistry.EditorRoleName, UserRoleRegistry.ManagerRoleName));
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<HttpContextAccessor>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<HttpClient>();

builder.Services.AddScoped<IKapoweyHttpContext>(factory =>
{
    var actionContext = factory.GetService<IActionContextAccessor>().ActionContext;
    if (actionContext == null) return null;
    return new KapoweyHttpContext(settings, new UrlHelper(actionContext), factory.GetService<IClockProvider>());
});

#endregion

#region Services

builder.Services.AddFluentEmail(smtpClientOptions.User)
    .AddRazorRenderer("/home/steven/source/kapowey/src/Kapowey.Core/Templates/EmailTemplates") // TODO; should be more dynamic
    .AddMailKitSender(smtpClientOptions);
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFranchiseCategoryService, FranchiseCategoryService>();
builder.Services.AddScoped<IFranchiseService, FranchiseService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IGradeService, GradeService>();
builder.Services.AddScoped<IGradeTermService, GradeTermService>();
builder.Services.AddScoped<IPublisherCategoryService, PublisherCategoryService>();
builder.Services.AddScoped<ISeriesCategoryService, SeriesCategoryService>();
builder.Services.AddScoped<IPublisherService, PublisherService>();
builder.Services.AddScoped<ISeriesService, SeriesService>();
builder.Services.AddScoped<IIssueTypeService, IssueTypeService>();
builder.Services.AddScoped<IIssueService, IssueService>();
builder.Services.AddScoped<ICollectionService, CollectionService>();
builder.Services.AddScoped<ICollectionIssueService, CollectionService>();
builder.Services.AddScoped<IApiApplicationService, ApiApplicationService>();

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseStaticFiles();
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

app.UseRouting();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();