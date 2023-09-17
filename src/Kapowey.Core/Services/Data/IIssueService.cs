using Kapowey.Core.Common.Models.API.Entities;

namespace Kapowey.Core.Services.Data
{
    public interface IIssueService : IApiEntityService<Issue>, IApiEntityListService<IssueInfo>
    {
    }
}