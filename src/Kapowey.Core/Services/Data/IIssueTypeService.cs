using Kapowey.Core.Common.Models.API.Entities;

namespace Kapowey.Core.Services.Data
{
    public interface IIssueTypeService : IApiEntityService<IssueTypeInfo>, IApiEntityListService<IssueTypeInfo>
    {
    }
}