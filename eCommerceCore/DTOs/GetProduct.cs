namespace eCommerceCore.DTOs
{
    public class GetProduct
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public decimal Price { get; set; }
        public string? CategoryName { get; set; }
        public int CategoryId { get; set; } 

    }




}
