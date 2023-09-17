using Kapowey.Core.Common.Models.API;
using Kapowey.Core.Entities;

namespace Kapowey.Core.Services.Data
{
    public interface IApiEntityService<T>
    {
        Task<IServiceResponse<T>> ByIdAsync(User user, Guid apiKeyToGet);

        Task<IServiceResponse<bool>> DeleteAsync(User user, Guid apiKeyToDelete);

        Task<IServiceResponse<bool>> ModifyAsync(User user, T modifyModel);

        Task<IServiceResponse<Guid>> AddAsync(User user, T createModel);
    }
}