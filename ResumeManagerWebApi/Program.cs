using Microsoft.EntityFrameworkCore;
using ResumeManagerWebApi.Data;
using ResumeManagerWebApi.Data.Repositories;
using ResumeManagerWebApi.Services.Documents;
using ResumeManagerWebApi.Services.User;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ResumeManagerDbContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("ResumeManagerDb"));
    options.AddInterceptors(new AccessTokenInterceptor());
});

builder.Services.AddControllers();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDocumentsService, DocumentsService>();
builder.Services.AddScoped<IDocumentsValidationService, DocumentsValidationService>();
builder.Services.AddScoped<IDocumentsRepository, DocumentsRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
