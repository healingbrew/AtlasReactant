using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using SharedLogic;

namespace DirectoryServer
{
    [DataContract]
    internal class HeliumConfig
    {
        [IgnoreDataMember]
        public static HeliumConfig Instance { get; } = new HeliumConfig();
        
        [IgnoreDataMember]
        private static Logger Logger { get; } = new Logger(nameof(HeliumConfig));

        [DataMember] public HashSet<IPEndPoint> BindAddresses { get; private set; } = new HashSet<IPEndPoint>();

        [DataMember] public IPEndPoint TicketServer { get; private set; }

        [DataMember] public IPEndPoint LobbyMasterServer { get; private set; }

        public static void Parse()
        {
            var argv = Environment.GetCommandLineArgs();

            for (var i = 1; i < argv.Length; ++i)
            {
                switch (argv[i].ToLower())
                {
                    case "-b":
                    case "--bind":
                        Instance.BindAddresses.Add(null);
                        i += 1;
                        break;
                    
                    case "-t":
                    case "--ticket":
                        Instance.TicketServer = null;
                        i += 1;
                        break;
                    
                    case "-l":
                    case "--lobby":
                        Instance.LobbyMasterServer = null;
                        i += 1;
                        break;
                    
                    case "-h":
                    case "-?":
                    case "--help":
                        Logger.Info(null, "Usage: {0} [--bind ip-endpoint] [--ticket ticket-endpoint] [--lobby lobby-endpoint]", argv[0]);
                        Environment.Exit(0);
                        break;
                    
                    default:
                        Logger.Error(nameof(HeliumConfig), "Don't know how to parse {0}", argv[i]);
                        break;
                }
            }
        }
    }
}
