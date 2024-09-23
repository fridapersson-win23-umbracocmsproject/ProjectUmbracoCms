using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ProjectUmbracoCms.Models;

namespace ProjectUmbracoCms.Services;

public class ServiceBusEmailService
{
	private readonly ServiceBusSender _serviceBusSender;
	private readonly ILogger<ServiceBusEmailService> _logger;

	public ServiceBusEmailService(ServiceBusClient serviceBusClient, string queueName, ILogger<ServiceBusEmailService> logger)
	{
		_serviceBusSender = serviceBusClient.CreateSender(queueName);
		_logger = logger;
	}

	public async Task PublishAsync(EmailRequest emailRequest)
	{
		try
		{
			string messageBody = JsonConvert.SerializeObject(emailRequest);
			var message = new ServiceBusMessage(messageBody);

			await _serviceBusSender.SendMessageAsync(message);
			_logger.LogInformation("Email message sent to Service Bus successfully.");
		}
		catch (Exception ex)
		{
			_logger.LogError($"ERROR : ServiceBusEmailService.PublishAsync() :: {ex.Message}");
		}
	}
}