using Notary.Contract;

using System;
using System.Collections.Generic;
using System.Text;

namespace Notary.Model
{
    public class SanModel
    {
        public SanModel()
        {
        }

        public SanModel(SubjectAlternativeName san)
        {
            Kind = san.Kind;
            Name = san.Name;
        }
        /// <summary>
        /// The kind of SAN
        /// </summary>
        public SanKind Kind
        {
            get;set;
        }

        /// <summary>
        /// Subject Alternative Name. Can be a DNS or e-mail
        /// </summary>
        public string Name
        {
            get;set;
        }
    }
}
