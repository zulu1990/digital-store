namespace Application.Orders.Models
{
    public class CheckoutRequestModel
    {
        public bool Delivery { get; set; }

        public string? Address { get; set; }
    }
}
