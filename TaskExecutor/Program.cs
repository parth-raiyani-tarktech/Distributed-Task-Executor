using Quartz.Spi;
using TaskExecutor.Repositories;
using TaskExecutor.Repositories.Interface;
using TaskExecutor.Services;
using TaskExecutor.Services.Interface;
using TaskExecutor.Services.Job;
using TaskExecutor.Services.Scheduler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(option => option.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<NodeHealthCheckerJob>();

builder.Services.AddSingleton<IJobFactory, MyJobFactory>();
builder.Services.AddSingleton<IMyScheduler, MyScheduler>();

builder.Services.AddSingleton<INodeManager, NodeManager>();
builder.Services.AddSingleton<ITaskManager, TaskManager>();
builder.Services.AddSingleton<ITaskOrchestrator, TaskOrchestrator>();

builder.Services.AddSingleton<INodeRepository, NodeRepository>();
builder.Services.AddSingleton<ITaskRepository, TaskRepository>();

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