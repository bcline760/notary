using Notary.Contract;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notary.Interface.Repository
{
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Retrieve a user by their e-mail address
        /// </summary>
        /// <param name="email">The e-mail adress to search</param>
        /// <returns>The User matching the e-mail or null if not found</returns>
        Task<User> GetByEmailAsync(string email);

        /// <summary>
        /// Get a user by their username
        /// </summary>
        /// <param name="username">The username to lookup</param>
        /// <returns>User object matching given username or null if not found</returns>
        Task<User> GetByUsernameAsync(string username);
    }
}
