using System.ComponentModel.DataAnnotations;

namespace Notary.Web.ViewModels
{
    public class AuthenticateViewModel
    {
        /// <summary>
        /// Get or set the login credentials
        /// </summary>
        [Required]
        public string Login { get; set; }

        /// <summary>
        /// Get or set the password credentials
        /// </summary>
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Get or set whether to persist the login token or not
        /// </summary>
        public bool Persist { get; set; }
    }
}
