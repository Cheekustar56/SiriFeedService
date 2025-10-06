using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace SiriFeedService
{
    public class FeedProcessor
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly string _connectionString;
        private readonly string _endpoint;
        private readonly string _requestorRef;
        private readonly List<string> _stopCodes;
        private readonly string _outputFolder;

        public FeedProcessor(ServiceConfiguration config)
        {
            _connectionString = config.ConnectionString ?? throw new ArgumentNullException(nameof(config.ConnectionString));
            _endpoint = config.EndpointUrl ?? throw new ArgumentNullException(nameof(config.EndpointUrl));
            _requestorRef = config.RequestorRef ?? throw new ArgumentNullException(nameof(config.RequestorRef));
            _stopCodes = config.StopCodes ?? throw new ArgumentNullException(nameof(config.StopCodes));
            _outputFolder = config.OutputFolder ?? throw new ArgumentNullException(nameof(config.OutputFolder));

        }

        public async Task ProcessFeedsAsync(CancellationToken token)
        {
            foreach (var stopId in _stopCodes)
            {
                try
                {
                    string xml = BuildSiriSmRequest(stopId);
                    var content = new StringContent(xml, Encoding.UTF8, "application/xml");

                    var response = await _client.PostAsync(_endpoint, content, token);
                    string result = await response.Content.ReadAsStringAsync();

                    Console.WriteLine($"Stop: {stopId}");
                    Console.WriteLine($"Response Status: {response.StatusCode}");
                    Console.WriteLine($"Saving response to: {Path.Combine(_outputFolder, DateTime.UtcNow.ToString("yyyy-MM-dd"), $"siri_{stopId}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xml")}");
                    Console.WriteLine($"Response body (truncated): {result.Substring(0, Math.Min(200, result.Length))}...");

                    string folder = Path.Combine(_outputFolder, DateTime.UtcNow.ToString("yyyy-MM-dd"));
                    Directory.CreateDirectory(folder);
                    string filename = $"siri_{stopId}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xml";
                    await File.WriteAllTextAsync(Path.Combine(folder, filename), result, token);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing stop {stopId}: {ex.Message}");
                }
            }
        }

        private string BuildSiriSmRequest(string stopId)
        {
            string timestamp = DateTime.UtcNow.ToString("s");

            return $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<Siri xmlns=""http://www.siri.org.uk/siri"">
  <ServiceRequest>
    <RequestTimestamp>{timestamp}</RequestTimestamp>
    <RequestorRef>{_requestorRef}</RequestorRef>
    <StopMonitoringRequest>
      <RequestTimestamp>{timestamp}</RequestTimestamp>
      <MonitoringRef>{stopId}</MonitoringRef>
    </StopMonitoringRequest>
  </ServiceRequest>
</Siri>";
        }
    }
}
