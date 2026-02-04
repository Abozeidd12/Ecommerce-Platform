namespace eCommerceCore.DTOs
{
    public class GetCartItem
    {

        public int Id { get; set; }
        public int cartId { get; set; }

        public int productId { get; set; }

        public decimal productPrice { get; set; }

        public string? ProductName { get; set; }


        public int quantity { get; set; }

    }
}
