using System;
using UnityEngine;
using BepInEx;
using BepInEx.Configuration;
using PhotonUtil;

namespace MaxDrawDistance
{

    [BepInPlugin(Guid, "HolloFoxe's MaxDrawDistance", Version)]
    public class MaxDrawDistance : BaseUnityPlugin
    {
        // constants
        private const string Guid = "org.hollofox.plugins.MaxDrawDistance";
        private const string Version = "1.0.0.0";

        private ConfigEntry<float> MaxDraw { get; set; }

        /// <summary>
        /// Awake plugin
        /// </summary>
        void Awake()
        {
            Logger.LogInfo("In Awake for MaxDrawDistance");

            MaxDraw = Config.Bind("Hotkeys", "Load Image Shortcut", 300f);

            Debug.Log("MaxDrawDistance Plug-in loaded");
            ModdingTales.ModdingUtils.Initialize(this, Logger);
        }

        private bool first = true;

        private bool OnBoard()
        {
            return (CameraController.HasInstance &&
                    BoardSessionManager.HasInstance &&
                    BoardSessionManager.HasBoardAndIsInNominalState &&
                    !BoardSessionManager.IsLoading);
        }

        

        /// <summary>
        /// Looping method run by plugin
        /// </summary>
        void Update()
        {
            if (OnBoard() && first)
            {
                var cameras = FindObjectsOfType<Camera>();
                foreach (var cam in cameras)
                {
                    cam.farClipPlane = MaxDraw.Value;
                }

                first = false;
            }
            if (!OnBoard()) first = true;
        }
    }
}
