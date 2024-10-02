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

public class ServiceDetailsFormSurfaceController : SurfaceController
{
    private readonly ServiceBusEmailService _serviceBusEmailService;
    public ServiceDetailsFormSurfaceController(IUmbracoContextAccessor umbracoContextAccessor, IUmbracoDatabaseFactory databaseFactory, ServiceContext services, AppCaches appCaches, IProfilingLogger profilingLogger, IPublishedUrlProvider publishedUrlProvider, ServiceBusEmailService serviceBusEmailService) : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
    {
        _serviceBusEmailService = serviceBusEmailService;
    }

    [HttpPost]
    public async Task<IActionResult> HandleServiceFormSubmit(ServiceDetailsFormModel form)
    {
        try
        {
            if (!FormValidatorHelper.IsValidForm(form, x =>
                !string.IsNullOrEmpty(x.Name) && 
                FormValidatorHelper.IsValidEmail(x.Email) && 
                !string.IsNullOrEmpty(x.Message)))
            {
                TempData["service_name"] = form.Name;
                TempData["service_email"] = form.Email;
                TempData["service_message"] = form.Message;

                TempData["service_error_name"] = string.IsNullOrEmpty(form.Name);
                TempData["service_error_email"] = !FormValidatorHelper.IsValidEmail(form.Email);
                TempData["service_error_message"] = string.IsNullOrEmpty(form.Message);

                return CurrentUmbracoPage();
            }

            await _serviceBusEmailService.PublishServiceDetailsAsync(form);

            TempData.Clear();
            TempData["service_success"] = "Form submitted successfully!";
            return CurrentUmbracoPage();

        }
        catch (Exception ex) { }
        return null!;
    }
}
