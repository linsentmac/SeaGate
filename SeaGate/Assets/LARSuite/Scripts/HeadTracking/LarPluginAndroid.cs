using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using LARSuite;

namespace LARSuiteInternal
{
    public class LarPluginAndroid : LarPlugin
    {

        private enum RenderEvent
        {
            Initialize,
            BeginAR,
            EndAR,
            EndEye,
            SubmitFrame,
            Shutdown,
            RecenterTracking,
            SetTrackingMode,
            SetPerformanceLevels
        };

        public static LarPluginAndroid Create()
        {
            if (Application.isEditor)
            {
                Debug.LogError("LarPlugin not supported in unity editor!");
                throw new InvalidOperationException();
            }

            return new LarPluginAndroid();
        }


        private LarPluginAndroid() { }

        private void IssueEvent(RenderEvent e)
        {
            // Queue a specific callback to be called on the render thread
            GL.IssuePluginEvent(LarPluginSO.GetRenderEventFunc(), (int)e);
        }

        public override bool IsInitialized() { return LarPluginSO.LarIsInitialized(); }

        public override bool IsRunning() { return LarPluginSO.LarIsRunning(); }

        public override bool IsSlamReady() { return LarPluginSO.LarIsSlamReady(); }
        public override IEnumerator Initialize()
        {
            //yield return new WaitUntil(() => LarIsInitialized() == false);  // Wait for shutdown

            yield return base.Initialize();

#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            LarPluginSO.LarInitializeEventData(activity.GetRawObject());
#endif
            IssueEvent(RenderEvent.Initialize);
            yield return new WaitUntil(() => LarPluginSO.LarIsInitialized() == true);

            yield return null;  // delay one frame - fix for re-init w multi-threaded rendering

            deviceInfo = GetDeviceInfo();
        }

        public override IEnumerator BeginAR(int cpuPerfLevel, int gpuPerfLevel)
        {
            //yield return new WaitUntil(() => LarIsRunning() == false);  // Wait for EndAR

            yield return base.BeginAR(cpuPerfLevel, gpuPerfLevel);

            float[] lowerLeft = { -1f, -1f, 0f, 1f };
            float[] upperLeft = { -1f, 1f, 0f, 1f };
            float[] upperRight = { 1f, 1f, 0f, 1f };
            float[] lowerRight = { 1f, -1f, 0f, 1f };
            LarPluginSO.LarSetupLayerCoords(0, -1, lowerLeft, lowerRight, upperLeft, upperRight);    // Eye/All
            LarPluginSO.LarSetupLayerCoords(1, -1, lowerLeft, lowerRight, upperLeft, upperRight);    // Overlay/All

            LarPluginSO.LarSetPerformanceLevelsEventData(cpuPerfLevel, gpuPerfLevel);

            yield return new WaitUntil(() => LarPluginSO.LarCanBeginAR() == true);
            IssueEvent(RenderEvent.BeginAR);
        }

        public override void EndAR()
        {
            base.EndAR();

            IssueEvent(RenderEvent.EndAR);
        }

        public override void EndEye()
        {
            IssueEvent(RenderEvent.EndEye);
        }

        public override void SetTrackingMode(TrackingMode mode)
        {
            LarPluginSO.LarSetTrackingModeEventData((int)mode);
            IssueEvent(RenderEvent.SetTrackingMode);
        }

        public override void SetPerformanceLevels(int newCpuPerfLevel, int newGpuPerfLevel)
        {
            LarPluginSO.LarSetPerformanceLevelsEventData((int)newCpuPerfLevel, (int)newGpuPerfLevel);
            IssueEvent(RenderEvent.SetPerformanceLevels);
        }

        public override void SetVSyncCount(int vSyncCount)
        {
            LarPluginSO.LarSetVSyncCount(vSyncCount);
        }

        public override void SetFieldOfView(float fieldOfView)
        {

            LarPluginSO.LarSetFieldOfView(fieldOfView);
        }

        public override void RecenterTracking()
        {
            IssueEvent(RenderEvent.RecenterTracking);
        }

        public override void GetPredictedPose(ref Quaternion orientation, ref Vector3 position, int frameIndex)
        {
            orientation = Quaternion.identity;
            position = Vector3.zero;

            LarPluginSO.LarGetPredictedPose(ref orientation.x, ref orientation.y, ref orientation.z, ref orientation.w,
                                ref position.x, ref position.y, ref position.z, frameIndex);

            orientation.z = -orientation.z;
            position.x = -position.x;
            position.y = -position.y;
        }

        public override DeviceInfo GetDeviceInfo()
        {
            DeviceInfo info = new DeviceInfo();

            LarPluginSO.LarGetDeviceInfo(ref info.displayWidthPixels,
                              ref info.displayHeightPixels,
                              ref info.displayRefreshRateHz,
                              ref info.targetFovXRad,
                              ref info.targetFovYRad);

            return info;
        }

