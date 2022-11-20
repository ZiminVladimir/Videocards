namespace Videocards.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string CartId { get; set; }
        public int VCid { get; set; }
        public Videocard SelectVC { get; set; }
    }
}
