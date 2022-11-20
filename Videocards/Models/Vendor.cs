using System.ComponentModel.DataAnnotations;

namespace Videocards.Models
{
    public class Vendor
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
