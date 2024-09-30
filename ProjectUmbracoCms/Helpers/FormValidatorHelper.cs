using System.Text.RegularExpressions;

namespace ProjectUmbracoCms.Helpers;

public static class FormValidatorHelper
{
    /// <summary>
    ///     Validates if the provided email is in a coorect format.
    /// </summary>
    /// <param name="email">the provided email to validate</param>
    /// <returns>True if the email is valid, else false</returns>
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]{2,}$");
        return regex.IsMatch(email);
    }

    /// <summary>
    ///     Validates a form of a specified type using custom validation function
    /// </summary>
    /// <typeparam name="T">The type of the form model to validate</typeparam>
    /// <param name="form">The form model to validate</param>
    /// <param name="validate">Function that takes the form model and returns a true/false value if the form is valid</param>
    /// <returns>True if the for passes the validation function, else false</returns>
    public static bool IsValidForm<T>(T form, Func<T, bool> validate)
    {
        return validate(form);
    }
}
