using System.Collections.Generic;
using System.Threading.Tasks;
using Matcha.API.Models;
using Microsoft.Extensions.Logging;
using CInterfaces = Coravel.Mailer.Mail.Interfaces;

namespace Matcha.API.Helpers
{
    public interface IMailer
    {
        Task SendMail(Mailable mailable);

        Task SendVerificationMail(MailUser to, string verifyLink);
        Task SendPasswordResetMail(MailUser to, string resetLink);
        Task SendLikeMail(MailUser to, string likedByName);
        Task SendUnlikeMail(MailUser to, string unlikedByName);
        Task SendViewedMail(MailUser to, string viewedBy);
        Task SendMessagedMail(MailUser to, string fromName, string messageSnippet);
    }

    public class Mailer : IMailer
    {
        private CInterfaces.IMailer _mailer;
        private IMailTemplate _mailTemplate;
        private ILogger _logger;

        public Mailer(CInterfaces.IMailer mailer, IMailTemplate mailTemplate, ILogger<Mailer> logger)
        {
            _mailer = mailer;
            _mailTemplate = mailTemplate;
            _logger = logger;
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

            _logger.LogInformation("[MAIL] Email sent to \"{0}\" with Subject \"{1}\"", to.Email, email.Subject);
            return SendMail(new Mailable(email));
        }

        public Task SendPasswordResetMail(MailUser to, string resetLink)
        {
            var email = new Email()
            {
                To = new List<MailUser> { to },
                Subject = "Matcha Password Reset",
                HTML = _mailTemplate.GetResetTemplate(resetLink)
            };

            _logger.LogInformation("[MAIL] Email sent to \"{0}\" with Subject \"{1}\"", to.Email, email.Subject);
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

            _logger.LogInformation("[MAIL] Email sent to \"{0}\" with Subject \"{1}\"", to.Email, email.Subject);
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

            _logger.LogInformation("[MAIL] Email sent to \"{0}\" with Subject \"{1}\"", to.Email, email.Subject);
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

            _logger.LogInformation("[MAIL] Email sent to \"{0}\" with Subject \"{1}\"", to.Email, email.Subject);
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

            _logger.LogInformation("[MAIL] Email sent to \"{0}\" with Subject \"{1}\"", to.Email, email.Subject);
            return SendMail(new Mailable(email));
        }
    }
}
