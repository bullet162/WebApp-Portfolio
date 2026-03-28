namespace BlazorPortfolio.Services;

public class KeepAliveService(IConfiguration config, ILogger<KeepAliveService> logger) : IHostedService, IDisposable
{
    private readonly HttpClient _http = new();
    private Timer? _timer;
    private readonly string? _baseUrl = config["KeepAlive:BaseUrl"];
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(
        double.TryParse(config["KeepAlive:IntervalMinutes"], out var m) ? m : 10);

    public Task StartAsync(CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_baseUrl))
        {
            logger.LogWarning("KeepAlive:BaseUrl is not configured. Keep-alive pings are disabled.");
            return Task.CompletedTask;
        }
        _timer = new Timer(Ping, null, _interval, _interval);
        return Task.CompletedTask;
    }

    private async void Ping(object? _)
    {
        var now = DateTime.Now;
        if (now.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) return;
        if (now.Hour < 6 || now.Hour >= 22) return;

        try
        {
            var resp = await _http.GetAsync(_baseUrl);
            if (!resp.IsSuccessStatusCode)
                logger.LogWarning("Keep-alive ping returned {Status}", resp.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Keep-alive ping failed");
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
