using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notary.Contract
{
    public class LoginCredential : ICredential
    {
        public LoginCredential() { }

        public LoginCredential(string username, string password, bool persist)
        {
            Username = username;
            Password = password;
            Persistant = persist;
        }

        public string Username { get; }
        public string Password { get; }
        public bool Persistant { get; }
    }
}
