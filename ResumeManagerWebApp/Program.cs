using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using ResumeManagerWebApp.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

var resumeManagerUrl = builder.Configuration["ApiSettings:ResumeManagerWebApi"];

builder.Services.AddHttpClient<DocumentApiService>(client =>
{
    client.BaseAddress = new Uri(resumeManagerUrl);
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    var config = builder.Configuration.GetSection("AzureAdB2C");

    options.Authority = $"{config["Instance"]}/{config["Domain"]}/{config["SignUpSignInPolicyId"]}/v2.0";
    options.ClientId = config["ClientId"];
    options.CallbackPath = config["CallbackPath"];
    options.SignedOutCallbackPath = config["SignedOutCallbackPath"];
    options.ResponseType = "id_token";
    options.SaveTokens = true;

    options.Scope.Add("openid");
    options.Scope.Add("profile");

    options.Events = new OpenIdConnectEvents
    {
        OnRemoteFailure = context =>
        {
            if (context.Failure.Message.Contains("AADB2C90118"))
            {
                context.Response.Redirect("/Account/ResetPassword");
                context.HandleResponse();
            }
            return Task.CompletedTask;
        }
    };
});

//builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
//    .AddOpenIdConnect(options =>
//    {
//        builder.Configuration.Bind("AzureAdB2C", options);
//        options.TokenValidationParameters.NameClaimType = "name";
//    });

var app = builder.Build();
app.UseStaticFiles();
app.MapDefaultControllerRoute();

app.Run();
