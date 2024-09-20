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



builder.Services.AddSingleton<ServiceBusClient>(provider =>
{
	var config = provider.GetRequiredService<IConfiguration>();
	var connectionString = config.GetConnectionString("ServiceBusConnection");
	return new ServiceBusClient(connectionString);  
});

builder.Services.AddSingleton<ServiceBusEmailService>(provider =>
{
	var serviceBusClient = provider.GetRequiredService<ServiceBusClient>(); 
	var logger = provider.GetRequiredService<ILogger<ServiceBusEmailService>>();
	var queueName = "email_request";  

	return new ServiceBusEmailService(serviceBusClient, queueName, logger);
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
