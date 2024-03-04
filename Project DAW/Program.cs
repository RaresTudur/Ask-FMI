using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project_DAW.Data;
using Project_DAW.Models;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();


/// pentru a verifica daca userul are valorile true pe coloanelele Moderator,Admitere,Licenta,Master
/// 

builder.Services.AddAuthorization(optiuni =>
{
    optiuni.AddPolicy("Moderator", policy =>
    {
        policy.RequireAssertion(context =>
        {
            var userid = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            return true;
        });
    });
}
    );
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Categorii}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