        public override void SubmitFrame(int frameIndex)
        {
            int i;
            int eyeCount = 0;
            if (eyes != null) for (i = 0; i < eyes.Length; i++)
            {
                if (eyes[i].isActiveAndEnabled == false || eyes[i].TextureId == 0 || eyes[i].Side == 0) continue;
                if (eyes[i].imageTransform != null && eyes[i].imageTransform.gameObject.activeSelf == false) continue;
                LarPluginSO.LarSetupLayerData(0, eyeCount, (int)eyes[i].Side, eyes[i].TextureId, eyes[i].ImageType == LarEye.eType.EGL_TEXTURE ? 1 : 0);
                float[] lowerLeft = { eyes[i].clipLowerLeft.x, eyes[i].clipLowerLeft.y, eyes[i].clipLowerLeft.z, eyes[i].clipLowerLeft.w };
                //Debug.Log("lowerleft="+lowerLeft[0]+","+lowerLeft[1]+","+lowerLeft[2]+","+lowerLeft[3]);
                float[] upperLeft = { eyes[i].clipUpperLeft.x, eyes[i].clipUpperLeft.y, eyes[i].clipUpperLeft.z, eyes[i].clipUpperLeft.w };
                //Debug.Log("upperLeft=" + upperLeft[0]+","+ upperLeft[1]+","+ upperLeft[2]+","+ upperLeft[3]);
                float[] upperRight = { eyes[i].clipUpperRight.x, eyes[i].clipUpperRight.y, eyes[i].clipUpperRight.z, eyes[i].clipUpperRight.w };
                //Debug.Log("upperRight=" + upperRight[0]+","+ upperRight[1]+","+ upperRight[2]+","+ upperRight[3]);
                float[] lowerRight = { eyes[i].clipLowerRight.x, eyes[i].clipLowerRight.y, eyes[i].clipLowerRight.z, eyes[i].clipLowerRight.w };
                //Debug.Log("lowerRight=" + lowerRight[0]+","+ lowerRight[1]+","+ lowerRight[2]+","+ lowerRight[3]);
                LarPluginSO.LarSetupLayerCoords(0, eyeCount, lowerLeft, lowerRight, upperLeft, upperRight);
                eyeCount++;
            }
            for (i = eyeCount; i < LarManager.s_eyeLayerMax; i++)
            {
                LarPluginSO.LarSetupLayerData(0, i, 0, 0, 0);
            }

            int overlayCount = 0;
            if (overlays != null) for (i = 0; i < overlays.Length; i++)
            {
                if (overlays[i].isActiveAndEnabled == false || overlays[i].TextureId == 0 || overlays[i].Side == 0) continue;
                if (overlays[i].imageTransform != null && overlays[i].imageTransform.gameObject.activeSelf == false) continue;
                LarPluginSO.LarSetupLayerData(1, overlayCount, (int)overlays[i].Side, overlays[i].TextureId, overlays[i].ImageType == LarOverlay.eType.EGL_TEXTURE ? 1 : 0);
                float[] lowerLeft = { overlays[i].clipLowerLeft.x, overlays[i].clipLowerLeft.y, overlays[i].clipLowerLeft.z, overlays[i].clipLowerLeft.w };
                float[] upperLeft = { overlays[i].clipUpperLeft.x, overlays[i].clipUpperLeft.y, overlays[i].clipUpperLeft.z, overlays[i].clipUpperLeft.w };
                float[] upperRight = { overlays[i].clipUpperRight.x, overlays[i].clipUpperRight.y, overlays[i].clipUpperRight.z, overlays[i].clipUpperRight.w };
                float[] lowerRight = { overlays[i].clipLowerRight.x, overlays[i].clipLowerRight.y, overlays[i].clipLowerRight.z, overlays[i].clipLowerRight.w };
                LarPluginSO.LarSetupLayerCoords(1, overlayCount, lowerLeft, lowerRight, upperLeft, upperRight); overlayCount++;
            }
            for (i = overlayCount; i < LarManager.s_overlayLayerMax; i++)
            {
                LarPluginSO.LarSetupLayerData(1, i, 0, 0, 0);
            }

            LarPluginSO.LarSubmitFrameEventData(frameIndex);
            IssueEvent(RenderEvent.SubmitFrame);
        }

        public override void Shutdown()
        {
            if (eyes != null) for (int i = 0; i < eyes.Length; i++)
            {
	        eyes[i].ReleaseBuffers();
	    }
            IssueEvent(RenderEvent.Shutdown);

            base.Shutdown();
        }

    }
}
