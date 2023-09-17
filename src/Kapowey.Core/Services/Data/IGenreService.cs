using Kapowey.Core.Common.Models.API.Entities;

namespace Kapowey.Core.Services.Data
{
    public interface IGenreService : IApiEntityService<GenreInfo>, IApiEntityListService<GenreInfo>
    {
    }
}