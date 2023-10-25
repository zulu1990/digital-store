namespace Domain.Entity
{
    public class Comment : BaseEntity
    {
        public string? Message { get; set; }
        public int? Rating { get; set; }

        public Guid UserId { get; set; }

        public int ProductIdentifier { get; set; }
    }
}
