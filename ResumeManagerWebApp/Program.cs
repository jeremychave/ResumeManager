using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using ResumeManagerWebApp.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

var resumeManagerUrl = builder.Configuration["ApiSettings:ResumeManagerWebApi"];
var resumeManagerHttpClient = "resumeManagerHttpClient";

builder.Services.AddHttpClient(resumeManagerHttpClient, client =>
{
    client.BaseAddress = new Uri(resumeManagerUrl);
});

builder.Services.AddTransient<IDocumentApiService, DocumentApiService>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return new DocumentApiService(factory.CreateClient(resumeManagerHttpClient), builder.Configuration);
});

builder.Services.AddTransient<IUserApiService, UserApiService>(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return new UserApiService(factory.CreateClient(resumeManagerHttpClient));
});

builder.Services
    .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration);

builder.Services.Configure<OpenIdConnectOptions>(
    OpenIdConnectDefaults.AuthenticationScheme,
    options =>
    {
        options.Events = new OpenIdConnectEvents
        {
            OnTokenValidated = async ctx =>
            {
                await ctx.HttpContext.RequestServices
                .GetRequiredService<IUserApiService>()
                .SyncUserAsync(ctx.Principal);
            }
        };
    });

var app = builder.Build();
//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();
