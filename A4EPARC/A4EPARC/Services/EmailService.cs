using System.IO;
using System.Net.Mail;

namespace A4EPARC.Services
{
    public class EmailService : IEmailService
    {
        public void Send(string fromEmail, string toEmail, string subject, string body)
        {
            var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            var client = new SmtpClient();
            client.Send(message);
        }

        public void SendWithAttachment(string fromEmail, string toEmail, string subject, string body, MemoryStream stream, string attachment)
        {
            var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            var data = new Attachment(new MemoryStream(stream.ToArray()), attachment);

            var disposition = data.ContentDisposition;
            disposition.CreationDate = File.GetCreationTime(attachment);
            disposition.ModificationDate = File.GetLastWriteTime(attachment);
            disposition.ReadDate = File.GetLastAccessTime(attachment);

            message.Attachments.Add(data);

            var client = new SmtpClient();
            client.Send(message);
        }
    }

    public interface IEmailService
    {
        void Send(string fromEmail, string toEmail, string subject, string body);

        void SendWithAttachment(string fromEmail, string toEmail, string subject, string body, MemoryStream stream,
                                string attachment);
    }
}