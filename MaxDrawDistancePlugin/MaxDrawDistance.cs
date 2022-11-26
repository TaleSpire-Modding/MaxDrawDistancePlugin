using UnityEngine;
using System;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using JetBrains.Annotations;
using ModdingTales;
using PluginUtilities;
using UnityEngine.SceneManagement;
using Sentry;
using static ConfigurationManager.ConfigurationManager;
using BepInEx.Logging;

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

        internal static Harmony harmony;
        internal static SentryOptions _sentryOptions = new SentryOptions
        {
            // Tells which project in Sentry to send events to:
            Dsn = "https://06dc4d9a9d3a466b9c31839439ce487e@o1208746.ingest.sentry.io/4503901801873408",
            Debug = true,
            TracesSampleRate = 0.2,
            IsGlobalModeEnabled = true,
            AttachStacktrace = true
        };

        internal static Action<Scope> _scope = scope =>
        {
            scope.User = new User
            {
                Username = BackendManager.Username,
            };
            scope.Release = Version;
        };

        private static ConfigEntry<ModdingUtils.LogLevel> LogLevelConfig { get; set; }
        private static ConfigEntry<float> MaxDraw { get; set; }
        private static ConfigEntry<float> MaxShadowDistance { get; set; }
        internal static ConfigEntry<float> MaxLosDistance { get; set; }
        internal static logToSentry useSentry => _useSentry.Value;
        
        internal static ManualLogSource _logger;

        private static ModdingUtils.LogLevel LogLevel => LogLevelConfig.Value == ModdingUtils.LogLevel.Inherited
            ? ModdingUtils.LogLevelConfig.Value
            : LogLevelConfig.Value;

        public void DoConfig(ConfigFile config)
        {
            var logLevelDescription =
                new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdvanced = true });
            var maxDrawDescription = new ConfigDescription("", null,
                new ConfigurationManagerAttributes { CallbackAction = (o) =>
                    {
                        SentryInvoke(SetDrawDistance);
                    }
                });
            var maxShadowDescription = new ConfigDescription("", null,
                new ConfigurationManagerAttributes {
                    CallbackAction = (o) =>
                    {
                        SentryInvoke(SetShadowDistance);
                    }
                });
            // var maxLosDescription = new ConfigDescription("", null, new ConfigurationManagerAttributes { /*CallbackAction = LosPatch.UpdateFarClip*/});

            LogLevelConfig = config.Bind("Logging", "Log Level", ModdingUtils.LogLevel.Inherited, logLevelDescription);

            if (LogLevel > ModdingUtils.LogLevel.None)
                Logger.LogInfo("In Awake for MaxDrawDistance");

            MaxDraw = config.Bind("Draw Distance", "Render Distance", 3000f, maxDrawDescription);
            MaxShadowDistance = config.Bind("Draw Distance", "Shadow Distance", 500f, maxShadowDescription);
            // MaxLosDistance = config.Bind("Draw Distance", "Line of Sight Distance", 3000f, maxLosDescription);

            if (LogLevel >= ModdingUtils.LogLevel.High)
                Logger.LogInfo("Config Bound");
        }

        public void DoPatching()
        {
            harmony = new Harmony(Guid);
            harmony.PatchAll();
            if (LogLevel > ModdingUtils.LogLevel.None)
                Logger.LogInfo($"Patched.");
        }

        [UsedImplicitly]
        private void Awake()
        {
            _logger = Logger;
            SentryInvoke(Setup);
        }

        private void Setup()
        {
            DoConfig(Config);
            DoPatching();

            _logger.LogEvent += logFowarding;

            if (LogLevel > ModdingUtils.LogLevel.None)
                Logger.LogInfo("MaxDrawDistance Plug-in loaded");

            ModdingUtils.Initialize(this, Logger, "HolloFoxes'");
            SetShadowDistance();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void SetDrawDistance()
        {
            if (LogLevel > ModdingUtils.LogLevel.None)
                Logger.LogInfo("Updating DrawDistance");

            var cameras = FindObjectsOfType<Camera>();
            foreach (var cam in cameras)
            {
                cam.farClipPlane = MaxDraw.Value;
            }
        }

        public void SetShadowDistance()
        {
            if (LogLevel > ModdingUtils.LogLevel.None)
                Logger.LogInfo("Updating ShadowDistance");
            QualitySettings.shadowDistance = MaxShadowDistance.Value;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SetDrawDistance();
            SetShadowDistance();
        }

        static void SentryInvoke(Action a) =>
            ConfigurationManager.Utilities.Utils.SentryInvoke(a, _sentryOptions, _logger);

        private void logFowarding(object o, LogEventArgs e)
        {
            if (useSentry != logToSentry.Enabled) return;
            switch (e.Level)
            {
                case BepInEx.Logging.LogLevel.Fatal:
                    SentrySdk.CaptureMessage(e.Data.ToString(), _scope, SentryLevel.Fatal);
                    break;
                case BepInEx.Logging.LogLevel.Error:
                    SentrySdk.CaptureMessage(e.Data.ToString(), _scope, SentryLevel.Error);
                    break;
            }
        }
    }
}