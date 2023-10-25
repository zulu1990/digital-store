using Domain.Enum;

namespace Application.Customer.Models
{
    public class EditUserRequestModel
    {
        public Role? Role { get; set; }
        public string? Currency { get; set; }
        public string? Address { get; set; }
    }
}
