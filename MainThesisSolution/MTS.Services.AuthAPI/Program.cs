using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MTS.Services.AuthAPI.Data;
using MTS.Services.AuthAPI.Models;
using MTS.Services.AuthAPI.Service.IService;
using MTS.Services.AuthAPI.Service;
using MTS.Services.AuthAPI.Repository.IRepository;
using MTS.Services.AuthAPI.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ConnectionString"),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()
    )
);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await SeedAdminUser(userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();
async Task SeedAdminUser(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
{
    // Create admin role if it doesn't exist
    if (!await roleManager.RoleExistsAsync("ADMIN"))
    {
        await roleManager.CreateAsync(new IdentityRole("ADMIN"));
    }

    // Create admin user if it doesn't exist
    string adminEmail = "admin@university.edu";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            Name = "System Administrator",
            UniversityId = "ADMIN001" // Give a special ID for admin
        };

        var result = await userManager.CreateAsync(adminUser, "Admin@01");

        if (result.Succeeded)
        {
            // Assign admin role
            await userManager.AddToRoleAsync(adminUser, "ADMIN");
        }
    }
}