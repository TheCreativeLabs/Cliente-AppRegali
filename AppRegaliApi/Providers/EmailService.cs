using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AppRegaliApi.Providers
{
    public static class EmailService
    {

        public enum EmailType
        {
            Registration,
            Recovery
        }
        public  static async Task SendAsync(string EmailTo,string Subject, string Body)
        {
            var apiKey = Properties.Settings.Default.SendGridApiKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("AppRegalo@app.com", "AppRegalo");
            var subject = Subject;
            var to = new EmailAddress(EmailTo, "Example User");
            var plainTextContent = Body;
            var htmlContent = Body;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }

     
    }
}