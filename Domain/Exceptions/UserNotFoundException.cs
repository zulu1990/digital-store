﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public string ExMessage { get;set; }
        public UserNotFoundException(string message)
        {
            ExMessage = message;
        }
    }
}
