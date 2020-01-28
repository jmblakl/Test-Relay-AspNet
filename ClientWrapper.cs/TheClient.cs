using Microsoft.Azure.Relay;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ClientWrapper.cs
{
    public sealed class TheClient
    {
        private const string RelayNamespaceFormat = "{0}.servicebus.windows.net";

        private readonly string _RelayNamespace;
        private readonly string _ConnectionName;
        private readonly string _KeyName;
        private readonly string _Key;
        private readonly HttpClientHandler _Handler;
        public TheClient(string relayNamespace, string connectionName, string keyName, string key)
        {
            _RelayNamespace = string.Format(RelayNamespaceFormat, relayNamespace);
            _ConnectionName = connectionName;
            _KeyName = keyName;
            _Key = key;
            _Handler = new HttpClientHandler();
        }

        public async Task DoQueryForThings()
        {
            using (HttpClient client = new HttpClient(_Handler, false))
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(string.Format("https://{0}/{1}/test/queryforthings", _RelayNamespace, _ConnectionName)),
                    Method = HttpMethod.Get
                };
                request.Headers.Add("ServiceBusAuthorization", await GetTokenAsync());
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
        }

        public async Task<string> PerformSignificantWork(string message)
        {
            //i a real world we would stream this to avoid memory pressure
            SomeUnitOfWork work = new SomeUnitOfWork { ImportantMessage = message };
            var messagePayload = JsonConvert.SerializeObject(work);

            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(messagePayload)))
            {
                using (HttpClient client = new HttpClient(_Handler, false))
                {
                    var request = new HttpRequestMessage
                    {
                        RequestUri = new Uri(string.Format("https://{0}/{1}/test/DoSignificantWork", _RelayNamespace, _ConnectionName)),
                        Method = HttpMethod.Post,
                        Content = new StreamContent(stream),                        
                    };
                    request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    request.Headers.Add("ServiceBusAuthorization", await GetTokenAsync());
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    //again we would stream this to the object in real world
                    var content = await response.Content.ReadAsStringAsync();
                    return (JsonConvert.DeserializeObject<SomeUnitOfWorkResponse>(content)).Response;
                }
            }
        }

        private async Task<string> GetTokenAsync()
        {
            var uri = new Uri(string.Format("https://{0}/{1}", _RelayNamespace, _ConnectionName));
            var tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(_KeyName, _Key);
            var token = await tokenProvider.GetTokenAsync(uri.AbsoluteUri, TimeSpan.FromHours(1));
            return token.TokenString;
        }
    }
}
