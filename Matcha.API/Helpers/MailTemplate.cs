using System.IO;
using System.Reflection;

namespace Matcha.API.Helpers
{
    public interface IMailTemplate
    {
        public string GetRegisterTemplate(string verifyLink);
        public string GetResetTemplate(string resetLink);
        public string GetLikeTemplate(string userName, string likedByName);
        public string GetUnlikeTemplate(string userName, string unlikedByName);
        public string GetViewedTemplate(string userName, string viewedBy);
        public string GetMessagedTemplate(string userName, string fromName, string messsageSnippet);
    }

    public class MailTemplate : IMailTemplate
    {
        public string GetRegisterTemplate(string verifyLink)
        {
            var html = GetTemplate("RegisterTemplate");
            return string.Format(html, verifyLink);
        }

        public string GetResetTemplate(string resetLink)
        {
            var html = GetTemplate("ResetTemplate");
            return string.Format(html, resetLink);
        }

        public string GetLikeTemplate(string userName, string likedByName)
        {
            var html = GetTemplate("LikeTemplate");
            return string.Format(html, userName, likedByName);
        }

        public string GetUnlikeTemplate(string userName, string unlikedByName)
        {
            var html = GetTemplate("UnlikeTemplate");
            return string.Format(html, userName, unlikedByName);
        }

        public string GetViewedTemplate(string userName, string viewedBy)
        {
            var html = GetTemplate("ViewedTemplate");
            return string.Format(html, userName, viewedBy);
        }

        public string GetMessagedTemplate(string userName, string fromName, string messsageSnippet)
        {
            var html = GetTemplate("MessagedTemplate");
            return string.Format(html, userName, fromName, messsageSnippet);
        }

        private string GetTemplate(string templateName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Matcha.API.Helpers.EmailTemplates." + templateName + ".html";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
