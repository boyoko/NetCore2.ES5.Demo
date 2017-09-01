using Nest;
using System;
using System.Collections.Concurrent;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace NetCore2.ES5.SearchGuard5.Demo
{
    class Program
    {
        public static Uri EsNode;
        public static ConnectionSettings EsConfig;
        public static ElasticClient EsClient;
        //private static X509Certificate2 _issuer = new X509Certificate2(@"c:\Data\certificates\ca\certs\cacert.pem", "qwerty");
        private static ConcurrentDictionary<string, bool> _knownPrints = new ConcurrentDictionary<string, bool>();


        static Func<object, X509Certificate, X509Chain, SslPolicyErrors, bool> s = (sender, cert, chain, errors) => true;

        static void Main(string[] args)
        {
            EsNode = new Uri("https://localhost:9200/");
            EsConfig = new ConnectionSettings(EsNode)
                .DefaultIndex("record-v2")
                .ServerCertificateValidationCallback(s)
                .BasicAuthentication("admin","admin");
            EsClient = new ElasticClient(EsConfig);

            //ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, errors) => true;

            var nodes = EsClient.NodesInfo();
            //ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) =>
            //{
            //    if (errors == SslPolicyErrors.None)
            //        return true;

            //    string certificateHash = certificate.GetCertHashString();
            //    bool knownThumbprintIsValid = false;
            //    if (_knownPrints.TryGetValue(certificateHash, out knownThumbprintIsValid))
            //        return knownThumbprintIsValid;

            //    var isValid = IsValidCertificate(certificate, chain);
            //    _knownPrints.AddOrUpdate(certificateHash, isValid, (s, b) => isValid);
            //    return isValid;

            //};

            if (!nodes.IsValid)
            {
                throw nodes.OriginalException;
            }


            var response = EsClient.Search<dynamic>(s=>s
                                                        .AllTypes()
                                                        .Query(q=>q
                                                                .MatchAll())
                                                    );

            Console.Read();

        }

        //private static bool IsValidCertificate(X509Certificate certificate, X509Chain chain)
        //{
        //    var privateChain = new X509Chain();
        //    //do not do this if you are not in charge of your CA.
        //    //revocation is a real security concern!
        //    privateChain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

        //    var cert2 = new X509Certificate2(certificate);
        //    privateChain.ChainPolicy.ExtraStore.Add(_issuer);
        //    privateChain.Build(cert2);

        //    //Assert our chain has the same number of elements as the certifcate presented by the server
        //    if (chain.ChainElements.Count != privateChain.ChainElements.Count)
        //        return false;

        //    //lets validate the our chain status
        //    foreach (X509ChainStatus chainStatus in privateChain.ChainStatus)
        //    {
        //        //If you are working with custom CA's the only way to get it to be tusted
        //        //Is to add your CA to the machine trusted store.
        //        //Otherwise you'd want to return false from the following statement
        //        if (chainStatus.Status == X509ChainStatusFlags.UntrustedRoot) continue;
        //        //if the chain has any error of any sort return false
        //        if (chainStatus.Status != X509ChainStatusFlags.NoError)
        //            return false;
        //    }

        //    int i = 0;
        //    var found = false;
        //    //We are going to walk both chains and make sure the thumbprints lign up
        //    //while making sure find our CA thumprint in the chain presented by the server
        //    foreach (var element in chain.ChainElements)
        //    {
        //        var c = element.Certificate.Thumbprint;
        //        if (c == _issuer.Thumbprint)
        //            found = true;

        //        var cPrivate = privateChain.ChainElements[i].Certificate.Thumbprint;
        //        if (c != cPrivate)
        //            return false;
        //        i++;
        //    }
        //    return found;
        //}

        
    }
}
