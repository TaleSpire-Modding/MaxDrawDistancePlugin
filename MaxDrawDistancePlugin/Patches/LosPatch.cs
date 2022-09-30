using DataModel;
using HarmonyLib;
using Unity.Mathematics;
using UnityEngine;

namespace MaxDrawDistance.Patches
{
    //[HarmonyPatch(typeof(LineOfSightManager), "OnSetupInternals")]
    /*[HarmonyPatch(typeof(LineOfSightManager), "StartProcess")]
    public class LosPatch
    {
        private static Camera _captureCamera;

        public static void Prefix(ref Camera ____captureCamera)
        {
            _captureCamera = ____captureCamera;
            UpdateFarClip();
        }

        internal static void UpdateFarClip(object o = null)
        {
            Debug.Log("captureCamera.farClipPlane updating");
            ViewPointManager.Instance.CaptureCamera.farClipPlane = MaxDrawDistance.MaxLosDistance.Value;
            _captureCamera.farClipPlane = MaxDrawDistance.MaxLosDistance.Value;
            Debug.Log("captureCamera.farClipPlane updated");
        }
    }

    [HarmonyPatch(typeof(ViewPointManager), "RenderToCubemap")]
    public class vpmPatch
    {
        public static bool Prefix(RenderTexture cubemap, float3 capturePos, ref Material ____tilePropOccluderMaterial, ref Mesh ____dummyFloorMesh, ref Material ____standardOccluderMaterial)
        {
            Debug.Log("RenderToCubemap Callback");


            double shadowDistance = (double)QualitySettings.shadowDistance;
            SharedBatchRenderer.SetBatchShadowRenderingState(false);
            QualitySettings.shadowDistance = 0.0f;
            ((BoardPresentation)BoardSessionManager.Board.BoardPresentation).CullAndRenderOccludersToCubemap(
                ViewPointManager.Instance.CaptureCamera, capturePos, MaxDrawDistance.MaxLosDistance.Value, cubemap, 
                ____tilePropOccluderMaterial, ____dummyFloorMesh, ____standardOccluderMaterial, 7);
            QualitySettings.shadowDistance = (float)shadowDistance;
            SharedBatchRenderer.SetBatchShadowRenderingState(true);

            Debug.Log("RenderToCubemap done");
            return false;
        }
    }*/
}
