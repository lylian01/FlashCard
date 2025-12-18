using FlashCard.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext
builder.Services.AddDbContext<FlashcardDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FlashcardConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Users/Login";           // Trang login
        options.LogoutPath = "/Home/Index";         // Trang logout
        options.AccessDeniedPath = "/Shared/Error";   // Khi bị chặn
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.None; // ⚠️ Cho phép chạy cả HTTP & HTTPS
        //options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Chỉ chạy trên HTTPS
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.Name = "FlashCardAuth"; // đặt tên cookie riêng
    });


var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Shared/Error");
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
    pattern: "{controller=Home}/{action=New}/{id?}");

app.Run();
