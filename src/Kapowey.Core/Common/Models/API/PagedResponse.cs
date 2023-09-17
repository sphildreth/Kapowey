namespace Kapowey.Core.Common.Models.API
{
    public sealed class PagedResponse<T> : ResponseBase, IPagedResponse<T>
    {
        /// <summary>
        /// The page number this page represents.
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// The size of this page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The total number of pages available.
        /// </summary>
        public int TotalNumberOfPages { get; set; }

        /// <summary>
        /// The total number of records available.
        /// </summary>
        public int TotalNumberOfRecords { get; set; }

        /// <summary>
        /// The URL to the next page - if null, there are no more pages.
        /// </summary>
        public string NextPageUrl { get; set; }

        /// <summary>
        /// The records this page represents.
        /// </summary>
        public IEnumerable<T> Data { get; private set; }

        public PagedResponse(IServiceResponseMessage message)
           : this(default, new IServiceResponseMessage[1] { message })
        {
        }

        public PagedResponse(IEnumerable<T> data)
           : this(data, new IServiceResponseMessage[1] { new ServiceResponseMessage(ServiceResponseMessageType.Ok) })
        {
        }

        public PagedResponse(IEnumerable<T> data, IServiceResponseMessage message)
           : this(data, new IServiceResponseMessage[1] { message })
        {
        }

        public PagedResponse(IEnumerable<T> data, IEnumerable<IServiceResponseMessage> messages = null)
            : base(messages)
        {
            Data = data;
        }
    }
}