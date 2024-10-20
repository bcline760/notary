using log4net;

using Notary.Contract;
using Notary.Interface.Repository;
using Notary.Interface.Service;

using System;
using System.Threading.Tasks;

namespace Notary.Service
{
    public class UserService : EntityService<User>, IUserService
    {
        public UserService(IUserRepository repository, ILog log) : base(repository, log)
        {
        }

        public async Task<User> GetByEmail(string email)
        {
            var repo = Repository as IUserRepository;
            if (repo == null)
            {
                throw new InvalidOperationException("Unable to cast to IUserRepository");
            }

            return await repo.GetByEmailAsync(email);
        }

        public async Task<User> GetByUsername(string username)
        {
            var repo = Repository as IUserRepository;
            if (repo == null)
            {
                throw new InvalidOperationException("Unable to cast to IUserRepository");
            }

            return await repo.GetByUsernameAsync(username);
        }

        public override Task SaveAsync(User entity, string updatedBySlug)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return base.SaveAsync(entity, updatedBySlug);
        }
    }
}
