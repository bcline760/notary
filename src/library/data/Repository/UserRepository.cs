using Notary.Contract;
using Notary.Model;
using Notary.Interface.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Notary.Data.Repository
{
    public sealed class UserRepository : BaseRepository<User, UserModel>, IUserRepository
    {
        public UserRepository(IMongoDatabase db) : base(db)
        {
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var filter = Builders<UserModel>.Filter.Eq("email", email);
            var result = await RunQuery(filter);

            return result;
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            var filter = Builders<UserModel>.Filter.Eq("username", username);
            var result = await RunQuery(filter);

            return result;
        }
    }
}
