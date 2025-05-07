using JamWav.Infrastructure.Persistence;
using JamWav.Infrastructure.Repositories;
using JamWav.Application.Interfaces;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<JamWavDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("JamWavDb")));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBandRepository, BandRepository>();
builder.Services.AddScoped<IFriendRepository, FriendRepository>();

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

public partial class Program { }