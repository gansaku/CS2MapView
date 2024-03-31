using log4net.Config;
using System.Runtime.CompilerServices;

namespace CS2MapView
{
    internal static class ModuleInitializer
    {
        [ModuleInitializer]
        internal static void InitLog4Net()
        {
            XmlConfigurator.Configure(new FileInfo(Path.Combine(AppContext.BaseDirectory, "log4net.xml")));
        }
    }
}
