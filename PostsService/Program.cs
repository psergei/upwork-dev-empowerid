using Microsoft.EntityFrameworkCore;
using PostsService.Controllers;
using PostsService.Repositories;
using PostsService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApiDbContext>(options =>
{
  var connectionString = builder.Configuration.GetConnectionString("Database");
  options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<IPostsRepository, PostsRepository>();
builder.Services.AddScoped<IServiceBus, ServiceBus>();

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

app.MapPostsControllerEndpoints();

app.Run();

