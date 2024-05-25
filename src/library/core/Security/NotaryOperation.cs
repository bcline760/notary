using System;
namespace Notary.Security
{
    public enum NotaryOperation
    {
        None = 0,
        // Certificates = 10
        CertificateRead = 10,
        CertificateWrite = 11,
        CertificateDelete = 12,
        CertificateRevoke = 13,
        CertificateIssue = 14,

        // Asymmetric Keys = 20
        AsymmetricKeyRead = 20,
        AsymmetricKeyWrite = 21,
        AsymmetricKeyDelete = 22,

        // Certificate Authorities = 30
        CertificateAuthorityRead = 30,
        CertificateAuthorityWrite = 31,
        CertificateAuthorityCreate = 32,
        CertificateAuthorityDelete = 33
    }
}
