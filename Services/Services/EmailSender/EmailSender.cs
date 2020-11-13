using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;

namespace Services.Services.EmailSender
{
    public class EmailSender : IEmailSender
    {
        private readonly string host;
        private readonly int port;
        private readonly EmailSenderAddress noReplay;
        private List<EmailSenderAddress> addresses = new List<EmailSenderAddress>();

        public EmailSender(IConfiguration configuration)
        {
            host = configuration.GetValue<string>("emailServer:host");
            port = configuration.GetValue<int>("emailServer:port", 587);

            int addressesCount = configuration.GetSection("emailServer").GetSection("addresses").GetChildren().Count();

            for (int i = 0; i < addressesCount; i++)
            {
                EmailSenderAddress address = new EmailSenderAddress()
                {
                    Name = configuration.GetValue<string>($"emailServer:addresses:{i}:Name"),
                    Address = configuration.GetValue<string>($"emailServer:addresses:{i}:address"),
                    Password = configuration.GetValue<string>($"emailServer:addresses:{i}:password")
                };

                addresses.Add(address);
            }

            noReplay = addresses.FirstOrDefault(x => x.Name.ToLower() == "noreply");
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(noReplay.Address);
            msg.To.Add(new MailAddress(email));
            msg.Subject = subject;
            //msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(message.Body, null, MediaTypeNames.Text.Html));
            msg.Body = htmlMessage;
            msg.IsBodyHtml = true;

            SmtpClient smtpClient = new SmtpClient(host, port);
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(noReplay.Address, noReplay.Password);
            smtpClient.Credentials = credentials;
            smtpClient.EnableSsl = true;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

            return Task.Factory.StartNew(() => smtpClient.Send(msg));
        }
    }



}
