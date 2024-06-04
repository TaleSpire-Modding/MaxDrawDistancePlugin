using UnityEngine;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using JetBrains.Annotations;
using ModdingTales;
using PluginUtilities;
using UnityEngine.SceneManagement;
using Sentry;
using MaxDrawDistance.Logging;

namespace MaxDrawDistance
{
    [BepInPlugin(Guid, "MaxDrawDistance", Version)]
    [BepInDependency(SetInjectionFlag.Guid)]
    [BepInDependency(ConfigurationManager.ConfigurationManager.Guid)]
    public class MaxDrawDistance : BaseUnityPlugin
    {
        // constants
        private const string Guid = "org.hollofox.plugins.MaxDrawDistance";
        public const string Version = "0.0.0.0";

        internal static SentryOptions _sentryOptions = new SentryOptions
        {
            // Tells which project in Sentry to send events to:
            Dsn = "https://06dc4d9a9d3a466b9c31839439ce487e@o1208746.ingest.sentry.io/4503901801873408",
            Debug = true,
            TracesSampleRate = 0.2,
            IsGlobalModeEnabled = true,
            AttachStacktrace = true,
        };

        private static ConfigEntry<float> MaxDraw { get; set; }
        private static ConfigEntry<float> MaxShadowDistance { get; set; }
        
        public void DoConfig(ConfigFile config)
        {
            ConfigDescription maxDrawDescription = new ConfigDescription("", null,
                new ConfigurationManagerAttributes { CallbackAction = (o) =>
                    {
                        SetDrawDistance();
                    }
                });

            ConfigDescription maxShadowDescription = new ConfigDescription("", null,
                new ConfigurationManagerAttributes {
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

        public void DoPatching()
        {
            new Harmony(Guid).PatchAll();
            Logger.LogDebug($"Patched.");
        }

        [UsedImplicitly]
        private void Awake()
        {
            Setup();
        }

        private void Setup()
        {
            DoConfig(Config);
            DoPatching();

            Logger.LogEvent += BepinexLogging.GenerateSentryLogFowarding(_sentryOptions, Version);
            Logger.LogInfo("Plug-in loaded");

            ModdingUtils.Initialize(this, Logger, "HolloFoxes'");
            SetShadowDistance();
            SceneManager.sceneLoaded += OnSceneLoaded;
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
