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

        public bool IsAnalysisInProgress
        {
            get
            {
                lock (instanceAnalysisLock)
                {
                    return isAnalysisInProgress;
                }
            }
            private set
            {
                lock (instanceAnalysisLock)
                {
                    isAnalysisInProgress = value;
                }
            }
        }
        
        private bool isAnalysisInProgress = false;

        private object instanceAnalysisLock = new object();

        public RestrictedAppDomainAnalysisService(ILogger logger)
        {
            this.logger = logger;

            this.logger.InitializeLogger(typeof(RestrictedAppDomainAnalysisService));
        }

        public IEnumerable<AnalysisResult> InspectAssembly(string assemblyFilePath)
        {
            try
            {
                if (IsAnalysisInProgress)
                {
                    throw new InvalidOperationException("Analysis of an assembly is already in progress.");
                }

                IsAnalysisInProgress = true;

                logger.Info($"Started analysis of assembly {assemblyFilePath}...");
                logger.Info("Creating analysis sandbox...");

                var testDomain = CreateDomainWithRestrictedPermissions(assemblyFilePath);
                var proxy = GetAssemblyAnalyzingProxyForSeparateAppDomain(testDomain);

                logger.Info("Inspecting assemblies with available analyzers...");
                var analysisResults = proxy.InspectAssembly(assemblyFilePath);

                logger.Info("Closing the sandbox...");
                AppDomain.Unload(testDomain);
                
                logger.Info("Assembly analysis completed successfully.");
                return analysisResults;
            }
            finally
            {
                IsAnalysisInProgress = false;
            }
        }

        private AppDomain CreateDomainWithRestrictedPermissions(string additionalReadAccessAssemblyFilePath = null)
        {
            var appDomainSetup = new AppDomainSetup();
            // TODO: to be verified
            appDomainSetup.ApplicationBase = Path.GetFullPath(@"./Analyzers/");

            var permissionSet = GetRestrictedPermissionSet(additionalReadAccessAssemblyFilePath);
            return AppDomain.CreateDomain("testDomain", null, appDomainSetup, permissionSet, null);
        }

        public long GetFileSize(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            return fileInfo.Length;
        }

        public void InspectFolderAndFilterResults(string searchFolderPath, bool searchRecursively, IReadOnlyDictionary<Type, IEnumerable<ISearchFilter>> searchCriteria)
        {
            // TODO: implement
            return;
        }

        public IReadOnlyDictionary<Type, IEnumerable<ISearchFilter>> GetAvailableSearchFilters()
        {
            logger.Info("Creating analysis sandbox...");

            var testDomain = CreateDomainWithRestrictedPermissions();
            var proxy = GetAssemblyAnalyzingProxyForSeparateAppDomain(testDomain);

            logger.Info("Retrieving available search filters...");
            var filters = proxy.GetAvailableSearchFilters();

            logger.Info("Closing the sandbox...");
            AppDomain.Unload(testDomain);

            return filters;
        }

        private PermissionSet GetRestrictedPermissionSet(string additionalReadAccessAssemblyFilePath = null)
        {
            //create restricted permission set, based on https://msdn.microsoft.com/en-us/library/bb763046(v=vs.110).aspx
            var permissionSet = new PermissionSet(PermissionState.None);
            permissionSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));

            if (!string.IsNullOrEmpty(additionalReadAccessAssemblyFilePath))
            {
                permissionSet.AddPermission(new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery, additionalReadAccessAssemblyFilePath));
            }

            return permissionSet;
        }

        private AssemblyProxy GetAssemblyAnalyzingProxyForSeparateAppDomain(AppDomain appDomain)
        {
            return (AssemblyProxy)appDomain.CreateInstanceAndUnwrap(ProxyType.Assembly.FullName, ProxyType.FullName);
        }
    }
}
