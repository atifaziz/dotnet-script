using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Dotnet.Script.Core
{
    public class ScriptDownloader
    {
        public Task<string> Download(string uri) =>
            Download(uri, (_, text) => text);

        public async Task<T> Download<T>(string uri, Func<HttpContentHeaders, string, T> resultor)
        {
            const string plainTextMediaType = "text/plain";
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(uri))
                {
                    response.EnsureSuccessStatusCode();

                    using (HttpContent content = response.Content)
                    {
                        string mediaType = content.Headers.ContentType.MediaType;

                        if (string.IsNullOrWhiteSpace(mediaType) || mediaType.Equals(plainTextMediaType, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return resultor(content.Headers, await content.ReadAsStringAsync());
                        }

                        throw new NotSupportedException($"The media type '{mediaType}' is not supported when executing a script over http/https");
                    }
                }
            }
        }
    }
}
