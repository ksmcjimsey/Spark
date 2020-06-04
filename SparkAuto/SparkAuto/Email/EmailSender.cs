using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SparkAuto.Email
{
    public class EmailSender : IEmailSender
    {
        // This is specific to the SendGrid
        public EmailOptions Options { get; set; }

        // Constructor
        // Dependency inject the SendGrid key into the EmailOptions
        // We pass in the emailOptions through dependency injection (preavailble values)
        // Set the value in startup.cs
        public EmailSender (IOptions<EmailOptions> emailOptions)
        {
            // Take the injected value and set it to the local copy
            Options = emailOptions.Value;
        }

        // IEmailSender is the interface and requires this method
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Get key then create the message
            var client = new SendGridClient(Options.SendGridKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("admin@spark.com", "Spark Auto"),
                Subject = subject,
                PlainTextContent = htmlMessage,
                HtmlContent = htmlMessage
            };
            msg.AddTo(new EmailAddress(email));

            // Now send the message
            try
            {
                return client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {

            }

            // If things go wrong
            return null;
            
        }
    }
}
