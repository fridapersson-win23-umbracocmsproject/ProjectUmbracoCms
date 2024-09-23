using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using ProjectUmbracoCms.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddDeliveryApi()
    .AddComposers()
    .AddAzureBlobMediaFileSystem()
    .AddAzureBlobImageSharpCache()
    .Build();


builder.Services.AddSingleton<ServiceBusClient>(serviceProvider =>
{
	var config = serviceProvider.GetRequiredService<IConfiguration>();
	var connectionString = config.GetConnectionString("ServiceBusConnection");
	return new ServiceBusClient(connectionString);
});

builder.Services.AddTransient<ServiceBusEmailService>(serviceProvider =>
{
	var client = serviceProvider.GetRequiredService<ServiceBusClient>();
	var config = serviceProvider.GetRequiredService<IConfiguration>();
	var queueName = config["ServiceBus:QueueName"];
	var logger = serviceProvider.GetRequiredService<ILogger<ServiceBusEmailService>>();
	return new ServiceBusEmailService(client, queueName, logger);
});



WebApplication app = builder.Build();





await app.BootUmbracoAsync();

app.UseHttpsRedirection();

app.UseUmbraco()
    .WithMiddleware(u =>
    {
        u.UseBackOffice();
        u.UseWebsite();
    })
    .WithEndpoints(u =>
    {
        u.UseBackOfficeEndpoints();
        u.UseWebsiteEndpoints();
    });

await app.RunAsync();
