using System;
using System.Collections.Generic;
using Coravel.Mailer.Mail;

namespace Matcha.API.Models
{
    public class Mailable : Mailable<Mailable>
    {
        private Email _email;

        public Mailable(Email email)
        {
            if (email.To == null || email.To.Count == 0)
                throw new ArgumentNullException("email.To", "To List is empty");
            if (string.IsNullOrEmpty(email.HTML))
                throw new ArgumentNullException("email.HTML", "HTML is empty");
            if (string.IsNullOrEmpty(email.Subject))
                throw new ArgumentNullException("email.Subject", "Subject is empty");
            _email = email;
        }

        public override void Build()
        {
            Html(_email.HTML);
            Subject(_email.Subject);

            var _toList = new List<MailRecipient>();
            foreach (var mailUser in _email.To)
            {
                _toList.Add(new MailRecipient(
                    mailUser.Email,
                    mailUser.Name
                ));
            }
            To(_toList);

            var _ccList = new List<MailRecipient>();
            foreach (var mailUser in _email.Cc)
            {
                _ccList.Add(new MailRecipient(
                    mailUser.Email,
                    mailUser.Name
                ));
            }
            Cc(_ccList);

            var _bccList = new List<MailRecipient>();
            foreach (var mailUser in _email.Bcc)
            {
                _bccList.Add(new MailRecipient(
                    mailUser.Email,
                    mailUser.Name
                ));
            }
            Bcc(_bccList);

            if (_email.ReplyTo != null)
            {
                ReplyTo(new MailRecipient(
                    _email.ReplyTo.Email,
                    _email.ReplyTo.Name
                ));
            }
        }
    }
}
