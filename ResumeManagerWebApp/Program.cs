using ResumeManagerWebApp.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

var resumeManagerUrl = builder.Configuration["ApiSettings:ResumeManagerWebApi"];

builder.Services.AddHttpClient<ProductApiService>(client =>
{
    client.BaseAddress = new Uri(resumeManagerUrl);
});

builder.Services.AddHttpClient<ResumeApiService>(client =>
{
    client.BaseAddress = new Uri(resumeManagerUrl);
});

var app = builder.Build();
app.UseStaticFiles();
app.MapDefaultControllerRoute();

//app.MapGet("/", () => "Hello World!");

app.Run();
