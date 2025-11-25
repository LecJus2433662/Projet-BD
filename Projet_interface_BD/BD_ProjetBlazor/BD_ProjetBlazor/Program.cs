using BD_ProjetBlazor.Authentication;
using BD_ProjetBlazor.Components;
using BD_ProjetBlazor.Data;
using BD_ProjetBlazor.Partials;
using BD_ProjetBlazor.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var constrBuilder =
 new SqlConnectionStringBuilder(builder.Configuration.GetConnectionString("defaultConnection"));
constrBuilder.Password = builder.Configuration["MDP"];
builder.Services.AddPooledDbContextFactory<ProgA25BdProjetProgContext>(
    x => x.UseSqlServer(constrBuilder.ConnectionString));
builder.Services.AddScoped<Requete_Info_mensuelles>();
builder.Services.AddScoped<Requete_EntreeSortieService>();
builder.Services.AddScoped<Requete_inscriptions>();
builder.Services.AddScoped<Requete_Connexion>();
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<Requete_Reservation>();
builder.Services.AddScoped<LoginForm>();
builder.Services.AddScoped<ReservationForm>();
//builder.Services.AddScoped<UserAccountService>();
builder.Services.AddAuthenticationCore();
builder.Services.AddAuthorizationCore();
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
