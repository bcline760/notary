using Notary.Interface.Service;
using Notary.Contract;
using Notary.Interface.Repository;

namespace Notary.Test;

public class CertificateServiceTest
{
    private Mock<ICertificateRepository> _certificateRepo;
    private Mock<ICertificateAuthorityService> _caService;

    public CertificateServiceTest()
    {
        _certificateRepo = new Mock<ICertificateRepository>();
        _caService = new Mock<ICertificateAuthorityService>();
    }

    [SetUp]
    public async Task SetupTest()
    {
        var certificate = CreateCertificateMock();
        var certificateList = new List<Certificate>
        {
            certificate
        };
        _certificateRepo.Setup(r => r.SaveAsync(certificate));
        _certificateRepo.Setup(r => r.GetCertificatesByCaAsync(It.IsAny<string>())).ReturnsAsync(certificateList);

        _caService.Setup(s => s.GetAsync(It.IsAny<string>())).ReturnsAsync(MockCa());
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
            IssuingSlug = It.IsAny<string>(),
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
            KeyUsages = It.IsAny<List<string>>(),
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

    private CertificateAuthority MockCa()
    {
        return new CertificateAuthority
        {
            Active = true,
            Created = DateTime.MinValue,
            CreatedBySlug = It.IsAny<string>(),
            DistinguishedName = new DistinguishedName
            {
                CommonName = It.IsAny<string>(),
                Country = It.IsAny<string>(),
                Locale = It.IsAny<string>(),
                Organization = It.IsAny<string>(),
                OrganizationalUnit = It.IsAny<string>(),
                StateProvince = It.IsAny<string>()
            },
            IsIssuer = It.IsAny<bool>(),
            IssuingDn = new DistinguishedName
            {
                CommonName = It.IsAny<string>(),
                Country = It.IsAny<string>(),
                Locale = It.IsAny<string>(),
                Organization = It.IsAny<string>(),
                OrganizationalUnit = It.IsAny<string>(),
                StateProvince = It.IsAny<string>()
            },
            KeyAlgorithm = It.IsAny<Algorithm>(),
            KeyCurve = null,
            KeyLength = It.IsAny<int>(),
            Name = It.IsAny<string>(),
            ParentCaSlug = It.IsAny<string>(),
            Slug = It.IsAny<string>(),
            Updated = null,
            UpdatedBySlug = It.IsAny<string>()
        };
    }

    private CertificateRequest MockRequest()
    {
        return new CertificateRequest
        {
            ParentCertificateSlug = It.IsAny<string>(),
            Curve = null,
            KeyAlgorithm = Algorithm.RSA,
            KeySize = It.IsAny<int>(),
            KeyUsages = It.IsAny<List<string>>(),
            Name = It.IsAny<string>(),
            NotAfter = It.IsAny<DateTime>(),
            NotBefore = It.IsAny<DateTime>(),
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