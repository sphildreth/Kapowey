using Kapowey.Core.Common.Models.API;
using Kapowey.Core.Entities;

namespace Kapowey.Core.Services.Data
{
    public interface IApiEntityListService<T>
    {
        Task<IPagedResponse<T>> ListAsync(User user, PagedRequest request);
    }
}