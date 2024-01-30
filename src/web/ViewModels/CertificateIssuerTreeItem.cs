namespace Notary.Web.ViewModels
{
    public class CertificateIssuerTreeItem
    {
        public CertificateIssuerTreeItem()
        {
            Children = new();
        }

        public CertificateIssuerTreeItem(string name, string slug)
        {
            Name = name;
            Slug = slug;
            Children = new();
        }

        public string Name { get; set; }

        public string Slug { get; set; }

        public HashSet<CertificateIssuerTreeItem> Children { get; }
    }
}
