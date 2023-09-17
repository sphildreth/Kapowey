using Kapowey.Core.Common.Interfaces;

namespace Kapowey.Core.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.UtcNow;
}
