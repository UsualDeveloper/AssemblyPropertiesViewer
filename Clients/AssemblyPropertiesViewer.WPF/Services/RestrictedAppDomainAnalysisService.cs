using AssemblyPropertiesViewer.Analyzers.Filtering.Interfaces;
using AssemblyPropertiesViewer.Analyzers.Loader;
using AssemblyPropertiesViewer.Analyzers.Models;
using AssemblyPropertiesViewer.Analyzers.Models.Filtering.Interfaces;
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
        private object instanceAnalysisLock = new object();

        private readonly Type ProxyType = typeof(AssemblyProxy);

        private readonly ILogger logger;
        private readonly IFileSystemService fileSystemService;
        private readonly IFilterMatchVisitor filterMatchVisitor;

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

        

        public RestrictedAppDomainAnalysisService(ILogger logger, IFileSystemService fileSystemService, IFilterMatchVisitor filterMatchVisitor)
        {
            this.logger = logger;
            this.fileSystemService = fileSystemService;
            this.filterMatchVisitor = filterMatchVisitor;

            this.logger.InitializeLogger(typeof(RestrictedAppDomainAnalysisService));
        }

        public IEnumerable<AnalysisResult> InspectAssembly(string assemblyFilePath/*, IReadOnlyDictionary<string, IEnumerable<ISearchFilter>> searchCriteria = null*/)
        {
            //TODO: detect which analyzers to use based on active filters
            return RunInRestrictedDomain<IEnumerable<AnalysisResult>>(
                assemblyFilePath, 
                (proxy, filePath) => proxy.InspectAssembly(filePath));
        }
        
        public IReadOnlyDictionary<string, IEnumerable<AnalysisResult>>  InspectFolderAndFilterResults(string searchFolderPath, bool searchRecursively, IReadOnlyDictionary<string, IEnumerable<ISearchFilter>> searchCriteria)
        {
            var folderAnalysisResults = new Dictionary<string, IEnumerable<AnalysisResult>>();

            var filesToAnalyze = fileSystemService.GetAssembliesInDirectory(searchFolderPath, searchRecursively);
            foreach (var filePath in filesToAnalyze)
            {
                // TODO: analysis should be performed only for analyzers associated with active filters
                var analyzerResults = InspectAssembly(filePath);
                
                bool? isFileMatch = null;
                foreach (var fileAnalysisResult in analyzerResults)
                {
                    IEnumerable<ISearchFilter> filtersForAnalyzer;
                    if (!searchCriteria.TryGetValue(fileAnalysisResult.AnalyzerTypeFullName, out filtersForAnalyzer))
                    {
                        continue;
                    }

                    foreach (var filter in filtersForAnalyzer)
                    {
                        if (!filter.IsFilterEnabled)
                        {
                            continue;
                        }

                        filterMatchVisitor.InitializeVisitorForAccept(fileAnalysisResult);
                        filter.Accept(filterMatchVisitor);
                        
                        if (!isFileMatch.HasValue && filterMatchVisitor.IsAcceptedFilterMatching)
                        {
                            isFileMatch = true;
                        }
                        else if (!filterMatchVisitor.IsAcceptedFilterMatching)
                        {
                            isFileMatch = false;
                            break;
                        }
                    }
                    
                    // all filters applied has to match for an assembly to be considered as matched
                    if (isFileMatch.HasValue && !isFileMatch.Value)
                    {
                        break;
                    }
                }

                // if no filtering was applied or assembly matched all the filters, consider is as matched
                if (!isFileMatch.HasValue || isFileMatch.Value)
                {
                    folderAnalysisResults.Add(filePath, analyzerResults);
                }
            }

            return folderAnalysisResults;
        }

        public IReadOnlyDictionary<string, IEnumerable<ISearchFilter>> GetAvailableSearchFilters()
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

        private TResult RunInRestrictedDomain<TResult>(string assemblyFilePath, Func<AssemblyProxy, string, TResult> methodToInvoke)
        {
            try
            {
                if (methodToInvoke == null)
                {
                    throw new ArgumentNullException(nameof(methodToInvoke));
                }

                if (IsAnalysisInProgress)
                {
                    throw new InvalidOperationException("Analysis of an assembly is already in progress.");
                }

                IsAnalysisInProgress = true;

                logger.Info($"Started analysis of assembly {assemblyFilePath}...");
                logger.Info("Creating analysis sandbox...");

                var testDomain = CreateDomainWithRestrictedPermissions(assemblyFilePath);
                var proxy = GetAssemblyAnalyzingProxyForSeparateAppDomain(testDomain);

                logger.Info("Invoking custom method with prepared proxy...");
                var analysisResults = methodToInvoke(proxy, assemblyFilePath);

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
