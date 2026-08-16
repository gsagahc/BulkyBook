using BulkyBook.Business.Services;
using BulkyBook.Business.Services.IServices;
using BulkyBook.DataAccess.Data ;
using BulkyBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register MySQL Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    { options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;              // Must have at least one number (0-9)
    options.Password.RequireLowercase = true;          // Must have at least one lowercase letter (a-z)
    options.Password.RequireUppercase = true;          // Must have at least one uppercase letter (A-Z)
    options.Password.RequireNonAlphanumeric = true;    // Must have at least one special character (!@#$%^&*)
    options.Password.RequiredLength = 7;               // Minimum 7 characters
    options.Password.RequiredUniqueChars = 1; })          // Minimum unique characters
  .AddEntityFrameworkStores<ApplicationDbContext>();
 
builder.Services.AddScoped<ICategoryService, CategoryService>();    
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IApplicationUserService, ApplicationUserService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "MyArea",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}",
    defaults: new { area = "Costumer" })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "Admin",
    pattern: "{controller=Home}/{action=Index}/{id?}",
    defaults: new { area = "Admin" })
    .WithStaticAssets();




app.Run();
