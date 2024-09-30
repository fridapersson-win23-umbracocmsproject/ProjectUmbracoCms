using Microsoft.AspNetCore.Mvc;
using ProjectUmbracoCms.Models;
using ProjectUmbracoCms.Services;
using System.Text.RegularExpressions;
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

		if (!IsValidForm(form))
		{
			TempData["name"] = form.Name;
			TempData["email"] = form.Email;
			TempData["phone"] = form.Phone;
			TempData["selectedOption"] = form.SelectedOption;

			TempData["error_name"] = string.IsNullOrEmpty(form.Name);
			TempData["error_email"] = !IsValidEmail(form.Email);
			TempData["error_phone"] = string.IsNullOrEmpty(form.Phone);
			TempData["error_selectedOption"] = string.IsNullOrEmpty(form.SelectedOption);

			return CurrentUmbracoPage();
		}



		var emailRequest = new EmailRequest
		{
			To = form.Email,
			Subject = "Thank you for your call back request",
			HtmlBody = $"<h1>Hi {form.Name}</h1><p>Thank you for your message, we will contact you asap about {form.SelectedOption}</p>",
			PlainText = $"Hi {form.Name}, Thank you for your message, we will contact you asap",
		};

		await _emailService.PublishAsync(emailRequest);

		TempData.Clear();
		ViewData["success"] = "Form submitted successfully!";
		return CurrentUmbracoPage();
	}

	private bool IsValidEmail(string email)
	{
		try
		{
			if(string.IsNullOrWhiteSpace(email))
			{
				return false;
			}

			var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]{2,}$");
			return regex.IsMatch(email);
		}
		catch (Exception ex)
		{

		}
		return false;
	}

	private bool IsValidForm(ContactFormModel form)
	{
		return !string.IsNullOrEmpty(form.Name) &&
			   IsValidEmail(form.Email) &&
			   !string.IsNullOrEmpty(form.Phone) &&
			   !string.IsNullOrEmpty(form.SelectedOption);
	}
}


