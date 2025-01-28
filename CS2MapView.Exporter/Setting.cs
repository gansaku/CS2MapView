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
        [Preserve]
        public enum PathType
        {
            Custom,
            Documents,
            Desktop,
            AppData
        }

        private Mod? Mod { get; set; }
        /// <summary>
        /// heightmapの解像度制限
        /// </summary>
        [SettingsUISection(Key_UISectionOutputConfig)]
        public ResolutionRestriction HeightMapResolutionRestriction { get; set; }

        [SettingsUISection(Key_UISectionOutputConfig)]
        public bool AddFileNameTimestamp { get; set; }

        private PathType _outputPathType = PathType.Documents;
        [SettingsUISection(Key_UISectionPath)]
        public PathType OutputPathType
        {
            get => _outputPathType;
            set
            {
                _outputPathType = value;
                _outputPath = GetPath(_outputPathType);
            }
        }

        private string? _outputPath;
        [SettingsUISection(Key_UISectionPath)]
        [SettingsUITextInput]
        [SettingsUIDisableByCondition(typeof(PathType), nameof(OutputPathEnabled), true)]
        public string? OutputPath
        {
            get => _outputPath;
            set
            {
                if (_outputPathType == PathType.Custom)
                {
                    OutputPathLastUserInput = value;
                }
                _outputPath = value;
            }
        }

        public bool OutputPathEnabled() => OutputPathType == PathType.Custom;

        [SettingsUIHidden]
        public string? OutputPathLastUserInput
        {
            get; set;
        }

        [SettingsUISection(Key_UISectionExecution)]
        [SettingsUIButton]
        public bool Export
        {
            set
            {
                _exportResult = "";
                var sys = CS2MapViewSystem.Instance;
                if (sys is null)
                {
                    _exportResult = "failed.";
                    return;
                }
                if (!Directory.Exists(OutputPath))
                {
                    Directory.CreateDirectory(OutputPath);
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
            _outputPathType = PathType.Documents;
            _outputPath = GetPath(_outputPathType);


            HeightMapResolutionRestriction = ResolutionRestriction.Width1024;
            AddFileNameTimestamp = true;

        }

        private string GetPath(PathType type)
        {
            string? result = null;
            switch (type)
            {
                case PathType.Documents:
                    result = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    break;
                case PathType.Desktop:
                    result = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    break;
                case PathType.AppData:
                    var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    result = local is null ? null : Path.GetFullPath(Path.Combine(local, @"..\LocalLow\Colossal Order\Cities Skylines II"));
                    break;
                case PathType.Custom:
                    if (OutputPathLastUserInput != null)
                    {
                        return OutputPathLastUserInput;
                    }
                    var drv = Environment.GetEnvironmentVariable("SystemDrive");
                    return drv ?? "/";
            }

            return result is null ? "CS2MapView" : Path.Combine(result, "CS2MapView");
        }
    }

}
