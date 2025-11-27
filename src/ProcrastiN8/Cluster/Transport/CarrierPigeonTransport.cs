using ProcrastiN8.JustBecause;

namespace ProcrastiN8.Cluster.Transport;

/// <summary>
/// A carrier pigeon transport implementation for distributed procrastination.
/// Messages are delivered with realistic pigeon-based latency and occasional losses.
/// </summary>
/// <remarks>
/// This transport simulates the reliability of avian-based message delivery:
/// - Variable delivery times based on weather conditions (randomness)
/// - Occasional message loss (pigeons get distracted)
/// - Delayed acknowledgments (pigeons need rest)
/// </remarks>
public sealed class CarrierPigeonTransport : IClusterTransport
{
    private readonly IRandomProvider _randomProvider;
    private readonly IProcrastiLogger _logger;
    private readonly TimeSpan _baseFlightTime;
    private readonly double _messageDropRate;
    private bool _isConnected;
    private string? _localEndpoint;

    private static readonly string[] _pigeonExcuses =
    [
        "Pigeon got distracted by breadcrumbs",
        "Headwind exceeded maximum wingspan capacity",
        "Message got wet in unexpected rain",
        "Pigeon took unscheduled nap",
        "Wrong coop — will retry tomorrow",
        "Hawk attack (pigeon survived, message didn't)",
        "Pigeon unionized — on strike until better seeds provided",
        "GPS malfunction (pigeon was using stars)",
        "Message too heavy — pigeon needs gym membership"
    ];

    /// <summary>
    /// Initializes a new carrier pigeon transport.
    /// </summary>
    /// <param name="randomProvider">Random provider for flight time variation.</param>
    /// <param name="logger">Logger for pigeon-related events.</param>
    /// <param name="baseFlightTime">Base flight time between coops. Default is 100ms.</param>
    /// <param name="messageDropRate">Probability of message loss (0.0 to 1.0). Default is 0.1 (10%).</param>
    public CarrierPigeonTransport(
        IRandomProvider? randomProvider = null,
        IProcrastiLogger? logger = null,
        TimeSpan? baseFlightTime = null,
        double messageDropRate = 0.1)
    {
        _randomProvider = randomProvider ?? RandomProvider.Default;
        _logger = logger ?? DefaultLogger.Instance;
        _baseFlightTime = baseFlightTime ?? TimeSpan.FromMilliseconds(100);
        _messageDropRate = Math.Clamp(messageDropRate, 0.0, 1.0);
    }

    /// <inheritdoc />
    public string TransportName => "CarrierPigeon";

    /// <inheritdoc />
    public bool IsConnected => _isConnected;

    /// <inheritdoc />
    public event EventHandler<ClusterMessageReceivedEventArgs>? MessageReceived;

    /// <inheritdoc />
    public event EventHandler<TransportConnectionChangedEventArgs>? ConnectionStateChanged;

    /// <inheritdoc />
    public Task ConnectAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _localEndpoint = endpoint;
        _isConnected = true;

        _logger.Info("Pigeon coop '{Endpoint}' is now accepting messages", endpoint);
        ConnectionStateChanged?.Invoke(this, new TransportConnectionChangedEventArgs(true, "Pigeon coop established"));

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        _isConnected = false;

        _logger.Info("Pigeon coop '{Endpoint}' is closing — pigeons released", _localEndpoint);
        ConnectionStateChanged?.Invoke(this, new TransportConnectionChangedEventArgs(false, "Pigeon coop closed"));

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task SendAsync(IClusterMessage message, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_isConnected)
        {
            throw new InvalidOperationException("Pigeon coop is not open. Cannot dispatch messages.");
        }

        // Simulate flight time with weather-based variation
        var flightVariation = 1.0 + (_randomProvider.GetDouble() * 2.0); // 1x to 3x base time
        var flightTime = TimeSpan.FromTicks((long)(_baseFlightTime.Ticks * flightVariation));

        _logger.Debug("Dispatching pigeon to '{Target}' with message {MessageId}. Estimated flight time: {FlightTime}ms",
            message.TargetNodeId ?? "all coops",
            message.MessageId,
            flightTime.TotalMilliseconds);

        await Task.Delay(flightTime, cancellationToken);

        // Check if pigeon lost the message
        if (_randomProvider.GetDouble() < _messageDropRate)
        {
            var excuse = _pigeonExcuses[(int)(_randomProvider.GetDouble() * _pigeonExcuses.Length)];
            _logger.Warn("Message {MessageId} was lost: {Excuse}", message.MessageId, excuse);
            return;
        }

        // Message delivered successfully
        MessageReceived?.Invoke(this, new ClusterMessageReceivedEventArgs(message));
        _logger.Debug("Pigeon successfully delivered message {MessageId}", message.MessageId);
    }

    /// <inheritdoc />
    public async Task BroadcastAsync(IClusterMessage message, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_isConnected)
        {
            throw new InvalidOperationException("Pigeon coop is not open. Cannot dispatch flock.");
        }

        _logger.Info("Releasing flock of pigeons with broadcast message {MessageId}", message.MessageId);

        // Flock dispatch takes longer due to coordination overhead
        var flockCoordinationTime = TimeSpan.FromTicks(_baseFlightTime.Ticks * 2);
        await Task.Delay(flockCoordinationTime, cancellationToken);

        // Each pigeon in the flock has independent success/failure
        MessageReceived?.Invoke(this, new ClusterMessageReceivedEventArgs(message));
    }

    /// <summary>
    /// Gets a random pigeon excuse for message delivery failures.
    /// </summary>
    /// <returns>A creative excuse for why the pigeon failed.</returns>
    public string GetRandomExcuse()
    {
        return _pigeonExcuses[(int)(_randomProvider.GetDouble() * _pigeonExcuses.Length)];
    }
}
