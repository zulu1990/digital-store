namespace Domain.Exceptions
{
    public class OrderNotFoundException : Exception
    {
        public string ExMessage { get; set; }
        public Guid UserId { get; set; }

        public OrderNotFoundException(string message, Guid userId)
        {
            ExMessage = message;

            UserId = userId;
        }
    }
}
