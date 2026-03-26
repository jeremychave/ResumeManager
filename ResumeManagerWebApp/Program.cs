using Azure.Identity;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using ResumeManagerWebApp.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

var resumeManagerUrl = builder.Configuration["ApiSettings:ResumeManagerWebApi"];

builder.Services.AddHttpClient<DocumentApiService>(client =>
{
    client.BaseAddress = new Uri(resumeManagerUrl);
});

builder.Services
    .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration);

var keyVaultUrl = builder.Configuration["KeyVaultUrl"];
builder.Configuration.AddAzureKeyVault(
    new Uri(keyVaultUrl),
    new DefaultAzureCredential()
);

var app = builder.Build();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();
