using Microsoft.Extensions.Configuration;

namespace Backend.Services;

public class SnowflakeIdGenerator : ISnowflakeIdGenerator
{
    private const int WorkerIdBits = 10;
    private const int SequenceBits = 12;
    private const int MaxWorkerId = (1 << WorkerIdBits) - 1;
    private const int MaxSequence = (1 << SequenceBits) - 1;

    private static readonly long Epoch = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero).ToUnixTimeMilliseconds();

    private readonly object _lock = new();
    private readonly long _workerId;

    private long _lastTimestamp = -1;
    private long _sequence;

    public SnowflakeIdGenerator(IConfiguration configuration)
    {
        var workerId = long.Parse(configuration["Snowflake:WorkerId"]!);

        if (workerId < 0 || workerId > MaxWorkerId)
        {
            throw new ArgumentOutOfRangeException("Snowflake:WorkerId", workerId, $"WorkerId must be between 0 and {MaxWorkerId}.");
        }

        _workerId = workerId;
    }

    public long NextId()
    {
        lock (_lock)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            if (now < _lastTimestamp)
            {
                throw new InvalidOperationException("Clock moved backwards, refusing to generate id.");
            }

            if (now == _lastTimestamp)
            {
                _sequence++;
                if (_sequence > MaxSequence)
                {
                    while (now <= _lastTimestamp)
                    {
                        now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    }
                    _sequence = 0;
                }
            }
            else
            {
                _sequence = 0;
            }

            _lastTimestamp = now;

            var elapsedMs = now - Epoch;

            return (elapsedMs << 22) | (_workerId << 12) | _sequence;
        }
    }
}
