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
            return new Version(AssemblyInfoHelper.AssemblyInfoHelperClass.AssemblyVersion);
        }
    }
}
