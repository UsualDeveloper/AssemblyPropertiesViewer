/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:AssemblyPropertiesViewer"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using AssemblyPropertiesViewer.Core.Interfaces;
using AssemblyPropertiesViewer.Core.Logger;
using AssemblyPropertiesViewer.Services;
using AssemblyPropertiesViewer.Services.Interfaces;
using AssemblyPropertiesViewer.ViewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace AssemblyPropertiesViewer
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
                return;
            
            RegisterServices();
            RegisterViewModels();
        }

        private void RegisterServices()
        {
            SimpleIoc.Default.Register<IAssemblyAnalysisService, RestrictedAppDomainAnalysisService>();
            SimpleIoc.Default.Register<ILogger, BasicLogger>();
            SimpleIoc.Default.Register<IWindowService, WindowService>();
            SimpleIoc.Default.Register<IFilteringControlCreationService, FilterDefinitionControlCreationVisitor>();
        }

        private void RegisterViewModels()
        {
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<FolderSearchCriteriaViewModel>();
        }
        
        // TODO: make this part of code more open to extending with other view models
        public MainViewModel MainViewModel
        {
            get
            {
                return SimpleIoc.Default.GetInstance<MainViewModel>();
            }
        }

        public FolderSearchCriteriaViewModel FolderSearchCriteriaViewModel
        {
            get
            {
                return SimpleIoc.Default.GetInstance<FolderSearchCriteriaViewModel>();
            }
        }

        public static void Cleanup()
        {
            SimpleIoc.Default.Unregister<IAssemblyAnalysisService>();
            SimpleIoc.Default.Unregister<ILogger>();
        }
    }
}