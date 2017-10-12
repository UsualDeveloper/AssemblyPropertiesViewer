using System;
using System.Collections.Generic;

namespace AssemblyPropertiesViewer.Analyzers.Loader
{
    public class BasicAnalyzersLoader
    {
        private readonly List<Type> basicAnalyzerTypes;

        public BasicAnalyzersLoader()
        {
            basicAnalyzerTypes = new List<Type>()
            {
                // set of basic analyzers is not to be changed often until the loading mechanism is rewritten
                // TODO: to be rewritten to load analyzer types dynamically based on specific criteria
                typeof(BasicAnalyzers.AssemblyFrameworkAnalyzer),
                typeof(BasicAnalyzers.DebugOrReleaseModeAnalyzer),
                typeof(BasicAnalyzers.AssemblyFullNameAnalyzer), 
                typeof(BasicAnalyzers.ChecksumAnalyzer), 
                typeof(BasicAnalyzers.AssemblyStrongNameAnalyzer)
            };
        }

        public IEnumerable<Type> GetBasicAnalyzers()
        {
            return basicAnalyzerTypes.ToArray();
        }
    }
}
