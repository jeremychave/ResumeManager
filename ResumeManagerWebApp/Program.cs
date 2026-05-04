using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;
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

var forwardOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto
};

// très important : on vide les listes, sinon le middleware ignore Azure
forwardOptions.KnownIPNetworks.Clear();
forwardOptions.KnownProxies.Clear();

app.UseForwardedHeaders(forwardOptions);

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.UseRouting();

app.MapDefaultControllerRoute();

app.Run();
