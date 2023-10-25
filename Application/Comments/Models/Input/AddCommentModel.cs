using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comments.Models.Input
{
    public class AddCommentModel
    {
        public int ProductIdentifier { get; set; }
        public string? Message { get; set; }
        public int? Rating { get; set; }
    }


    public class EditCommentModel
    {
        public string? Message { get; set; }
        public int? Rating { get; set; }

        public Guid CommentId { get; set; }
    }
}
