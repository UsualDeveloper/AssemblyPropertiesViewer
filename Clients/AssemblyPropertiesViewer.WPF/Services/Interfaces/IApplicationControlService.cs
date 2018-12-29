using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyPropertiesViewer.Services.Interfaces
{
    public interface IApplicationControlService
    {
        /// <summary>
        /// Closes the entire application.
        /// </summary>
        void CloseApplication();
    }
}
