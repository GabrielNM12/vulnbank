using VulnBank.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "Nimbus.Session";
    options.IdleTimeout = TimeSpan.FromHours(1);
});

var app = builder.Build();

app.UseDeveloperExceptionPage();

app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

Db.Initialise();

// Executar apenas localmente, em ambiente isolado.
app.Run();
