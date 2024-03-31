using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;
using Game.UI.Widgets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.Scripting;
using CS2MapView.Exporter.System;

namespace CS2MapView.Exporter
{
    /// <summary>
    /// MOD設定
    /// </summary>
    [FileLocation(Mod.ModPackageName)]
    [SettingsUIGroupOrder(Key_UISectionOutputConfig, Key_UISectionPath, Key_UISectionExecution, Key_UISectionMisc)]
    [SettingsUIShowGroupName]
    public class CS2MapViewModSettings : ModSetting
    {

        internal const string Key_UISectionPath = "Path";

        internal const string Key_UISectionOutputConfig = "OutputConfig";
        internal const string Key_UISectionExecution = "Execution";
        internal const string Key_UISectionMisc = "Misc";
        [Preserve]
        public enum ResolutionRestriction
        {
            Width4096,
            Width2048,
            Width1024
        }

        private Mod? Mod { get; set; }
        /// <summary>
        /// heightmapの解像度制限
        /// </summary>
        [SettingsUISection(Key_UISectionOutputConfig)]
        public ResolutionRestriction HeightMapResolutionRestriction { get; set; }

        [SettingsUISection(Key_UISectionOutputConfig)]
        public bool AddFileNameTimestamp { get; set; }

        private string? _outputPath;
        [SettingsUISection(Key_UISectionPath)]
        public string? OutputPath => _outputPath;

        [SettingsUISection(Key_UISectionExecution)]
        [SettingsUIButton]
        public bool Export
        {
            set
            {
                _exportResult = "";
                var sys = CS2MapViewSystem.Instance;
                if(sys is null)
                {
                    _exportResult = "failed.";
                    return;
                }
                Task<string?> task = sys.RunExport(OutputPath, HeightMapResolutionRestriction, AddFileNameTimestamp);
                task?.Wait();
                var result = task?.Result;
                if (!(result is null))
                {
                    _exportResult = $"created {result}.";
                }
                else
                {
                    _exportResult = "failed.";
                }
                ApplyAndSave();
            }
        }

        private string? _exportResult;
        [SettingsUISection(Key_UISectionExecution)]
        public string? ExportResult => _exportResult;



        [SettingsUISection(Key_UISectionMisc)]
        public bool ResetSettings { set => SetDefaults(); }

        public CS2MapViewModSettings(IMod mod) : base(mod)
        {
            Mod = mod as Mod;
            SetDefaults();

        }

        public override void SetDefaults()
        {
            var spDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (spDir is null)
            {
                _outputPath = string.Empty;
            }
            else
            {
                _outputPath = Path.Combine(spDir, "CS2MapView");
            }

            if (!Directory.Exists(_outputPath))
            {
                Directory.CreateDirectory(_outputPath);
            }
            HeightMapResolutionRestriction = ResolutionRestriction.Width1024;
            AddFileNameTimestamp = true;

        }
    }

}
