using System.Threading.Tasks;
using Matcha.API.Models;
using CInterfaces = Coravel.Mailer.Mail.Interfaces;

namespace Matcha.API.Helpers
{
    public interface IMailer
    {
        Task SendMail(Mailable mailable);
    }

    public class Mailer : IMailer
    {
        private CInterfaces.IMailer _mailer;

        public Mailer(CInterfaces.IMailer mailer) => _mailer = mailer;

        public Task SendMail(Mailable mailable) => _mailer.SendAsync(mailable);
    }
}
