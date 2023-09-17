using Kapowey.Core.Common.Interfaces;

namespace Kapowey.Core.Services;

public class UtcClockProvider : IClockProvider
{
    public DateTimeOffset UtcNow => DateTime.UtcNow;
}