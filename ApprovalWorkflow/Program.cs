using System.Reflection;
using ApprovalSystem.Data;
using ApprovalSystem.Extensions;
using ApprovalSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                 .WriteTo.Console()
                 .WriteTo.File("AppLogs\\log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: null)
                 .Enrich.FromLogContext()
                 .CreateLogger();

//try
//{
    Log.Information($"Starting Approval Workflow at {DateTime.Now}");
    var builder = WebApplication.CreateBuilder(args);

    #region Setup Logging

    builder.Host.UseSerilog((context, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration));
    #endregion

    // Add services to the container.
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));
    });

    #region Mvc Setup
    var mvcBuilder = builder.Services.AddMvc(options =>
    {
        var lockAll = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

        options.Filters.Add(new AuthorizeFilter(lockAll));
        options.EnableEndpointRouting = false;
    })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
        });

    builder.Services.Configure<CookieOptions>(options =>
    {
        options.SameSite = SameSiteMode.Strict;
        options.HttpOnly = true;
        options.MaxAge = TimeSpan.FromDays(2);
        options.Secure = true;
        options.IsEssential = true;
    });
    builder.Services.AddHttpContextAccessor();

    #endregion

    #region application services
    builder.Services.AddApplicationServices();
    #endregion

    #region Identity configurations

    builder.Services.AddIdentity<User, Role>()
        .AddUserManager<UserManager<User>>()
        .AddRoleManager<RoleManager<Role>>()
        .AddSignInManager<SignInManager<User>>()
        .AddDefaultTokenProviders()
        .AddEntityFrameworkStores<ApplicationDbContext>();

    builder.Services.Configure<IdentityOptions>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 8;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;

        options.SignIn.RequireConfirmedEmail = false;
        options.SignIn.RequireConfirmedAccount = false;
        options.SignIn.RequireConfirmedPhoneNumber = false;

        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
        options.Lockout.MaxFailedAccessAttempts = 5;

        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ.-_@1234567890";
        options.User.RequireUniqueEmail = true;
    });

    #endregion

    #region Authentication
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
        options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
    });

    builder.Services.AddAuthorization();
    #endregion

    #region Open Api Documentation
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "Approval Workflow",
            Description = "A general approval system which passes an item through a workflow of different users before it finally persists in the system",

            Contact = new OpenApiContact
            {
                Name = "Tavershima Ako",
                Email = "akotaver@gmail.com",
                Url = new Uri("https://localhost:34452/")
            },
            License = new OpenApiLicense
            {
                Name = "MIT License",
                Url = new Uri("https://localhost:34452/"),
            }
        });

        options.DocInclusionPredicate((docName, description) => true);

    });
    #endregion

    #region Cors
    builder.Services.AddCors(x =>
    {
        x.AddDefaultPolicy(n =>
        {
            n.WithOrigins(builder.Configuration.GetSection("AllowedHosts").Value)
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
    });
    #endregion

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint($"/swagger/v1/swagger.json", "Approval System");
            c.RoutePrefix = "swagger";
        });
    }

    app.UseHttpsRedirection();
    app.UseHsts();

    app.UseStaticFiles();
    app.UseRouting();
    app.UseCors();

    app.UseAuthentication();
    app.UseAuthorization();

#pragma warning disable ASP0014 // Suggest using top level route registrations
    app.UseEndpoints(endpoint =>
    {
        endpoint.MapControllers();
    });

#pragma warning restore ASP0014 // Suggest using top level route registrations

    app.Run();
//}
//catch (Exception e)
//{
//    Log.Fatal(e, "Application host failed to start");
//}