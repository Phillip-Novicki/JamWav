using System.Net;

namespace JamWav.Infrastructure.Tests.Utils;

public class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpResponseMessage? _singleResponse;
    private readonly Dictionary<string, HttpResponseMessage>? _responsesByUrl;

    public HttpRequestMessage? LastRequest { get; private set; }

    public FakeHttpMessageHandler(HttpResponseMessage response)
    {
        _singleResponse = response;
    }

    public FakeHttpMessageHandler(Dictionary<string, HttpResponseMessage> responsesByUrl)
    {
        _responsesByUrl = responsesByUrl;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LastRequest = request;
        if (_responsesByUrl is not null)
        {
            var url = request.RequestUri?.ToString() ?? string.Empty;
            return Task.FromResult(
                _responsesByUrl.TryGetValue(url, out var response)
                    ? response
                    : new HttpResponseMessage(HttpStatusCode.NotFound));
        }
        return Task.FromResult(_singleResponse!);
    }
}
