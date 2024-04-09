using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("https://localhost:5001");

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("StudentProfilesWebApi",
    (options) =>
    {
        options.BaseAddress = new Uri("https://localhost:5002/");
        new MediaTypeWithQualityHeaderValue("application/json", 1.0);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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

app.Run();
