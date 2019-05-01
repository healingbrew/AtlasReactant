using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using SharedLogic;

namespace DirectoryServer
{
    // The Config Model of AtlasReactor is called HydrogenConfig
    [DataContract]
    internal class HeliumConfig
    {
        [PublicAPI]
        [IgnoreDataMember]
        public static HeliumConfig Instance { get; } = new HeliumConfig();
        
        private static HeliumConfig DefaultInstance { get; } = new HeliumConfig();
        
        [IgnoreDataMember]
        private static Logger Logger { get; } = new Logger(nameof(HeliumConfig));

        [DataMember] public IPAddress BindAddress { get; private set; } = new IPAddress(0);

        [DataMember] public int BindPort { get; private set; } = 6050;

        [DataMember] public string LobbyMasterServer { get; private set; } = "127.0.0.1:6060";

        [DataMember] public string CertificatePath { get; private set; }

        public static void Parse()
        {
            var argv = Environment.GetCommandLineArgs();

            for (var i = 1; i < argv.Length; ++i)
            {
                switch (argv[i].ToLower())
                {
                    case "-b":
                    case "--bind":
                    {
                        if (argv.Length <= i + 1)
                        {
                            Logger.Fatal(null, $"{argv[i]} must have a value");
                            return;
                        }

                        Instance.BindAddress = IPAddress.Parse(argv[i + 1]);
                        i += 1;
                        break;
                    }
                    
                    case "-l":
                    case "--lobby":
                    {
                        if (argv.Length <= i + 1)
                        {
                            Logger.Fatal(null, $"{argv[i]} must have a value");
                            return;
                        }

                        Instance.LobbyMasterServer = argv[i + 1];
                        i += 1;
                        break;
                    }
                    
                    case "-c":
                    case "--cert":
                    {
                        if (argv.Length <= i + 1)
                        {
                            Logger.Fatal(null, $"{argv[i]} must have a value");
                            return;
                        }

                        Instance.CertificatePath = argv[i + 1];
                        i += 1;
                        break;
                    }
                    
                    case "-p":
                    case "--port":
                    {
                        if (argv.Length <= i + 1)
                        {
                            Logger.Fatal(null, $"{argv[i]} must have a value");
                            return;
                        }

                        Instance.BindPort = int.Parse(argv[i + 1]);
                        i += 1;
                        break;
                    }
                    
                    case "-h":
                    case "-?":
                    case "--help":
                        Logger.Info(null, $"Usage: {Path.GetFileName(argv[0])} [--bind ip] [--port port] [--lobby address] [--cert path] [--help] [--version]");
                        Logger.Info(null, $"Usage: {Path.GetFileName(argv[0])} [-b ip] [-p port] [-l address] [-c path] [-h] [-?] [-v]");
                        if (argv[i] == "--help")
                        {
                            Logger.Info(null, $"\t--bind, -b: IP Address to bind to ({DefaultInstance.BindAddress?.ToString() ?? "null"})");
                            Logger.Info(null, $"\t--port, -p: Port to bind to ({DefaultInstance.BindPort})");
                            Logger.Info(null, $"\t--lobby, -l: Lobby address to provide new clients ({DefaultInstance.LobbyMasterServer ?? "null"})");
                            Logger.Info(null, $"\t--cert, -c: PFX Certificate path for HTTPS ({DefaultInstance.CertificatePath ?? "null"})");
                            Logger.Info(null, $"\t--help, -h, -?: Print this help and exit");
                            Logger.Info(null, $"\t--version, -v: Print version and exit");
                        }
                        Environment.Exit(0);
                        break;

                    case "-v":
                    case "--version":
                    {
                        Console.Error.WriteLine($"DirectoryServer=DServer/{Assembly.GetExecutingAssembly().GetName().Version}, AtlasReactant/{typeof(Logger).Assembly.GetName().Version}");
                        Environment.Exit(0);
                        break;
                    }
                    
                    default:
                        Logger.Error(nameof(HeliumConfig), "Don't know how to parse {0}", argv[i]);
                        break;
                }
            }
        }
    }
}
