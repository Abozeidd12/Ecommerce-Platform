using System.ComponentModel.DataAnnotations;

namespace eCommerceDomain.Entities
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        public string? UserId { get; set; }

        

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }

}
