using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MooraHub.Data;
using MooraHub.Services;

var builder = WebApplication.CreateBuilder(args);

// =========================
// DB
// =========================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// =========================
// Identity + Roles
// =========================
builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>() // ✅ Roles enabled
    .AddEntityFrameworkStores<ApplicationDbContext>();

// ✅ Admin-only policy (used by [Authorize(Policy="AdminOnly")])
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// =========================
// MVC + RazorPages
// =========================
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// =========================
// Session + Services
// =========================
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(4);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<CartSessionService>();

// ✅ Upgrade #1 badge helper service (safe even if you don’t use it yet)
builder.Services.AddScoped<InboxBadgeService>();

var app = builder.Build();

// =========================
// Seed Admin Role + Admin User
// =========================
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    const string adminRole = "Admin";
    const string adminEmail = "thabisoelseventyseven@gmail.com";
    const string adminPassword = "Mosesi100#";

    // 1) Ensure role exists
    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        var roleResult = await roleManager.CreateAsync(new IdentityRole(adminRole));
        if (!roleResult.Succeeded)
        {
            var errors = string.Join(" | ", roleResult.Errors.Select(e => e.Description));
            throw new Exception("Admin role creation failed: " + errors);
        }
    }

    // 2) Ensure admin user exists
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var createResult = await userManager.CreateAsync(adminUser, adminPassword);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(" | ", createResult.Errors.Select(e => e.Description));
            throw new Exception("Admin user creation failed: " + errors);
        }
    }

    // 3) Ensure admin user is in Admin role
    if (!await userManager.IsInRoleAsync(adminUser, adminRole))
    {
        var addRoleResult = await userManager.AddToRoleAsync(adminUser, adminRole);
        if (!addRoleResult.Succeeded)
        {
            var errors = string.Join(" | ", addRoleResult.Errors.Select(e => e.Description));
            throw new Exception("Adding Admin user to role failed: " + errors);
        }
    }
}

// =========================
// Pipeline
// =========================
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// ✅ Session must be BEFORE routing endpoints get used
app.UseSession();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
