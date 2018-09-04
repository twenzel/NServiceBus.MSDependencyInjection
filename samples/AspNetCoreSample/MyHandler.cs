using System.Threading.Tasks;
using NServiceBus;

namespace Sample.Core
{
    public class MyHandler : IHandleMessages<MyMessage>
    {
        private MyService _myService;

        public MyHandler(MyService myService)
        {
            _myService = myService;
        }

        public Task Handle(MyMessage message, IMessageHandlerContext context)
        {
            _myService.WriteHello();
            return Task.CompletedTask;
        }
    }
}
