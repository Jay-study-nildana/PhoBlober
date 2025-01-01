using Microsoft.Extensions.Configuration;
using PhoBloberWebAPI.Services;
using PhoBloberWebAPI.Services.IServices;
using PhoBloberWebAPI.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//builder.Services.AddScoped<IMessageBus, MessageBus>();
builder.Services.AddScoped<IBlobStorageStuff,BlobStorageStuff>();
builder.Services.AddScoped<IComputerVisionStuff,ComputerVisionStuff>();
builder.Services.AddScoped<ITranslatorStuff, TranslatorStuff>();
//load up the Translator Settings
builder.Services.Configure<TranslatorSettings>(
    builder.Configuration.GetSection("TranslatorSettings"));
//load up the Translator Settings
builder.Services.Configure<CVSettings>(
    builder.Configuration.GetSection("CVSettings"));
//load up the Storage Settings
builder.Services.Configure<StorageSettings>(
    builder.Configuration.GetSection("StorageSettings"));
// Register custom service that loads up the translator service
builder.Services.AddTransient<TranslatorSettingsService>();
// Register custom service that loads up the CV service
builder.Services.AddTransient<CVSettingsService>();
// Register custom service that loads up the Storage Settings Service
builder.Services.AddTransient<StorageSettingsService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
