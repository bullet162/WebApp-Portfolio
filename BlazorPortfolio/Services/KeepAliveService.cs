namespace BlazorPortfolio.Services;

public class KeepAliveService : IHostedService, IDisposable
{
    private readonly HttpClient _http;
    private readonly ILogger<KeepAliveService> _logger;
    private readonly string? _baseUrl;
    private readonly TimeSpan _interval;
    private Timer? _timer;

    public KeepAliveService(IConfiguration config, ILogger<KeepAliveService> logger)
        : this(config, logger, new HttpClient()) { }

    public KeepAliveService(IConfiguration config, ILogger<KeepAliveService> logger, HttpClient httpClient)
    {
        _logger = logger;
        _http = httpClient;
        _baseUrl = config["KeepAlive:BaseUrl"];
        _interval = TimeSpan.FromMinutes(
            double.TryParse(config["KeepAlive:IntervalMinutes"], out var m) && m >= 1 ? m : 14);
    }

    public Task StartAsync(CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_baseUrl))
        {
            _logger.LogWarning("KeepAlive:BaseUrl is not configured. Keep-alive pings are disabled.");
            return Task.CompletedTask;
        }
        // Fire immediately on startup, then repeat on interval
        _timer = new Timer(Ping, null, TimeSpan.Zero, _interval);
        return Task.CompletedTask;
    }

    private async void Ping(object? _)
    {
        try
        {
            var resp = await _http.GetAsync(_baseUrl);
            if (!resp.IsSuccessStatusCode)
                _logger.LogWarning("Keep-alive ping returned {Status}", resp.StatusCode);
            else
                _logger.LogInformation("Keep-alive ping OK at {Time}", DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Keep-alive ping failed");
        }
    }

    public Task StopAsync(CancellationToken ct)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
        _http.Dispose();
    }
}
