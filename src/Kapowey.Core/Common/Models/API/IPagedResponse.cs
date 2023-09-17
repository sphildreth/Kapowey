namespace Kapowey.Core.Common.Models.API
{
    public interface IPagedResponse<T> : IResponse
    {
        IEnumerable<T> Data { get; }
        string NextPageUrl { get; set; }
        int PageNumber { get; set; }
        int PageSize { get; set; }
        int TotalNumberOfPages { get; set; }
        int TotalNumberOfRecords { get; set; }
    }
}