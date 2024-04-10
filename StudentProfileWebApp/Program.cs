using Microsoft.Net.Http.Headers;
using StudentProfileWebApp;
using System.Net.Http.Headers;

DotEnvReader reader = new DotEnvReader();
bool dotEnvOk = reader.LoadEnv();
if (!dotEnvOk)
{
    Console.WriteLine("Env file not found or contains bad syntax. Ignoring...");
}

var builder = WebApplication.CreateBuilder(args);

string? listenProtocol = Environment.GetEnvironmentVariable("LISTEN_PROTOCOL");
string? listenAddress = Environment.GetEnvironmentVariable("LISTEN_ADDRESS");
string? listenPort = Environment.GetEnvironmentVariable("LISTEN_PORT");

listenProtocol ??= "http";
listenAddress ??= "127.0.0.1";
listenPort ??= "5000";

Console.WriteLine(string.Format("Listening on {0}:{1} using {2}", listenAddress, listenPort, listenProtocol));
builder.WebHost.UseUrls(string.Format("{0}://{1}:{2}", listenProtocol, listenAddress, listenPort));

// Add services to the container.
builder.Services.AddControllersWithViews();

string? apiBaseUrl = Environment.GetEnvironmentVariable("STUDENT_PROFILE_API_BASE_URL");
apiBaseUrl ??= "http://127.0.0.1:3000";

builder.Services.AddHttpClient("StudentProfilesWebApi",
    (options) =>
    {
        options.BaseAddress = new Uri(apiBaseUrl);
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
