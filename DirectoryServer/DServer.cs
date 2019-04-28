using SharedLogic;

namespace DirectoryServer
{
    public static class DServer
    {
        private static Logger Logger { get; } = new Logger(nameof(DServer));
        
        public static int Main()
        {
            Logger.Info(null, "Initializing...");
            return new DRuntime().InitializeLifetimeService();
        }
    }
}
