using System.Collections.Generic;
using System.Threading.Tasks;
using Matcha.API.Models;
using CInterfaces = Coravel.Mailer.Mail.Interfaces;

namespace Matcha.API.Helpers
{
    public interface IMailer
    {
        Task SendMail(Mailable mailable);

        Task SendVerificationMail(MailUser to, string verifyLink);
        Task SendLikeMail(MailUser to, string likedByName);
        Task SendUnlikeMail(MailUser to, string unlikedByName);
        Task SendViewedMail(MailUser to, string viewedBy);
        Task SendMessagedMail(MailUser to, string fromName, string messageSnippet);
    }

    public class Mailer : IMailer
    {
        private CInterfaces.IMailer _mailer;
        private IMailTemplate _mailTemplate;

        public Mailer(CInterfaces.IMailer mailer, IMailTemplate mailTemplate)
        {
            _mailer = mailer;
            _mailTemplate = mailTemplate;
        }

        public Task SendMail(Mailable mailable) => _mailer.SendAsync(mailable);

        public Task SendVerificationMail(MailUser to, string verifyLink)
        {
            var email = new Email()
            {
                To = new List<MailUser> { to },
                Subject = "Matcha Account Verification",
                HTML = _mailTemplate.GetRegisterTemplate(verifyLink)
            };

            return SendMail(new Mailable(email));
        }

        public Task SendLikeMail(MailUser to, string likedByName)
        {
            var email = new Email()
            {
                To = new List<MailUser> { to },
                Subject = "Matcha Profile Liked",
                HTML = _mailTemplate.GetLikeTemplate(to.Name, likedByName)
            };

            return SendMail(new Mailable(email));
        }

        public Task SendUnlikeMail(MailUser to, string unlikedByName)
        {
            var email = new Email()
            {
                To = new List<MailUser> { to },
                Subject = "Matcha Profile Unliked",
                HTML = _mailTemplate.GetUnlikeTemplate(to.Name, unlikedByName)
            };

            return SendMail(new Mailable(email));
        }

        public Task SendViewedMail(MailUser to, string viewedBy)
        {
            var email = new Email()
            {
                To = new List<MailUser> { to },
                Subject = "Matcha Profile Viewed",
                HTML = _mailTemplate.GetViewedTemplate(to.Name, viewedBy)
            };

            return SendMail(new Mailable(email));
        }

        public Task SendMessagedMail(MailUser to, string fromName, string messageSnippet)
        {
            var email = new Email()
            {
                To = new List<MailUser> { to },
                Subject = "Matcha - New Message",
                HTML = _mailTemplate.GetMessagedTemplate(to.Name, fromName, messageSnippet)
            };

            return SendMail(new Mailable(email));
        }
    }
}
