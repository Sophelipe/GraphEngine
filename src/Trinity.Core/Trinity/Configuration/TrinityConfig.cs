// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using Trinity.Network;
using Trinity.Diagnostics;
using Trinity.Core.Lib;
using System.IO;
using System.Runtime.InteropServices;
using Trinity.Configuration;
using Trinity.Utilities;
using System.Linq;

namespace Trinity
{
    /// <summary>
    /// Specifies the running modes supported by Trinity. This is obsolete.
    /// </summary>
    public enum RunningMode : int
    {
        /// <summary>
        /// Undefined running mode.
        /// </summary>
        Undefined,

        /// <summary>
        ///Embedded (in-process) mode.
        /// </summary>
        Embedded,

        /// <summary>
        /// As a server.
        /// </summary>
        Server,

        /// <summary>
        /// As a proxy.
        /// </summary>
        Proxy,

        /// <summary>
        /// As a client.
        /// </summary>
        Client,
    }

    /// <summary>
    /// A class for accessing and manipulating various system parameters.
    /// </summary>
    public unsafe static partial class TrinityConfig
    {
        #region Fields
        internal static bool is_config_loaded = false;
        private static object config_load_lock = new object();
        internal const bool RWTimeout = false;
        #endregion

        /// <summary>
        /// Static constructor
        /// </summary>
        static TrinityConfig()
        {
            GetConfigurationInstances().ToList();
            LoadTrinityConfig(false);
        }

        /// <summary>
        /// Gets or sets the running mode of current Trinity process.
        /// This property is obsolete and is kept for backward compatibility.
        /// Assigning a running mode to this property does not affect the system behavior.
        /// </summary>
        [Obsolete("TrinityConfig.CurrentRunningMode is deprecated.")]
        public static RunningMode CurrentRunningMode { get { return s_current_cluster_config.RunningMode; } set { s_current_cluster_config.RunningMode = value; } }

        /// <summary>
        /// Current system logging logLevel, default is LogLevel.Info.
        /// </summary>
        public static LogLevel LoggingLevel
        {
            get { return LoggingConfig.Instance.LoggingLevel; }
            set { LoggingConfig.Instance.LoggingLevel = value; }
        }

        /// <summary>
        /// Sets the directory for writing logs into.
        /// </summary>
        internal static string LogDirectory
        {
            get { return LoggingConfig.Instance.LogDirectory; }
            set { LoggingConfig.Instance.LogDirectory = value; }
        }
        
        /// <summary>
        /// Gets and Sets the bool value to represent whether to echo log entry on the standard output.
        /// </summary>
        public static bool LogEchoOnConsole
        {
            get { return LoggingConfig.Instance.LogEchoOnConsole; }
            set { LoggingConfig.Instance.LogEchoOnConsole = value; }
        }

        /// <summary>
        /// Gets and Sets the bool value to represent whether to stream log entries to a file on disk.
        /// </summary>
        public static bool LogToFile
        {
            get { return LoggingConfig.Instance.LogToFile; }
            set { LoggingConfig.Instance.LogToFile = value; }
        }
        /// <summary>
        /// Provides a local testing configuration with one server (localhost).
        /// </summary>
        internal static bool LocalTest
        {
            set
            {
                if (value)
                {
                    Servers.Clear();
                    Proxies.Clear();

                    ServerInfo server = ServerInfo._LegacyCreateServerInfo("127.0.0.1",
                        TrinityConfig.CurrentClusterConfig.ServerPort,
                        AssemblyPath.MyAssemblyPath,
                        AssemblyPath.MyAssemblyPath + "storage\\",
                        LogLevel.Debug.ToString(),
                        "0");

                    switch (CurrentClusterConfig.RunningMode)
                    {
                        case RunningMode.Server:
                            TrinityConfig.AddServer(server);
                            break;
                        case RunningMode.Proxy:
                            TrinityConfig.AddProxy(server);
                            break;
                    }
                }
            }
        }
    }
}
