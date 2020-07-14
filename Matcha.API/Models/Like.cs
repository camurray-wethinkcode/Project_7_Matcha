namespace Matcha.API.Models
 {
     public class Like
     {
         public long LikerId { get; set; }
         public long LikeeId { get; set; }
         public virtual User Liker { get; set; }
         public virtual User Likee { get; set; }
     }
 }  