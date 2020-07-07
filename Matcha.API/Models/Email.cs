using System.Collections.Generic;

namespace Matcha.API.Models
{
    public class MailUser
    {
        public string Name;
        public string Email;
    }
    public class Email
    {
        public List<MailUser> To;
        public List<MailUser> Cc = new List<MailUser>();
        public List<MailUser> Bcc = new List<MailUser>();
        public MailUser ReplyTo;
        public string HTML;
        public string Subject;
    }
}
