namespace eCommerceCore.DTOs
{
    public class UpdateProduct
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public int CategoryId { get; set; }

    }




}
