using Microsoft.AspNetCore.Mvc;
using ProjectUmbracoCms.Models;
using ProjectUmbracoCms.Services;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;

namespace ProjectUmbracoCms.Controllers;

public class ContactSurfaceController : SurfaceController
{
	private readonly ServiceBusEmailService _emailService;
	public ContactSurfaceController(IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory, ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger, IPublishedUrlProvider publishedUrlProvider, ServiceBusEmailService emailService) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
	{
		_emailService = emailService;
	}

	[HttpPost]
	public async Task<IActionResult> HandleSubmit(ContactFormModel form)
	{
		if (!ModelState.IsValid)
		{
			TempData["name"] = form.Name;
			TempData["email"] = form.Email;
			TempData["phone"] = form.Phone;
			TempData["selectedOption"] = form.SelectedOption;

			TempData["error_name"] = string.IsNullOrEmpty(form.Name);
			TempData["error_email"] = string.IsNullOrEmpty(form.Email);
			TempData["error_phone"] = string.IsNullOrEmpty(form.Phone);
			TempData["error_selectedOption"] = string.IsNullOrEmpty(form.SelectedOption);

			return CurrentUmbracoPage();
		}

		var emailRequest = new EmailRequest
		{
			To = form.Email,
			Subject = "Thank you for your message",
			HtmlBody = $"<h1>Hi {form.Name}</h1><p>Thank you for your message, we will contact you asap about {form.SelectedOption}</p>",
			PlainText = $"Hi {form.Name}, Thank you for your message, we will contact you asap",
		};

		await _emailService.PublishAsync(emailRequest);

		ViewData["success"] = "Form submitted successfully!";
		return CurrentUmbracoPage();
	}


	[HttpPost]
	public async Task<IActionResult> HandleSupportFormSubmit(SupportFormModel supportForm)
	{
		if (!ModelState.IsValid)
		{
			TempData["email"] = supportForm.Email;

			TempData["error_email"] = string.IsNullOrEmpty(supportForm.Email);


			return CurrentUmbracoPage();
		}
		var emailRequest = new EmailRequest
		{
			To = supportForm.Email,
			Subject = "Thank you for your support request",
			HtmlBody = $"<h1>Hi</h1><p>Thank you for your support request, we will reach out to you asap, we will contact you on {supportForm.Email}</p>",
			PlainText = $"Hi! Thank you for your support request, we will reach out to you asap"
		};

		await _emailService.PublishAsync(emailRequest);

		ViewData["success"] = "Your request for online support has been submitted successfully.";
		return CurrentUmbracoPage();

	}
}
