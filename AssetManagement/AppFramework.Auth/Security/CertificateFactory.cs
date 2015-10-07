using System.Security.Cryptography.X509Certificates;
using System.Web;
using AppFramework.Auth.Config;

namespace AppFramework.Auth.Security
{
    public static class CertificateFactory
    {
        public static X509Certificate2 Get()
        {
            var certificateSettings = AuthConfiguration.Instance.Certificate;
            var certificatePath = HttpRuntime.AppDomainAppPath + certificateSettings.File;
            return new X509Certificate2(certificatePath, certificateSettings.Password);
        }
    }
}