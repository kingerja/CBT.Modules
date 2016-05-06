﻿using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace CBT.NuGet.Tasks
{
    /// <summary>
    /// Pushes a package to the server and publishes it.
    ///
    /// NuGet's default configuration is obtained by loading %AppData%\NuGet\NuGet.config, then loading any nuget.config or .nuget\nuget.config starting from root of drive and ending in current directory.
    /// </summary>
    public sealed class NuGetPush : CommandBase
    {
        /// <summary>
        /// Gets or sets the API key for the server.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if buffering should be disabled when pushing to an HTTP(S) server to decrease memory usage. Note that when this option is enabled, integrated windows authentication might not work.
        /// </summary>
        public bool DisableBuffering { get; set; }

        /// <summary>
        /// Gets or sets the package path.
        /// </summary>
        [Required]
        public string Package { get; set; }

        /// <summary>
        /// Get or sets the server URL. If not specified, nuget.org is used unless DefaultPushSource config value is set in the NuGet config file.
        /// </summary>
        public string Source { get; set; }

        protected override void GenerateCommandLineCommands(CommandLineBuilder commandLineBuilder)
        {
            commandLineBuilder.AppendSwitch("push");

            commandLineBuilder.AppendFileNameIfNotNull(Package);

            commandLineBuilder.AppendSwitchIfNotNull("-Source ", Source);

            commandLineBuilder.AppendSwitchIfNotNull("-ApiKey ", ApiKey);

            commandLineBuilder.AppendSwitchIfTrue("-DisableBuffering", DisableBuffering);
        }
    }
}