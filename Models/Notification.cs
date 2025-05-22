using System.ComponentModel.DataAnnotations;

namespace GiftGenieAPIWebApp.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        public int UserId { get; set; }  
        public virtual User User { get; set; } 
        public int SenderUserId { get; set; }  
        public virtual User SenderUser { get; set; }
        public string Message { get; set; }
    }

}
