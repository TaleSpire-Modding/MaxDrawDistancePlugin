using UnityEngine;
using BepInEx;
using BepInEx.Configuration;
using JetBrains.Annotations;
using ModdingTales;
using UnityEngine.SceneManagement;

namespace MaxDrawDistance
{

    [BepInPlugin(Guid, "HolloFoxe's MaxDrawDistance", Version)]
    public class MaxDrawDistance : BaseUnityPlugin
    {
        // constants
        private const string Guid = "org.hollofox.plugins.MaxDrawDistance";
        private const string Version = "1.2.0.0";

        private static ConfigEntry<ModdingUtils.LogLevel> LogLevelConfig { get; set; }
        private static ConfigEntry<float> MaxDraw { get; set; }
        private static ConfigEntry<float> MaxShadowDistance { get; set; }
        
        private static ModdingUtils.LogLevel LogLevel => LogLevelConfig.Value == ModdingUtils.LogLevel.Inherited ? ModdingUtils.LogLevelConfig.Value: LogLevelConfig.Value;

        public void DoConfig(ConfigFile config)
        {
            var logLevelDescription = new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdvanced = true });
            var maxDrawDescription = new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdvanced = false, CallbackAction = SetDrawDistance });
            var maxShadowDescription = new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdvanced = true, CallbackAction = SetShadowDistance });

            LogLevelConfig = config.Bind("Logging", "Log Level", ModdingUtils.LogLevel.Inherited, logLevelDescription);

            if (LogLevel > ModdingUtils.LogLevel.None)
                Logger.LogInfo("In Awake for MaxDrawDistance");

            MaxDraw = config.Bind("Draw Distance", "Render Distance", 3000f, maxDrawDescription);
            MaxShadowDistance = config.Bind("Draw Distance", "Shadow Distance", 3000f, maxShadowDescription);

            if (LogLevel >= ModdingUtils.LogLevel.High)
                Logger.LogInfo("Config Bound");
        }

        [UsedImplicitly]
        private void Awake()
        {
            DoConfig(Config);

            if (LogLevel > ModdingUtils.LogLevel.None)
                Logger.LogInfo("MaxDrawDistance Plug-in loaded");
            ModdingUtils.Initialize(this, Logger);
            
            SetShadowDistance();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void SetDrawDistance(object o = null)
        {
            if (LogLevel > ModdingUtils.LogLevel.None)
                Logger.LogInfo("Updating DrawDistance");
            var cameras = FindObjectsOfType<Camera>();
            foreach (var cam in cameras)
            {
                cam.farClipPlane = MaxDraw.Value;
            }
        }

        public void SetShadowDistance(object o = null)
        {
            if (LogLevel > ModdingUtils.LogLevel.None)
                Logger.LogInfo("Updating ShadowDistance");
            QualitySettings.shadowDistance = MaxShadowDistance.Value;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
            => SetDrawDistance();
        
    }
}
