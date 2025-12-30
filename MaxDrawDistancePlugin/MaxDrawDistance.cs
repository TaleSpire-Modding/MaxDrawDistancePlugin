using UnityEngine;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using JetBrains.Annotations;
using PluginUtilities;
using UnityEngine.SceneManagement;

namespace MaxDrawDistance
{
    [BepInPlugin(Guid, Name, Version)]
    [BepInDependency(SetInjectionFlag.Guid)]
    public class MaxDrawDistance : DependencyUnityPlugin
    {
        // constants
        private const string Guid = "org.hollofox.plugins.MaxDrawDistance";
        public const string Version = "0.0.0.0";
        public const string Name = "MaxDrawDistance";

        private static ConfigEntry<float> MaxDraw { get; set; }
        private static ConfigEntry<float> MaxShadowDistance { get; set; }

        private Harmony harmony;
        private float? defaultShadowDistance;

        public void DoConfig(ConfigFile config)
        {
            ConfigDescription maxDrawDescription = new ConfigDescription("", null,
                new ConfigurationAttributes { CallbackAction = (o) =>
                    {
                        SetDrawDistance();
                    }
                });

            ConfigDescription maxShadowDescription = new ConfigDescription("", null,
                new ConfigurationAttributes {
                    CallbackAction = (o) =>
                    {
                        SetShadowDistance();
                    }
                });

            Logger.LogDebug("Awake");

            MaxDraw = config.Bind("Draw Distance", "Render Distance", 3000f, maxDrawDescription);
            MaxShadowDistance = config.Bind("Draw Distance", "Shadow Distance", 500f, maxShadowDescription);
            
            Logger.LogDebug("Config Bound");
        }

        [UsedImplicitly]
        protected override void OnAwake()
        {
            DoConfig(Config);
            
            harmony = new Harmony(Guid);
            harmony.PatchAll();
            Logger.LogDebug($"Patched.");

            defaultShadowDistance = QualitySettings.shadowDistance;

            Logger.LogInfo("Plug-in loaded");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        [UsedImplicitly]
        protected override void OnDestroyed()
        {
            harmony.UnpatchSelf();
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (defaultShadowDistance != null)
                QualitySettings.shadowDistance = defaultShadowDistance.Value;
            Logger.LogInfo("Plug-in unloaded");

        }

        private void SetDrawDistance()
        {
            Logger.LogDebug("Updating Draw Distance");

            Camera[] cameras = FindObjectsOfType<Camera>();
            foreach (Camera cam in cameras)
            {
                cam.farClipPlane = MaxDraw.Value;
            }
        }

        public void SetShadowDistance()
        {
            Logger.LogDebug("Updating Shadow Distance");
            QualitySettings.shadowDistance = MaxShadowDistance.Value;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SetDrawDistance();
            SetShadowDistance();
        }
    }
}
