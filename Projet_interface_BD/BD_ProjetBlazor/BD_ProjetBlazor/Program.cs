using BD_ProjetBlazor.Components;
using BD_ProjetBlazor.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var constrBuilder =
 new SqlConnectionStringBuilder(builder.Configuration.GetConnectionString("defaultConnection"));
constrBuilder.Password = builder.Configuration["MDP"];
builder.Services.AddPooledDbContextFactory<ProgA25BdProjetProgContext>(
    x => x.UseSqlServer(constrBuilder.ConnectionString));
// Add services to the container.
    builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseMigrationsEndPoint();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
