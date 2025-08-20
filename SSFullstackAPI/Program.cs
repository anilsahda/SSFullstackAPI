using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using SSFullstackAPI.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon")));
//builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(MySqlConnection, ServerVersion.AutoDetect(MySqlCon)));
//builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(PostgresCon));

builder.Services.AddSingleton<IMongoClient>(sp => { return new MongoClient(builder.Configuration.GetConnectionString("MongoCon")); });
builder.Services.AddSingleton(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(builder.Configuration.GetValue<string>("MongoDatabase"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
