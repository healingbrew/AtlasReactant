using SharedLogic;

namespace DirectoryServer
{
    public class DRuntime
    {
        private static Logger Logger { get; } = new Logger(nameof(DRuntime));

        public int InitializeLifetimeService()
        {
            Logger.Info(null, "Parsing arguments...");
            HeliumConfig.Parse();

            return 0;
        }
    }
}
