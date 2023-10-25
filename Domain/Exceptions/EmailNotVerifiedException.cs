using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class EmailNotVerifiedException : Exception
    {
        public readonly string ErrorMessage;

        public EmailNotVerifiedException(string errorMessage) 
        {
            ErrorMessage = errorMessage;
        }
    }
}
