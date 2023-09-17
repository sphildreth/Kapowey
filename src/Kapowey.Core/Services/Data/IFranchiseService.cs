using Kapowey.Core.Common.Models.API.Entities;

namespace Kapowey.Core.Services.Data
{
    public interface IFranchiseService : IApiEntityService<Franchise>, IApiEntityListService<FranchiseInfo>
    {
    }
}