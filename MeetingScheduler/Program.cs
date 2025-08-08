using MeetingScheduler.Application.Interfaces;
using MeetingScheduler.Application.Services;
using MeetingScheduler.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IMeetingRepository, MeetingRepository>();
builder.Services.AddSingleton<MeetingSchedulerService>();
builder.Services.AddSingleton<UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
