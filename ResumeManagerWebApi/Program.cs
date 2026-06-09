using Microsoft.EntityFrameworkCore;
using ResumeManagerWebApi.Data;
using ResumeManagerWebApi.Data.Repositories;
using ResumeManagerWebApi.Services.Documents;
using ResumeManagerWebApi.Services.User;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ResumeManagerDbContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("ResumeManagerDb"));
});

builder.Services.AddControllers();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDocumentsService, DocumentsService>();
builder.Services.AddScoped<IDocumentsValidationService, DocumentsValidationService>();
builder.Services.AddScoped<IDocumentsRepository, DocumentsRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Execute migration at startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ResumeManagerDbContext>();

    try
    {
        db.Database.Migrate();
        Console.WriteLine("Migrations applied successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error occurred while applying migrations: " + ex.Message);
        throw;
    }
}

//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
