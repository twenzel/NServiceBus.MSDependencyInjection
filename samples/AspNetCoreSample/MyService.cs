using NServiceBus.Logging;

namespace Sample.Core
{
    public class MyService
    {
        static ILog log = LogManager.GetLogger<MyService>();

        public void WriteHello()
        {
            log.Info("Hello from MyService.");
        }
    }
}
