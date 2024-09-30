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
            if (!IsValidForm(form))
            {
                TempData["service_name"] = form.Name;
                TempData["service_email"] = form.Email;
                TempData["service_message"] = form.Message;

                TempData["service_error_name"] = string.IsNullOrEmpty(form.Name);
                TempData["service_error_email"] = !IsValidEmail(form.Email);
                TempData["service_error_message"] = string.IsNullOrEmpty(form.Message);

                return CurrentUmbracoPage();
            }

            //var emailRequest = new EmailRequest
            //{
            //    To = form.Email,
            //    Subject = "Thank you for your question",
            //    HtmlBody = $"<h1>Thank you for your message, {form.Name}!</h1> <br /> <p>We have recieved your message and we will contact you asap</p>",
            //    PlainText = $"Thank you for your message, {form.Name}! We have recieved your message and we will contact you asap"
            //};

            //await _serviceBusEmailService.PublishAsync(emailRequest);
            await _serviceBusEmailService.PublishServiceDetailsAsync(form);

            TempData.Clear();
            TempData["service_success"] = "Form submitted successfully!";
            return CurrentUmbracoPage();

        }
        catch (Exception ex) { }
        return null!;
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
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

    private bool IsValidForm(ServiceDetailsFormModel form)
    {
        return !string.IsNullOrEmpty(form.Name) &&
               IsValidEmail(form.Email) &&
               !string.IsNullOrEmpty(form.Message);
    }
}
