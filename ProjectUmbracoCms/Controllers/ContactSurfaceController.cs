using Microsoft.AspNetCore.Mvc;
using ProjectUmbracoCms.Models;
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
	public ContactSurfaceController(IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory, ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger, IPublishedUrlProvider publishedUrlProvider) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
	{
	}

	public IActionResult HandleSubmit(ContactFormModel form)
	{
		if (!ModelState.IsValid) 
		{
			TempData["name"] = form.Name;
			TempData["email"] = form.Email;
			TempData["message"] = form.Message;

			TempData["error_name"] = string.IsNullOrEmpty(form.Name);
			TempData["error_email"] = string.IsNullOrEmpty(form.Email);
			TempData["error_message"] = string.IsNullOrEmpty(form.Message);

			return CurrentUmbracoPage();
		}

		ViewData["success"] = "Form submittet successfully!";
		return CurrentUmbracoPage();

	}
}
