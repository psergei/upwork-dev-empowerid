using CommentsService.Controllers;
using CommentsService.Repositories;
using CommentsService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApiDbContext>(options =>
{
  var connectionString = builder.Configuration.GetConnectionString("Database");
  options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<ICommentsRepository, CommentsRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<QueueService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapCommentsControllerEndpoints();

app.Run();

