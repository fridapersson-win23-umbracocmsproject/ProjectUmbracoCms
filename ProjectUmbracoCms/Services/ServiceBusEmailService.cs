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

	public async Task PublishServiceDetailsAsync(ServiceDetailsFormModel form)
	{
		try
		{
			var emailRequest = new EmailRequest
			{
				To = form.Email,
				Subject = "Thank you for your question",
				HtmlBody = $"<h1>Thank you for your message, {form.Name}!</h1> <br /> <p>We have recieved your message and we will contact you asap</p>",
				PlainText = $"Thank you for your message, {form.Name}! We have recieved your message and we will contact you asap"
			};

			await PublishAsync(emailRequest);
		}
		catch (Exception ex)
		{
			_logger.LogError($"ERROR : ServiceBusEmailService.PublishAsync() :: {ex.Message}");
		}
	}

	public async Task PublishSupportAsync(SupportFormModel form)
	{
		try
		{
			var emailRequest = new EmailRequest
			{
				To = form.Email,
				Subject = "Thank you for your online support request",
				HtmlBody = $"<h1>Request recieved</h1> <br /> <p>We have recieved your request and our team will contact you asap</p>",
				PlainText = "Request recieved! We have recieved your request and our team will contact you asap"
			};

			await PublishAsync(emailRequest);
		}
		catch (Exception ex)
		{
			_logger.LogError($"ERROR : ServiceBusEmailService.PublishAsync() :: {ex.Message}");
		}
	}
}

