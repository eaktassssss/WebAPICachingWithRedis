using Microsoft.EntityFrameworkCore;
using WebAPICachingWithRedis.Application.Implementations;
using WebAPICachingWithRedis.Application.Interfaces;
using WebAPICachingWithRedis.Configurations;
using WebAPICachingWithRedis.Persistance.Context;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(sql => sql.UseSqlServer(builder.Configuration.GetConnectionString("ConString")));
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.Configure<RedisConfiguration>(builder.Configuration.GetSection("Redis"));
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
