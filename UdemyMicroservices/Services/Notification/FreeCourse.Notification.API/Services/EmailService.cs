using FreeCourse.Notification.API.Entities.Email;
using FreeCourse.Notification.API.Services.Interfaces;
using FreeCourse.Shared.Dtos;
using FreeCourse.Shared.Messages;
using MailKit.Net.Smtp;
using MimeKit;

namespace FreeCourse.Notification.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfig;
        public EmailService(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }
        public async Task<Response<EmailMessage>> SendEmailAsync(EmailMessage message)
        {
            var emailMessage = CreateEmailMessage(message);
            var response = await SendAsync(emailMessage);
            if (response.Data == null || (response.Erros != null && response.Erros.Any()))
                return Response<EmailMessage>.Fail(response.Erros, 404);

            return Response<EmailMessage>.Success(message, 200);
        }

        private MimeMessage CreateEmailMessage(EmailMessage message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.UserName, _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;

            var builder = new BodyBuilder { HtmlBody = message.Content };

            if (message.Attachments != null && message.Attachments.Any())
            {
                byte[] fileBytes;
                foreach (var attachment in message.Attachments)
                {
                    using (var ms = new MemoryStream())
                    {
                        attachment.CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }
                    builder.Attachments.Add(attachment.FileName, fileBytes, ContentType.Parse(attachment.ContentType));
                }
            }

            emailMessage.Body = builder.ToMessageBody();
            return emailMessage;
        }
        private async Task<Response<MimeMessage>> SendAsync(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);
                    await client.SendAsync(mailMessage);

                    return Response<MimeMessage>.Success(mailMessage, 200);
                }
                catch (SmtpCommandException smtpCommandException)
                { return Response<MimeMessage>.Fail($"Smtp error : {smtpCommandException.Message}", 400); }
                catch (SmtpProtocolException smtpProtocolException)
                { return Response<MimeMessage>.Fail($"Smtp error : {smtpProtocolException.Message}", 400); }
                catch (System.Net.Mail.SmtpFailedRecipientException smtpFailedRecipientException)
                { return Response<MimeMessage>.Fail($"Smtp error : {smtpFailedRecipientException.Message}", 400); }
                catch (System.Net.Mail.SmtpException smtpException)
                { return Response<MimeMessage>.Fail($"Smtp error : {smtpException.Message}", 400); }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }
    }
}
