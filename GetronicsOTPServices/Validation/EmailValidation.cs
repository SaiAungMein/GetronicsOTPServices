using GetronicsOTPServices.Configs;
using GetronicsOTPServices.Controllers;
using GetronicsOTPServices.Interfaces;
using log4net;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace GetronicsOTPServices.Validation
{
    public class EmailValidation : IEmailValidation
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(EmailValidation));
        private readonly EmailConfig _emailConfig;
        public EmailValidation(EmailConfig emailConfig)
        {
            _emailConfig = emailConfig;
        }
        public bool VerifyEmail(string email)
        {
            // Use regular expression pattern for email validation
            const string emailPattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            
            if(Regex.IsMatch(email, emailPattern))
            {
                return VerifyDomain(email);
            }

            _log.Warn($"Email is not valid {email}");

            return false;
        }

        private bool VerifyDomain(string email)
        {
            // get domain from email
            string emailDomain = email.Substring(email.IndexOf('@'));

            var validDomains = _emailConfig.AcceptDomains
                .Split(',')
                .Select(domain => domain.Trim())
                .ToList();

            // Just in case, if we want to accept mulitple domain
            foreach (string validDomain in validDomains)
            {
                if (emailDomain.Equals(validDomain, StringComparison.OrdinalIgnoreCase)) // email domain valid 
                {
                    return true; 
                }
            }

            _log.Warn($"Email domain is not accept {email}");

            return false; // Email domain is not valid
        }
    }
}
