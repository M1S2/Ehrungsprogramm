using System;
using System.Diagnostics;
using System.Reflection;

using Ehrungsprogramm.Contracts.Services;

namespace Ehrungsprogramm.Services
{
    public class ApplicationInfoService : IApplicationInfoService
    {
        public ApplicationInfoService()
        {
        }

        public Version GetVersion()
        {
            // Set the app version in Ehrungsprogramm > Properties > Package > PackageVersion
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            var version = FileVersionInfo.GetVersionInfo(assemblyLocation).FileVersion;
            return new Version(version);
        }
    }
}
