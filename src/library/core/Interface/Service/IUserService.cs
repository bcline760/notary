using Notary.Contract;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notary.Interface.Service
{
    /// <summary>
    /// Represents a service in which to retrieve and store users in the database
    /// </summary>
    public interface IUserService : IEntityService<User>
    {
        /// <summary>
        /// Look up an user by e-mail.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<User> GetByEmail(string email);

        /// <summary>
        /// Get a user by their username
        /// </summary>
        /// <param name="username">The username to lookup</param>
        /// <returns>User object matching given username or null if not found</returns>
        Task<User> GetByUsername(string username);
    }
}
