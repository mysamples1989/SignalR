using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SignalRDemo.Data;
using SignalRDemo.Hubs;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

//var connectionAzureSignalR = "Endpoint=https://dotnetmastery.service.signalr.net;AccessKey=m9Enl0s8dqQGU6l0M0SGDqjwMttKvqN84hX+acKepmU=;Version=1.0;";

//builder.Services.AddSignalR().AddAzureSignalR(connectionAzureSignalR);
builder.Services.AddSignalR();

var app = builder.Build();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.MapHub<UserHub>("/hubs/userCount");

ApplyMigration();

app.Run();


void ApplyMigration()
{
        using (var scope = app.Services.CreateScope())
        {
                var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                        _db.Database.Migrate();
                }
        }
}