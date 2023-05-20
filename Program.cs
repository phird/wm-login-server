using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using wm_api.Data;
using wm_api.Services; 
using wm_api.Models;
using MongoDB.Driver;



var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var dbContext = new DbContext(configuration);
// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
    option =>
    {
        option.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // valid the server
            ValidateAudience = true, // valid audoence 
            ValidateLifetime = true, // check expire token 
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    }
);

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// Register IMongoClient
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    // Configure and create the MongoDB client
    var connectionString = builder.Configuration["MongoDB:URI"];
    return new MongoClient(connectionString);
});

// Register IMongoDatabase
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var dbName = builder.Configuration["MongoDB:DatabaseName"];
    return client.GetDatabase(dbName);
});

// Register IMongoCollection<Users>
builder.Services.AddScoped<IMongoCollection<Users>>(sp =>
{
    var database = sp.GetRequiredService<IMongoDatabase>();
    var collectionsName = builder.Configuration["MongoDB:CollectionName"];
    return database.GetCollection<Users>(collectionsName);
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<DbContext>();


builder.Services.AddControllers();
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
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
