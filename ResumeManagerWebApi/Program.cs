using Azure.Identity;
using ResumeManagerWebApi.Repositories;
using ResumeManagerWebApi.Services.Documents;

var builder = WebApplication.CreateBuilder(args);

var keyVaultUrl = builder.Configuration["KeyVaultUrl"];
builder.Configuration.AddAzureKeyVault(
    new Uri(keyVaultUrl),
    new DefaultAzureCredential()
);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IDocumentsService, DocumentsService>();
builder.Services.AddScoped<IDocumentsValidationService, DocumentsValidationService>();
builder.Services.AddScoped<IDocumentsRepository, DocumentsRepository>();

var app = builder.Build();

//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
