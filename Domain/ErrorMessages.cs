using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ErrorMessages
    {

        public const string CommentNotFound = "Comment Not Found";

        public const string IncorrectCommentParameters = "Incorrect Comment Parameters";

        public const string UserNotFound = "User Was Not Found";

        public const string IncorrectCode = "Code was incorrect";

        public const string UserAlreadyExist = "User Already Exists";

        public const string IncorrectCredentials = "Incorrect Credentials";

        public const string NegativePriceDetected = "Negative Price Detected";

        public static string UserWasNotBanned { get; set; }
    }
}
