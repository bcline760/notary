using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Notary.Contract;
using Notary.Interface.Repository;
using Notary.Interface.Service;
using Notary.Service;

namespace Notary.Test;

internal class CertificateRevokeServiceTest
{
    private ICertificateRevokeService certificateRevokeService;

    public CertificateRevokeServiceTest()
    {
    }

    [SetUp]
    public void Setup()
    {
        var rcRepoMock = new Mock<IRevocatedCertificateRepository>();
        var asymKeySvcMock = new Mock<IAsymmetricKeyService>();
        var caSvcMock = new Mock<ICertificateAuthorityService>();
        var certSvcMock = new Mock<ICertificateService>();
        var logSvcMock = new Mock<ILog>();

        var rc = CreateMock();
        var list = new List<RevocatedCertificate>() { rc };
        RevocatedCertificate nullRc = null;

        rcRepoMock.Setup(s => s.GetAllAsync()).ReturnsAsync(list);
        rcRepoMock.Setup(s => s.GetAsync("slug")).ReturnsAsync(rc);
        rcRepoMock.Setup(s => s.GetAsync("bad-slug")).ReturnsAsync(nullRc);
        rcRepoMock.Setup(s => s.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(rc);

        certificateRevokeService = new CertificateRevokeService(
            rcRepoMock.Object,
            caSvcMock.Object,
            certSvcMock.Object,
            asymKeySvcMock.Object,
            logSvcMock.Object,
            new Configuration.NotaryConfiguration
            {
                ApplicationKey = "key"
            }
        );
    }

    [TearDown]
    public void TearDown()
    {

    }

    [Test]
    public async Task TestGetAllAsync()
    {
        var rcList = await certificateRevokeService.GetAllAsync();

        Assert.That(rcList != null);

        Assert.That(rcList.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task TestGetAsync()
    {
        var bad = await certificateRevokeService.GetAsync("bad-slug");
        Assert.That(bad == null);

        var good = await certificateRevokeService.GetAsync("slug");
        Assert.That(good != null);
        Assert.That(good.Active, Is.True);
        Assert.That(string.Compare(good.Slug, "slug") == 0);
    }

    private RevocatedCertificate CreateMock()
    {
        return new RevocatedCertificate
        {
            Active = true,
            CertificateSlug = "test",
            Created = DateTime.UtcNow,
            CreatedBySlug = "test",
            Reason = RevocationReason.Unspecified,
            SerialNumber = "sn",
            Slug = "slug",
            Thumbprint = "thumb",
            Updated = DateTime.UtcNow,
            UpdatedBySlug = "slug"
        };
    }
}
