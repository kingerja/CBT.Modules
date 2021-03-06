﻿using NuGet.Configuration;
using System.Collections.Generic;

namespace CBT.NuGet.Internal
{
    /// <inheritdoc />
    /// <summary>
    /// Provides a base implementation of <see cref="T:CBT.Core.Internal.INuGetPackageConfigParser" />.
    /// </summary>
    internal abstract class NuGetPackageConfigParserBase : INuGetPackageConfigParser
    {
        protected readonly CBTTaskLogHelper Log;

        protected readonly ISettings NuGetSettings;

        protected NuGetPackageConfigParserBase(ISettings settings, CBTTaskLogHelper log)
        {
            Log = log;
            NuGetSettings = settings;
        }

        public abstract bool TryGetPackages(string packageConfigPath, PackageRestoreData packageRestoreData, out IEnumerable<PackageIdentityWithPath> packages);
    }
}