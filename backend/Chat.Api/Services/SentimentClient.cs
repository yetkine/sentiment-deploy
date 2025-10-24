using System.Net.Http.Json;
using System.Text.Json;

namespace Chat.Api.Services
{
    public class SentimentClient
    {
        private readonly HttpClient _http;
        private readonly string _endpoint;
        private static readonly JsonSerializerOptions _json =
            new(JsonSerializerDefaults.Web) { PropertyNameCaseInsensitive = true };

        public SentimentClient(IConfiguration config, HttpClient http)
        {
            _http = http;
            // AiService:Endpoint yoksa "analyze" kullan
            var ep = config["AiService:Endpoint"] ?? "analyze";
            _endpoint = ep.Trim().TrimStart('/'); // safety: "analyze" formatına indir
        }

        public async Task<(string label, double score)> AnalyzeAsync(string text, CancellationToken ct)
        {
            var payload = new { text };
            using var resp = await _http.PostAsJsonAsync(_endpoint, payload, ct);
            resp.EnsureSuccessStatusCode();

            using var stream = await resp.Content.ReadAsStreamAsync(ct);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

            var root = doc.RootElement;
            var label = root.GetProperty("label").GetString() ?? "unknown";
            var score = root.GetProperty("score").GetDouble();

            return (label, score);
        }
    }
}
