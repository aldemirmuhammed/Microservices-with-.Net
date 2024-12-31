using Microsoft.AspNetCore.Http;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeCourse.Shared.Messages
{
    public class EmailMessage
    {
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public IFormFileCollection Attachments { get; set; }

        public TextFormat TextFormat { get; set; }

        public EmailMessage(IEnumerable<string> to, string subject, string content, IFormFileCollection _attachments = null, TextFormat _textFormat = TextFormat.Html)
        {
            Subject = subject;
            Content = content;
            To = new List<MailboxAddress>();
            if (_textFormat != TextFormat.Html)
            {
                TextFormat = _textFormat;
                To.AddRange(to.Select(x => new MailboxAddress("email", x)));
            }
            else
                To.AddRange(to.Select(x => new MailboxAddress(x, x)));

            if (_attachments != null)
                Attachments = _attachments;
            if (_textFormat != TextFormat.Html)
                TextFormat = _textFormat;
        }

       
    }
}
