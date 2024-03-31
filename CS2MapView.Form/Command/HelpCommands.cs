using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using CS2MapView.Serialization;

namespace CS2MapView.Form.Command
{
    internal class HelpCommands
    {
        private MainForm MainForm { get; init; }

        internal HelpCommands(MainForm mainForm)
        {
            MainForm = mainForm;
        }
        internal void ShowVersionInfo()
        {
            var coreAssembly = Assembly.GetAssembly(typeof(ICS2MapViewRoot))?.GetName();
            var formAssembly = Assembly.GetAssembly(typeof(MainForm))?.GetName();
            var serializationAssembly = Assembly.GetAssembly(typeof(CS2MainData))?.GetName();
      
            var str = $"{formAssembly?.Name} : {formAssembly?.Version}\r\n{coreAssembly?.Name} : {coreAssembly?.Version}\r\n{serializationAssembly?.Name} : {serializationAssembly?.Version}";
            MessageBox.Show(str, "cs2mapview", MessageBoxButtons.OK);
        }
    }
}
