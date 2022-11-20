namespace Videocards.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string User { get; set; }
        public int ContactPhone { get; set; }
        public string Address { get; set; }
        public int VCid { get; set; }
        public Videocard videocard { get; set; }
    }
}
