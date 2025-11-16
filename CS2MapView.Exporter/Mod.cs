using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using CS2MapView.Exporter.Systems;
using Game;
using Game.Modding;
using Game.SceneFlow;
using Unity.Entities;
namespace CS2MapView.Exporter
{
    public class Mod : IMod
    {
        internal const string ModPackageName = "CS2MapView.Exporter";
        public static ILog log = LogManager.GetLogger(ModPackageName).SetShowsErrorsInUI(false);
        private CS2MapViewModSettings? m_Setting;

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));

            SetupConfig();
            //updateしてもらう必要ないけども
            updateSystem.UpdateAfter<CS2MapViewSystem>(SystemUpdatePhase.Serialize);
        }

        private void SetupConfig()
        {
            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
            {
                log.Info($"Current mod asset at {asset.path}");
            }
            m_Setting = new CS2MapViewModSettings(this);
            m_Setting.RegisterInOptionsUI();
            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEn(m_Setting));
            GameManager.instance.localizationManager.AddSource("ja-JP", new LocaleJa(m_Setting));

            AssetDatabase.global.LoadSettings(ModPackageName, m_Setting, new CS2MapViewModSettings(this));

           
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
            if (m_Setting != null)
            {
                m_Setting.UnregisterInOptionsUI();
                m_Setting = null;
            }
        }
    }
}
