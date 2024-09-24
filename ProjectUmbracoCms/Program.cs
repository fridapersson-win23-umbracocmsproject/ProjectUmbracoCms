using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using ProjectUmbracoCms.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.ConfigureKeyVault();

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
	var connectionString = config["ServiceBusConnection"]; 
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




public static class WebApplicationBuilderExtensions
{
	public static WebApplicationBuilder ConfigureKeyVault(this WebApplicationBuilder builder)
	{
		var keyVaultEndpoint = builder.Configuration["AzureKeyVaultEndpoint"];
		if (!string.IsNullOrWhiteSpace(keyVaultEndpoint) && Uri.TryCreate(keyVaultEndpoint, UriKind.Absolute, out var validUri))
		{
			// Lägg till Key Vault i konfigurationen
			builder.Configuration.AddAzureKeyVault(validUri, new DefaultAzureCredential());
			Console.WriteLine($"Connected to Key Vault: {keyVaultEndpoint}");
		}
		else
		{
			Console.WriteLine("Key Vault URI is invalid or missing.");
		}

		return builder;
	}
}