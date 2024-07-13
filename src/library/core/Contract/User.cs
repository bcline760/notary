using Notary.Model;
using Notary.Security;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notary.Contract
{
    public class User : Entity
    {
        public User() { }

        public User(UserModel model) : base(model)
        {
            if (model == null) throw new ArgumentNullException("model");

            Name = model.Name;
            Email = model.Email;
            Password = model.Password;
            PasswordSalt = model.PasswordSalt;
            Provider = model.Provider;
            Roles = model.Roles;
            Username = model.Username;
        }

        /// <summary>
        /// Get or set the name of the user
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get or set the user's e-mail address
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Get or set the password for the User
        /// </summary>
        public byte[] Password { get; set; }

        /// <summary>
        /// Get or set the salt used for the password
        /// </summary>
        public byte[] PasswordSalt { get; set; }

        /// <summary>
        /// Get or set the type of authentication used for this user
        /// </summary>
        public AuthenticationProvider Provider { get; set; }

        /// <summary>
        /// Get or set the roles assigned to the user
        /// </summary>
        public NotaryOperation[] Roles { get; set; }

        /// <summary>
        /// Get or set the username of the user
        /// </summary>
        public string Username { get; set; }

        public override string[] SlugProperties() => new string[] { Username.ToLower(), Guid.NewGuid().ToString().FirstEight() };
    }
}
