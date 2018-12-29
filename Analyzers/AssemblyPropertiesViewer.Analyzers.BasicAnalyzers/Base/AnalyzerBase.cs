using AssemblyPropertiesViewer.Analyzers.Models;
using AssemblyPropertiesViewer.Analyzers.Models.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AssemblyPropertiesViewer.Analyzers.BasicAnalyzers.Base
{
    [Serializable]
    public abstract class AnalyzerBase
    {
        protected const string NoValueFoundString = "/no value found/";

        private IEnumerable<CustomAttributeData> customAttributes;

        public virtual IEnumerable<ISearchFilter> GetSearchFilters()
        {
            return null;
        }

        protected U GetAssemblyAttributePropertyValueOrDefault<T, U>(Assembly assembly, string propertyName) where T : Attribute
        {
            CustomAttributeData selectedAttribute;
            if (!TryGetCustomAttribute<T>(assembly, out selectedAttribute))
            {
                return default(U);
            }

            var attributeArgumentMatched = selectedAttribute?.NamedArguments?.Where(i => i.MemberName == propertyName && i.TypedValue.ArgumentType == typeof(U));
            CustomAttributeNamedArgument? attributeArgument = attributeArgumentMatched.Any() ? (CustomAttributeNamedArgument?)attributeArgumentMatched.First() : null;

            if (!attributeArgument.HasValue)
            {
                return default(U);
            }

            return (U)(attributeArgument.Value.TypedValue.Value);
        }

        protected U GetAssemblyAttributeConstructorArgumentValueOrDefault<T, U>(Assembly assembly, int argumentIndex = 0) where T : Attribute
        {
            CustomAttributeData selectedAttribute;
            if (!TryGetCustomAttribute<T>(assembly, out selectedAttribute))
            {
                return default(U);
            }

            CustomAttributeTypedArgument? attributeArgument = selectedAttribute?.ConstructorArguments?[argumentIndex];
            
            if (!attributeArgument.HasValue || attributeArgument.Value.ArgumentType != typeof(U))
            {
                return default(U);
            }

            return (U)(attributeArgument.Value.Value);
        }

        private bool TryGetCustomAttribute<T>(Assembly assembly, out CustomAttributeData selectedAttribute)
        {
            selectedAttribute = null;

            if (customAttributes == null)
            {
                customAttributes = CustomAttributeData.GetCustomAttributes(assembly);
            }

            selectedAttribute = customAttributes.SingleOrDefault(a => a.AttributeType == typeof(T));
            return (selectedAttribute != null);
        }
    }
}
