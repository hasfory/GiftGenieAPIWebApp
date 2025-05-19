using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftGenieAPIWebApp.Models
{
    public class Image
    {
        [Key]
        public int ImageId { get; set; }
        [Required]
        public byte[] Photo { get; set; } = default!;
        [ForeignKey(nameof(Gift))]
        public int GiftId { get; set; }
        public Gift Gift { get; set; } = default!;
    }
}
