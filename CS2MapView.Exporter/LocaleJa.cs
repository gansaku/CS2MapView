using Colossal;
using System.Collections.Generic;
using SC = CS2MapView.Exporter.CS2MapViewModSettings;
namespace CS2MapView.Exporter
{
    public class LocaleJa : IDictionarySource
    {
        private readonly Dictionary<string, string> Data;
        public LocaleJa(Game.Modding.ModSetting s)
        {
            Data = new Dictionary<string, string>
            {
                { s.GetSettingsLocaleID(),                                  "CS2MapView" },
                { s.GetOptionGroupLocaleID(SC.Key_UISectionOutputConfig),   "出力設定" },
                { s.GetOptionGroupLocaleID(SC.Key_UISectionPath),           "パス" },
                { s.GetOptionGroupLocaleID(SC.Key_UISectionExecution),      "実行"},
                { s.GetOptionGroupLocaleID(SC.Key_UISectionMisc),           "その他"},

                { s.GetOptionLabelLocaleID(nameof(SC.HeightMapResolutionRestriction)), "地形出力解像度（最大）" },
                { s.GetOptionDescLocaleID(nameof(SC.HeightMapResolutionRestriction)), "地形・水域情報出力の解像度を落とすことにより、出力ファイルサイズが低下します。" },
                { s.GetOptionLabelLocaleID(nameof(SC.AddFileNameTimestamp)), "ファイル名のタイムスタンプ" },
                { s.GetOptionDescLocaleID(nameof(SC.AddFileNameTimestamp)), "ファイル名に日付・時刻を付与します。" },
                { s.GetOptionLabelLocaleID(nameof(SC.OutputPath)),          string.Empty },
                { s.GetOptionDescLocaleID(nameof(SC.OutputPath)),           string.Empty},
                { s.GetOptionLabelLocaleID(nameof(SC.OutputPathType)),      "出力先" },
                { s.GetOptionDescLocaleID(nameof(SC.OutputPathType)),       "ファイルの出力先パスを選択します。" },
                { s.GetOptionLabelLocaleID(nameof(SC.ResetSettings)),        "リセット" },
                { s.GetOptionDescLocaleID(nameof(SC.ResetSettings)),        "設定をリセットします。" },
                { s.GetOptionLabelLocaleID(nameof(SC.Export)),              "エクスポート"},
                { s.GetOptionDescLocaleID(nameof(SC.Export)),               "CS2MapViewで読み込むためのマップファイルを出力します。"},
                { s.GetOptionLabelLocaleID(nameof(SC.ExportResult)),        "実行結果"},
                { s.GetOptionDescLocaleID(nameof(SC.ExportResult)), string.Empty },

                { s.GetEnumValueLocaleID(SC.PathType.Custom), "カスタム" },
                { s.GetEnumValueLocaleID(SC.PathType.Documents), "ドキュメント" },
                { s.GetEnumValueLocaleID(SC.PathType.Desktop), "デスクトップ" },
                { s.GetEnumValueLocaleID(SC.PathType.AppData), "LocalLow" },
                { s.GetEnumValueLocaleID(SC.ResolutionRestriction.Width4096) , "4096x4096" },
                { s.GetEnumValueLocaleID(SC.ResolutionRestriction.Width2048) , "2048x2048" },
                { s.GetEnumValueLocaleID(SC.ResolutionRestriction.Width1024) , "1024x1024" },

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
