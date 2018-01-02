﻿using AssemblyPropertiesViewer.Analyzers.Models;
using System.Collections.Generic;

namespace AssemblyPropertiesViewer.Services.Interfaces
{
    public interface IAssemblyAnalysisService
    {
        bool IsAnalysisInProgress { get; }

        IEnumerable<AnalysisResult> InspectAssembly(string assemblyFilePath);

        long GetFileSize(string filePath);
    }
}
