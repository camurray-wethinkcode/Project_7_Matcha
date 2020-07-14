using System;

namespace Matcha.API.Models
{
    public class Photo
    {
        public long Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }
        public virtual User User { get; set; }
        public long UserId { get; set; }
    }
}