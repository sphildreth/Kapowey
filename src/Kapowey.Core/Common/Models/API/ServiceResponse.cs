namespace Kapowey.Core.Common.Models.API
{
    public class ServiceResponse<T> : ResponseBase, IServiceResponse<T>
    {
        public object Id { get; private set; }

        public T Data { get; private set; }

        public ServiceResponse()
            : base()
        {
        }

        public ServiceResponse(IServiceResponseMessage message)
           : this(default, new IServiceResponseMessage[1] { message })
        {
        }

        public ServiceResponse(IEnumerable<IServiceResponseMessage> messages)
           : this(default, messages)
        {
        }

        public ServiceResponse(T data, IServiceResponseMessage message)
           : this(data, new IServiceResponseMessage[1] { message })
        {
        }

        public ServiceResponse(T data, IEnumerable<IServiceResponseMessage> messages = null, object id = null)
            : base(messages)
        {
            Data = data;
            Id = id;
        }

        public void SetData(T data) => Data = data;

        public void SetId(object id) => Id = id;
    }
}