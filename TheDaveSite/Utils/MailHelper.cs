using SendGridMail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace TheDaveSite.Utils
{
    public class MailHelper
    {
        public static void SendSimpleMail(string subject, string message, string[] recipients){
            System.Configuration.Configuration rootWebConfig =
System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/");

            var user = (String)rootWebConfig.AppSettings.Settings["mailUser"].Value;
            var password = (String)rootWebConfig.AppSettings.Settings["mailPassword"].Value;
            var mailCredentials = new NetworkCredential(user, password);
            var adminMailAddress = (String)rootWebConfig.AppSettings.Settings["adminMailAddress"].Value;

            var from = new MailAddress(adminMailAddress);
            var to = recipients.Select(x => new MailAddress(x)).ToArray();

            // Create an email, passing in the the eight properties as arguments.
            SendGrid myMessage = SendGrid.GetInstance(from, to, new MailAddress[0], new MailAddress[0], subject, "", message);

            // Create an SMTP transport for sending email.
            Web.GetInstance(mailCredentials).Deliver(myMessage);
        }

        public static void SendSimpleAdminMail(string subject, string message)
        {
            System.Configuration.Configuration rootWebConfig =
System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("/");

            var user = (String)rootWebConfig.AppSettings.Settings["mailUser"].Value;
            var password = (String)rootWebConfig.AppSettings.Settings["mailPassword"].Value;
            var mailCredentials = new NetworkCredential(user, password);
            var adminMailAddress = (String)rootWebConfig.AppSettings.Settings["adminMailAddress"].Value;

            var from = new MailAddress(adminMailAddress);
            var to = new MailAddress[] { new MailAddress(adminMailAddress) };

            // Create an email, passing in the the eight properties as arguments.
            SendGrid myMessage = SendGrid.GetInstance(from, to, new MailAddress[0], new MailAddress[0], subject, "", message);

            // Create an SMTP transport for sending email.
            Web.GetInstance(mailCredentials).Deliver(myMessage);
        }
    }
}