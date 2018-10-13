using AssemblyPropertiesViewer.Analyzers.Filtering.Interfaces;
using AssemblyPropertiesViewer.Analyzers.Models;
using AssemblyPropertiesViewer.Analyzers.Models.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AssemblyPropertiesViewer.Analyzers.Filtering
{
    /// <summary>
    /// Visitor responsible for determining if analysis results satisfy specific search criteria based on selected filter type and its settings.
    /// </summary>
    public class FilterMatchVisitor : IFilterMatchVisitor
    {
        public bool IsAcceptedFilterMatching
        {
            get
            {
                if (!isVisited)
                    throw new InvalidOperationException("The visitor was not yet visited and matching results are not yet available.");

                return isAcceptedFilterMatching;
            }
        }

        private bool isAcceptedFilterMatching = false;
        private bool isVisited = false;
        private AnalysisResult fileAnalysisResult = null;

        public void InitializeVisitorForAccept(AnalysisResult fileAnalysisResult)
        {
            if (fileAnalysisResult == null)
            {
                throw new ArgumentNullException(nameof(fileAnalysisResult));
            }

            this.fileAnalysisResult = fileAnalysisResult;

            isAcceptedFilterMatching = false;
            isVisited = false;
        }

        public void Visit(DropDownFilter filter)
        {
            AssertWasInitialized();
            
            isAcceptedFilterMatching = (filter.SelectedValue == fileAnalysisResult.Value);

            MarkWasVisited();
        }

        public void Visit(BooleanFilter filter)
        {
            AssertWasInitialized();
            bool fileDetectedValue;
            if (bool.TryParse(fileAnalysisResult.Value, out fileDetectedValue))
            {
                isAcceptedFilterMatching = (filter.IsSelected == fileDetectedValue);
            }
            else
            {
                isAcceptedFilterMatching = false;
            }
            
            MarkWasVisited();
        }

        public void Visit(StringFilter filter)
        {
            AssertWasInitialized();

            if (string.IsNullOrEmpty(fileAnalysisResult.Value))
            {
                isAcceptedFilterMatching = false;
            }
            else
            {
                if (filter.FullPatternMatchOnly)
                {
                    isAcceptedFilterMatching = (filter.MatchPattern == fileAnalysisResult.Value);
                }
                else
                {
                    Regex regex = new Regex(filter.MatchPattern ?? ".*");
                    isAcceptedFilterMatching = (regex.IsMatch(fileAnalysisResult.Value));
                }
            }

            MarkWasVisited();
        }
        
        private void AssertWasInitialized()
        {
            if (fileAnalysisResult == null)
            {
                throw new InvalidOperationException("The visitor instance was not initialized correctly");
            }
        }

        private void MarkWasVisited()
        {
            this.isVisited = true;
        }
    }
}
