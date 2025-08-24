using Amazon;
using Amazon.DynamoDBv2;
using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using SSFullstackAPI.Data;

var builder = WebApplication.CreateBuilder(args);
var awsOptions = builder.Configuration.GetSection("AWS");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//sql, mysql, postgresql, oracle
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon")));
//builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(MySqlConnection, ServerVersion.AutoDetect(MySqlCon)));
//builder.Services.AddDbContext<AppDbContext>(options => options.UseOracle(builder.Configuration.GetConnectionString("OracleCon")));
//builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(PostgresCon));

// Mongodb
builder.Services.AddSingleton<IMongoClient>(sp => { return new MongoClient(builder.Configuration.GetConnectionString("MongoCon")); });
builder.Services.AddSingleton(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(builder.Configuration.GetValue<string>("MongoDatabase"));
});

// Register DynamoDB client
builder.Services.AddSingleton<IAmazonDynamoDB>(sp =>
{
    return new AmazonDynamoDBClient(
        awsOptions["AccessKey"],
        awsOptions["SecretKey"],
        RegionEndpoint.GetBySystemName(awsOptions["Region"])
    );
});

// Register S3 client
builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    return new AmazonS3Client(awsOptions["AccessKey"], awsOptions["SecretKey"], RegionEndpoint.GetBySystemName(awsOptions["Region"]));
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

app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
