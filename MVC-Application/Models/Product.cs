namespace MVC_Application.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Url { get; set; }
        public int ProductIdentifier { get; set; }
    }
}
