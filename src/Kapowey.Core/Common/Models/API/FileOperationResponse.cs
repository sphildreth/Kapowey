using System.Text.Json.Serialization;
using Microsoft.Net.Http.Headers;
using NodaTime;

namespace Kapowey.Core.Common.Models.API
{
    public sealed class FileOperationResponse<T> : ServiceResponse<T>, IFileOperationResponse<T>
    {
        [JsonIgnore]
        public bool IsNotFoundResult { get; set; }

        public string ContentType { get; set; }

        public EntityTagHeaderValue ETag { get; set; }

        public Instant LastModified { get; set; }

        public FileOperationResponse(IServiceResponseMessage message)
            : base(message)
        {
        }

        public FileOperationResponse(IEnumerable<IServiceResponseMessage> messages)
            : base(messages)
        {
        }

        public FileOperationResponse(T data, IServiceResponseMessage message)
            : this(data, new IServiceResponseMessage[1] { message })
        {
        }

        public FileOperationResponse(T data, IEnumerable<IServiceResponseMessage> messages)
            : base(data, messages)
        {
        }

        public FileOperationResponse(bool isNotFoundResult, IServiceResponseMessage message)
            : base(message)
        {
            IsNotFoundResult = isNotFoundResult;
        }
    }
}