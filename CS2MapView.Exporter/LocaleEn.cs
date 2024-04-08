using Colossal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SC = CS2MapView.Exporter.CS2MapViewModSettings;

namespace CS2MapView.Exporter
{
    public class LocaleEn : IDictionarySource
    {
        private readonly Dictionary<string, string> Data;
        public LocaleEn(Game.Modding.ModSetting s)
        {
            Data = new Dictionary<string, string>
            {
                { s.GetSettingsLocaleID(),                                  "CS2MapView" },
                { s.GetOptionGroupLocaleID(SC.Key_UISectionOutputConfig),   "Output settings" },
                { s.GetOptionGroupLocaleID(SC.Key_UISectionPath),           "Pathes" },
                { s.GetOptionGroupLocaleID(SC.Key_UISectionExecution),      "Execution"},
                { s.GetOptionGroupLocaleID(SC.Key_UISectionMisc),           "Misc"},

                { s.GetOptionLabelLocaleID(nameof(SC.HeightMapResolutionRestriction)), "Maximum terrain output resolution" },
                { s.GetOptionDescLocaleID(nameof(SC.HeightMapResolutionRestriction)), "By decreasing the resolution of the terrain and water information output, the output file size will be reduced." },
                { s.GetOptionLabelLocaleID(nameof(SC.AddFileNameTimestamp)), "Add timestamp to file name" },
                { s.GetOptionDescLocaleID(nameof(SC.AddFileNameTimestamp)), "Add timestamp to file name." },
                { s.GetOptionLabelLocaleID(nameof(SC.OutputPath)),          string.Empty },
                { s.GetOptionDescLocaleID(nameof(SC.OutputPath)),           string.Empty },
                { s.GetOptionLabelLocaleID(nameof(SC.OutputPathType)),      "Export file path" },
                { s.GetOptionDescLocaleID(nameof(SC.OutputPathType)),       "Export file path" },
                { s.GetOptionLabelLocaleID(nameof(SC.ResetSettings)),        "Reset" },
                { s.GetOptionDescLocaleID(nameof(SC.ResetSettings)),        "Reset setting." },
                { s.GetOptionLabelLocaleID(nameof(SC.Export)),              "Export"},
                { s.GetOptionDescLocaleID(nameof(SC.Export)),               "Outputs a map file for CS2MapView."},
                { s.GetOptionLabelLocaleID(nameof(SC.ExportResult)),        "Status"},
                { s.GetOptionDescLocaleID(nameof(SC.ExportResult)), string.Empty },

                { s.GetEnumValueLocaleID(SC.PathType.Documents), "Documents" },
                { s.GetEnumValueLocaleID(SC.PathType.Desktop), "Desktop" },
                { s.GetEnumValueLocaleID(SC.PathType.AppData), "LocalLow" },
                { s.GetEnumValueLocaleID(SC.ResolutionRestriction.Width4096) , "4096x4096" },
                { s.GetEnumValueLocaleID(SC.ResolutionRestriction.Width2048) , "2048x2048" },
                { s.GetEnumValueLocaleID(SC.ResolutionRestriction.Width1024) , "1024x1024" }


            };
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            foreach (KeyValuePair<string, string> entry in Data)
            {
                yield return entry;
            }
        }

        public void Unload()
        {
            Data.Clear();
        }
    }
}
