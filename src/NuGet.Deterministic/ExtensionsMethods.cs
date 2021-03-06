﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NuGet.LibraryModel;
using NuGet.Packaging.Core;
using NuGet.ProjectModel;

namespace NuGet.Tasks.Deterministic
{
    public static class ExtensionsMethods
    {
        public static void LogErrorFromText(this TaskLoggingHelper log, string code, string message, params object[] args)
        {
            log.LogError(
                subcategory: null, 
                errorCode: code,
                helpKeyword: null, 
                file: null,
                lineNumber: 0,
                columnNumber: 0,
                endLineNumber: 0,
                endColumnNumber: 0,
                message: message,
                messageArgs: args
            );
        }

        public static void LogWarningFromText(this TaskLoggingHelper log, string code, string message, params object[] args)
        {
            log.LogWarning(
                subcategory: null,
                warningCode: code,
                helpKeyword: null,
                file: null,
                lineNumber: 0,
                columnNumber: 0,
                endLineNumber: 0,
                endColumnNumber: 0,
                message: message,
                messageArgs: args
            );
        }

        public static string GetMetadataNoThrow(this ITaskItem taskItem, string metadataName)
        {
            try
            {
                return taskItem.GetMetadata(metadataName);
            }
            catch (KeyNotFoundException)
            {
                return default(string);
            }
        }

        public static LockFileLibrary GetLibrary(this LockFile lockFile, LibraryDependency libraryDependency)
        {
            // lockFile.GetLibrary is case sensitive.  So if a package is referenced in one case but another package list it as a dependency as another case it will fail to look up the library.
            // https://github.com/NuGet/Home/issues/6500
            // In the event NuGet resolved with a warning of:
            //   NUGETRESTORE : warning : NU1603: Microsoft.Aether.Logging.CentralLoggingService depends on NEST (>= 1.9.0) but NEST 1.9.0 was not found. An approximate best match of NEST 1.9.1 was resolved.
            // Then the lookup for GetLibrary will fail because LockFile.Libraries will actually contain the version resolved to where libraryDependency contains the version specified.  In this case because lockFile.Libraries will only ever contain one version we will just take that version blindly.
            return lockFile.GetLibrary(libraryDependency.Name, libraryDependency.LibraryRange.VersionRange.MinVersion) ??
                                     (lockFile.Libraries.FirstOrDefault(l => l.Name.Equals(libraryDependency.Name, StringComparison.OrdinalIgnoreCase) && l.Version.Equals(libraryDependency.LibraryRange.VersionRange.MinVersion)) ??
                                      lockFile.Libraries.FirstOrDefault(l => l.Name.Equals(libraryDependency.Name, StringComparison.OrdinalIgnoreCase)));
        }
        

        public static IEnumerable<LockFileLibrary> ResolveDependencies(this LockFileTargetLibrary targetLibrary, LockFile lockFile, LockFileTarget lockFileTarget)
        {
            foreach (PackageDependency dependency in targetLibrary.Dependencies)
            {
                LockFileTargetLibrary dependencyTargetLibrary = lockFileTarget.GetTargetLibrary(dependency.Id);

                yield return lockFile.GetLibrary(dependencyTargetLibrary.Name, dependencyTargetLibrary.Version);

                foreach (LockFileLibrary lockFileLibrary in dependencyTargetLibrary.ResolveDependencies(lockFile, lockFileTarget))
                {
                    yield return lockFileLibrary;
                }
            }
        }
    }
}