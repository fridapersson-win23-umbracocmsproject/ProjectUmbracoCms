using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;
using ProjectUmbracoCms.Models;

namespace ProjectUmbracoCms.Services;

public class ServiceBusEmailService
{
	private readonly ServiceBusClient _serviceBusClient;
	private readonly ServiceBusSender _serviceBusSender;
	private readonly ILogger<ServiceBusEmailService> _logger;


	public ServiceBusEmailService(ServiceBusClient serviceBusClient, string queueName, ILogger<ServiceBusEmailService> logger)
	{
		_serviceBusClient = serviceBusClient;  
		_serviceBusSender = _serviceBusClient.CreateSender(queueName); 
		_logger = logger;
	}

	public async Task PublishAsync(EmailRequest form)
	{
		try
		{
			string messageBody = JsonConvert.SerializeObject(form);
			var message = new ServiceBusMessage(messageBody);

			await _serviceBusSender.SendMessageAsync(message);
		}
		catch (Exception ex)
		{
			_logger.LogError($"ERROR : ServiceBusEmailService.PublishAsync() :: {ex.Message} ");
		}
	}

}
