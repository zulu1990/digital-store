using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Handler
{
    public interface IPasswordHander
    {
        void CreateSaltAndHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        bool ValidatePassword(string password, byte[] passwordHash, byte[] passwordSalt);
    }
}
