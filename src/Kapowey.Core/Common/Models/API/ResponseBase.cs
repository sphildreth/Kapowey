namespace Kapowey.Core.Common.Models.API
{
    public abstract class ResponseBase
    {
        private List<IServiceResponseMessage> _messages = new List<IServiceResponseMessage>();

        public bool IsSuccess => _messages?.All(x => x.MessageType == ServiceResponseMessageType.Ok) ?? true;

        public IEnumerable<IServiceResponseMessage> Messages => _messages;

        public ResponseBase()
        {
        }

        public ResponseBase(IServiceResponseMessage message)
           : this(new IServiceResponseMessage[1] { message })
        {
        }

        public ResponseBase(IEnumerable<IServiceResponseMessage> messages)
        {
            _messages = messages?.ToList();
        }

        public void AddMessage(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                (_messages ?? (_messages = new List<IServiceResponseMessage>())).Add(new ServiceResponseMessage(message));
            }
        }
    }
}