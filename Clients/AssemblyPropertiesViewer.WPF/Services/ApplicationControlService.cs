using AssemblyPropertiesViewer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyPropertiesViewer.Services
{
    internal class ApplicationControlService : IApplicationControlService
    {
        public void CloseApplication()
        {
            App.Current.Shutdown();
        }
    }
}
