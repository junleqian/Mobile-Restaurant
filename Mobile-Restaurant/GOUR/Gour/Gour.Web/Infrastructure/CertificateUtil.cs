namespace Gour.Web.Infrastructure
{
    using System;
    using System.Globalization;
    using System.Security.Cryptography.X509Certificates;

    public static class CertificateUtil
    {
        public static X509Certificate2 GetCertificate(StoreName name, StoreLocation location, string thumbprint)
        {
            var store = new X509Store(name, location);
            X509Certificate2Collection certificates = null;
            store.Open(OpenFlags.ReadOnly);

            try
            {
                X509Certificate2 result = null;

                // Every time we call store.Certificates property, a new collection will be returned.
                certificates = store.Certificates;

                for (int i = 0; i < certificates.Count; i++)
                {
                    X509Certificate2 cert = certificates[i];

                    if (cert.Thumbprint.Equals(thumbprint, StringComparison.OrdinalIgnoreCase))
                    {
                        result = new X509Certificate2(cert);
                        break;
                    }
                }

                if (result == null)
                {
                    throw new ApplicationException(string.Format(CultureInfo.InvariantCulture, "No certificate was found for thumbprint {0}", thumbprint));
                }

                return result;
            }
            finally
            {
                if (certificates != null)
                {
                    for (int i = 0; i < certificates.Count; i++)
                    {
                        X509Certificate2 cert = certificates[i];
                        cert.Reset();
                    }
                }

                store.Close();
            }
        }
    }
}