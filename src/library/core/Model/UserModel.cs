using MongoDB.Bson.Serialization.Attributes;

using Notary.Contract;
using Notary.Security;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notary.Model
{
    public class UserModel : BaseModel
    {
        public UserModel() { }

        public UserModel(User user) : base(user)
        {
            Name = user.Name;
            Email = user.Email;
            Password = user.Password;
            PasswordSalt = user.PasswordSalt;
            Provider = user.Provider;
            Roles = user.Roles;
            Username = user.Username;
        }

        /// <summary>
        /// Get or set the name of the user
        /// </summary>
        [BsonElement("name"), BsonRequired]
        public string Name { get; set; }

        /// <summary>
        /// Get or set the user's e-mail address
        /// </summary>
        [BsonElement("email"), BsonRequired]
        public string Email { get; set; }

        /// <summary>
        /// Get or set the password for the User
        /// </summary>
        [BsonElement("pwd")]
        public byte[] Password { get; set; }

        /// <summary>
        /// Get or set the salt used for the password
        /// </summary>
        [BsonElement("pwd_salt")]
        public byte[] PasswordSalt { get; set; }

        /// <summary>
        /// Get or set the type of authentication used for this user
        /// </summary>
        [BsonElement("provider"), BsonRequired]
        public AuthenticationProvider Provider { get; set; }

        /// <summary>
        /// Get or set the roles assigned to the user
        /// </summary>
        [BsonElement("roles"), BsonRequired]
        public NotaryOperation[] Roles { get; set; }

        /// <summary>
        /// Get or set the username of the user
        /// </summary>
        [BsonElement("username"), BsonRequired]
        public string Username { get; set; }
    }
}
