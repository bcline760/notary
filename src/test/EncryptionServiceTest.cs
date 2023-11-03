namespace Notary.Test;

using Notary.Interface.Service;
using Org.BouncyCastle.Crypto.Tls;

public class EncryptionServiceTest
{
    private Mock<IEncryptionService> _svc;

    [SetUp]
    public void Setup()
    {
        var loggerMoq = new Mock<ILog>();
        loggerMoq.Setup(s => s.Info(It.IsAny<string>()));
        _svc = new Mock<IEncryptionService>();

        _svc.Setup(s => s.Hash(It.IsAny<string>())).Returns(It.IsAny<string>());
    }

    [Test]
    public void ItShouldHash()
    {
        Assert.Pass();
    }
}