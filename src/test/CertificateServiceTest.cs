using Notary.Interface.Service;
using Notary.Contract;

namespace Notary.Test;

public class CertificateServiceTest
{
    private Mock<ICertificateService> _svc = null;

    public CertificateServiceTest()
    {
        _svc = new Mock<ICertificateService>();
    }

    [SetUp]
    public void SetupTest()
    {
        var certificate = CreateCertificateMock();

        _svc.Setup(s => s.IssueCertificateAsync(MockRequest())).ReturnsAsync(certificate);
        _svc.Setup(s => s.RequestCertificateAsync(It.IsAny<string>(), It.IsAny<CertificateFormat>(), It.IsAny<string>())).ReturnsAsync(It.IsAny<byte[]>());
        _svc.Setup(s => s.SaveAsync(certificate, It.IsAny<string>()));
    }

    [Test]
    public async Task IssueCertificateAsyncTest()
    {

    }

    private Certificate CreateCertificateMock()
    {
        return new Certificate
        {
            Active = true,
            CertificateAuthoritySlug = It.IsAny<string>(),
            CreatedBySlug = It.IsAny<string>(),
            Created = DateTime.MinValue,
            IsCaCertificate = It.IsAny<bool>(),
            Issuer = new DistinguishedName
            {
                CommonName = It.IsAny<string>(),
                Country = It.IsAny<string>(),
                Locale = It.IsAny<string>(),
                Organization = It.IsAny<string>(),
                OrganizationalUnit = It.IsAny<string>(),
                StateProvince = It.IsAny<string>()
            },
            KeyAlgorithm = Algorithm.RSA,
            KeyCurve = null,
            KeyLength = It.IsAny<int>(),
            KeyUsage = It.IsAny<short>(),
            Name = It.IsAny<string>(),
            NotAfter = DateTime.MaxValue,
            NotBefore = DateTime.MinValue,
            RevocationDate = null,
            SerialNumber = It.IsAny<string>(),
            SignatureAlgorithm = It.IsAny<string>(),
            Slug = It.IsAny<string>(),
            Subject = new DistinguishedName
            {
                CommonName = It.IsAny<string>(),
                Country = It.IsAny<string>(),
                Locale = It.IsAny<string>(),
                Organization = It.IsAny<string>(),
                OrganizationalUnit = It.IsAny<string>(),
                StateProvince = It.IsAny<string>()
            },
            SubjectAlternativeNames = new List<SubjectAlternativeName>(),
            Thumbprint = It.IsAny<string>(),
            Updated = null,
            UpdatedBySlug = null
        };
    }

    private CertificateRequest MockRequest()
    {
        return new CertificateRequest
        {
            CertificateAuthoritySlug = It.IsAny<string>(),
            CertificatePassword = It.IsAny<string>(),
            Curve = null,
            KeyAlgorithm = Algorithm.RSA,
            KeySize = It.IsAny<int>(),
            KeyUsage = It.IsAny<short>(),
            LengthInHours = It.IsAny<int>(),
            Name = It.IsAny<string>(),
            RequestedBySlug = It.IsAny<string>(),
            Subject = new DistinguishedName
            {
                CommonName = It.IsAny<string>(),
                Country = It.IsAny<string>(),
                Locale = It.IsAny<string>(),
                Organization = It.IsAny<string>(),
                OrganizationalUnit = It.IsAny<string>(),
                StateProvince = It.IsAny<string>()
            },
            SubjectAlternativeNames = new List<SubjectAlternativeName>()
        };
    }
}