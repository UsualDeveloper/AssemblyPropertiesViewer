using AssemblyPropertiesViewer.Analyzers.Loader;
using AssemblyPropertiesViewer.Analyzers.Models;
using AssemblyPropertiesViewer.Core.Interfaces;
using AssemblyPropertiesViewer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Security.Permissions;

namespace AssemblyPropertiesViewer.Services
{
    internal class RestrictedAppDomainAnalysisService : IAssemblyAnalysisService
    {
        private readonly Type ProxyType = typeof(AssemblyProxy);

        private ILogger logger;

        public RestrictedAppDomainAnalysisService(ILogger logger)
        {
            this.logger = logger;

            this.logger.InitializeLogger(typeof(RestrictedAppDomainAnalysisService));
        }

        public IEnumerable<AnalysisResult> InspectAssembly(string assemblyFilePath)
        {
            logger.Info($"Started analysis of assembly {assemblyFilePath}...");
            logger.Info("Creating analysis sandbox...");

            var appDomainSetup = new AppDomainSetup();
            // TODO: to be verified
            appDomainSetup.ApplicationBase = Path.GetFullPath(@"./Analyzers/");

            var permissionSet = GetRestrictedPermissionSet(assemblyFilePath);
            var testDomain = AppDomain.CreateDomain("testDomain", null, appDomainSetup, permissionSet, null);
            var proxy = GetAssemblyAnalyzingProxyForSeparateAppDomain(testDomain, assemblyFilePath);

            logger.Info("Inspecting assemblies with available analyzers...");
            var analysisResults = proxy.InspectAssembly(assemblyFilePath);

            logger.Info("Closing the sandbox...");
            AppDomain.Unload(testDomain);

            logger.Info("Assembly analysis completed successfully.");
            return analysisResults;
        }

        public long GetFileSize(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            return fileInfo.Length;
        }

        private PermissionSet GetRestrictedPermissionSet(string assemblyFilePath)
        {
            //create restricted permission set, based on https://msdn.microsoft.com/en-us/library/bb763046(v=vs.110).aspx
            var permissionSet = new PermissionSet(PermissionState.None);
            permissionSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
            permissionSet.AddPermission(new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery, assemblyFilePath));

            return permissionSet;
        }

        private AssemblyProxy GetAssemblyAnalyzingProxyForSeparateAppDomain(AppDomain appDomain, string filePath)
        {
            return (AssemblyProxy)appDomain.CreateInstanceAndUnwrap(ProxyType.Assembly.FullName, ProxyType.FullName);
        }
    }
}
