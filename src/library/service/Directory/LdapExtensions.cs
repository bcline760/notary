using System;
using System.Collections.Generic;
using System.Text;

using Novell.Directory.Ldap;

namespace Notary.Service.Directory
{
    /// <summary>
    /// Extensions for LDAP functionality
    /// </summary>
    public static class LdapExtensions
    {
        /// <summary>
        /// Generate a DirectoryEntity object from LDAP attributes
        /// </summary>
        /// <param name="attributeSet">The set of LDAP attributes used to generate the model</param>
        /// <param name="distinguishedName">The distinguished name to use if LDAP attribute doesn't exist</param>
        /// <returns>A DirectoryEntity object modeled from LDAP attributes</returns>
        public static DirectoryEntity CreateEntityFromAttributes(this LdapAttributeSet attributeSet, string distinguishedName)
        {
            var ldapUser = new DirectoryEntity
            {
                ObjectSid = attributeSet.ContainsKey("objectSid") ? attributeSet.GetAttribute("objectSid")?.StringValue : null,
                ObjectGuid = attributeSet.ContainsKey("objectGUID") ? attributeSet.GetAttribute("objectGUID")?.StringValue : null,
                ObjectCategory = attributeSet.ContainsKey("objectCategory") ? attributeSet.GetAttribute("objectCategory")?.StringValue: null,
                ObjectClass = attributeSet.ContainsKey("objectClass") ? attributeSet.GetAttribute("objectClass")?.StringValue: null,
                MemberOf = attributeSet.ContainsKey("memberOf") ? attributeSet.GetAttribute("memberOf")?.StringValueArray: null,
                CommonName = attributeSet.ContainsKey("cn") ? attributeSet.GetAttribute("cn")?.StringValue: null,
                UserName = attributeSet.ContainsKey("name") ? attributeSet.GetAttribute("name")?.StringValue: null,
                SamAccountName = attributeSet.ContainsKey("sAMAccountName") ? attributeSet.GetAttribute("sAMAccountName")?.StringValue: null,
                UserPrincipalName = attributeSet.ContainsKey("userPrincipalName") ? attributeSet.GetAttribute("userPrincipalName")?.StringValue: null,
                Name = attributeSet.ContainsKey("name") ? attributeSet.GetAttribute("name")?.StringValue: null,
                DistinguishedName = attributeSet.ContainsKey("distinguishedName") ? attributeSet.GetAttribute("distinguishedName")?.StringValue ?? distinguishedName: null,
                DisplayName = attributeSet.ContainsKey("displayName") ? attributeSet.GetAttribute("displayName")?.StringValue: null,
                FirstName = attributeSet.ContainsKey("givenName") ? attributeSet.GetAttribute("givenName")?.StringValue: null,
                LastName = attributeSet.ContainsKey("sn") ? attributeSet.GetAttribute("sn")?.StringValue: null,
                Description = attributeSet.ContainsKey("description") ? attributeSet.GetAttribute("description")?.StringValue: null,
                Phone = attributeSet.ContainsKey("telephoneNumber") ? attributeSet.GetAttribute("telephoneNumber")?.StringValue: null,
                EmailAddress = attributeSet.ContainsKey("mail") ? attributeSet.GetAttribute("mail")?.StringValue: null,
                Address = new LdapAddress
                {
                    Street = attributeSet.ContainsKey("streetAddress") ? attributeSet.GetAttribute("streetAddress")?.StringValue: null,
                    City = attributeSet.ContainsKey("l") ? attributeSet.GetAttribute("l")?.StringValue: null,
                    PostalCode = attributeSet.ContainsKey("postalCode") ? attributeSet.GetAttribute("postalCode")?.StringValue: null,
                    StateName = attributeSet.ContainsKey("st") ? attributeSet.GetAttribute("st")?.StringValue: null,
                    CountryName = attributeSet.ContainsKey("co") ? attributeSet.GetAttribute("co")?.StringValue: null,
                    CountryCode = attributeSet.ContainsKey("c") ? attributeSet.GetAttribute("c")?.StringValue : null
                },

                SamAccountType = attributeSet.ContainsKey("sAMAccountType") ? int.Parse(attributeSet.GetAttribute("sAMAccountType")?.StringValue ?? "0") : 0
            };

            return ldapUser;
        }
    }
}
