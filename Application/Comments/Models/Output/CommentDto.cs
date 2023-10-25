namespace Application.Comments.Models.Output
{
    public class CommentDto
    {
        public Guid CommentId { get; set; }
        public string? Message { get; set; }
        public int? Rating { get; set; }
        public string Name { get; set; } = "Anonymus";
    }
}
