using AssemblyPropertiesViewer.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace AssemblyPropertiesViewer.Messages
{
    public class ShowPropertiesMessage : NotificationMessage<PropertiesViewModel>
    {
        public ShowPropertiesMessage(PropertiesViewModel content) : base(content, "Show results")
        { }
    }
}
