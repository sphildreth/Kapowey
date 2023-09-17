namespace Kapowey.Core.Common.Interfaces;

public interface IClockProvider
{
    DateTimeOffset UtcNow { get; }
}