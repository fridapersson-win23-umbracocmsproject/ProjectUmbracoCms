using Microsoft.AspNetCore.Mvc;
using ProjectUmbracoCms.Helpers;
using ProjectUmbracoCms.Services;
using ProjectUmbracoCms.ViewModels;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Website.Controllers;

namespace ProjectUmbracoCms.Controllers;

public class SupportFormSurfaceController : SurfaceController
{
	private readonly ServiceBusEmailService _servicebusEmailService;
	public SupportFormSurfaceController(IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory, ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger, IPublishedUrlProvider publishedUrlProvider, ServiceBusEmailService servicebusEmailService) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
	{
		_servicebusEmailService = servicebusEmailService;
	}

	[HttpPost]
	public async Task<IActionResult> HandleSupportFormSubmit(SupportFormModel form)
	{
		if (!FormValidatorHelper.IsValidForm(form, x => FormValidatorHelper.IsValidEmail(x.Email)))
		{
			TempData["support_email"] = form.Email;

			TempData["support_error_email"] = !FormValidatorHelper.IsValidEmail(form.Email);


			return CurrentUmbracoPage();
		}
		var emailRequest = new EmailRequest
		{
			To = form.Email,
			Subject = "Thank you for your support request",
			HtmlBody = $"<h1>Hi</h1><p>Thank you for your support request, we will reach out to you asap, we will contact you on {form.Email}</p>",
			PlainText = $"Hi! Thank you for your support request, we will reach out to you asap"
		};

		await _servicebusEmailService.PublishSupportAsync(form);

        TempData.Clear();
        ViewData["support_success"] = "Your request for online support has been submitted successfully.";
		return CurrentUmbracoPage();

	}

  //  private bool IsValidEmail(string email)
  //  {
  //      try
  //      {
  //          if (string.IsNullOrWhiteSpace(email))
  //          {
  //              return false;
  //          }

  //          var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]{2,}$");
  //          return regex.IsMatch(email);
  //      }
  //      catch (Exception ex)
  //      {

  //      }
  //      return false;
  //  }

  //  private bool IsValidForm(SupportFormModel form)
  //  {
		//return IsValidEmail(form.Email);
  //  }
}
