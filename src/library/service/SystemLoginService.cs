using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Notary.Configuration;
using Notary.Contract;
using Notary.Interface.Service;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Notary.Service
{
    public class SystemLoginService : LoginService
    {
        public SystemLoginService(IUserService userSvc, NotaryConfiguration config) : base(userSvc)
        {
            Configuration = config;
        }

        public async override Task<AuthenticationToken> Authenticate(ICredential credential)
        {
            if (credential == null)
            {
                throw new ArgumentNullException(nameof(credential));
            }

            var user = await UserService.GetByUsername(credential.Username);
            if (user != null)
            {
                byte[] salt = user.PasswordSalt;
                byte[] userPwd = user.Password;

                var pdb = new Pkcs5S2ParametersGenerator(new Sha256Digest());
                pdb.Init(PbeParametersGenerator.Pkcs5PasswordToBytes(credential.Password.ToCharArray()), salt, 2048); // TODO: Magic Number
                var key = (KeyParameter)pdb.GenerateDerivedMacParameters(128 * 8); // TODO: Magic Number

                byte[] hashedPwd = key.GetKey();

                bool equalHashes = userPwd.AreBytesEqual(hashedPwd);
                if (equalHashes)
                {
                    var expiry = credential.Persistant ? DateTime.UtcNow.AddYears(1) : DateTime.UtcNow.AddHours(1);
                    var token = new AuthenticationToken
                    {
                        AccessToken = GenerateToken(user, expiry),
                        Active = true,
                        Created = DateTime.UtcNow,
                        CreatedBySlug = user.Slug,
                        Expiry = expiry
                    };

                    return token;
                }
            }

            return null;
        }

        private string GenerateToken(User user, DateTime expiry)
        {
            var handler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.Default.GetBytes(Configuration.ApplicationKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = expiry,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("slug", user.Slug),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Role, "")
                })
            };

            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }

        protected NotaryConfiguration Configuration { get; }
    }
}
