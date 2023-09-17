using Kapowey.Core.Common.Models.API.Entities;

namespace Kapowey.Core.Services.Data
{
    public interface ISeriesService : IApiEntityService<Series>, IApiEntityListService<SeriesInfo>
    {
    }
}