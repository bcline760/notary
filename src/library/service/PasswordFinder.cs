using Org.BouncyCastle.OpenSsl;
using System;
using System.Collections.Generic;
using System.Text;

namespace Notary.Service
{
    public class PasswordFinder : IPasswordFinder
    {
        private string _pwd;
        public PasswordFinder(string password)
        {
            _pwd = password;
        }

        public char[] GetPassword()
        {
            return _pwd.ToCharArray();
        }
    }
}
