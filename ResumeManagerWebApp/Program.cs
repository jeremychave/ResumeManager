using ResumeManagerWebApp.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<ProductApiService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5000/");
});
var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

app.Run();
