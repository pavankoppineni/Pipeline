// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pipeline.Shell.Pipelines;


var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<EmployeeBackgroundService>()
    .AddHostedService<LeaveRequestBackgroundService>();

IHost host = builder.Build();
host.Run();
