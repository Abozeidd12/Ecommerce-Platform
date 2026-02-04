namespace eCommerceCore.DTOs
{
    public class GetCategory
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public ICollection<GetProduct> getProducts { get; set; } = new List<GetProduct>();

    }


}
