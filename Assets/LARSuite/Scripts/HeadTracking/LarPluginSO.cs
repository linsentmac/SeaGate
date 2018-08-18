using LARSuite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace LARSuiteInternal
{
    public class LarPluginSO
    {
        [DllImport("larplugin")]
        public static extern IntPtr GetRenderEventFunc();

        [DllImport("larplugin")]
        public static extern bool LarIsInitialized();

        [DllImport("larplugin")]
        public static extern bool LarIsRunning();

        [DllImport("larplugin")]
        public static extern bool LarIsSlamReady();

        [DllImport("larplugin")]
        public static extern bool LarCanBeginAR();

        [DllImport("larplugin")]
        public static extern void LarInitializeEventData(IntPtr activity);

        [DllImport("larplugin")]
        public static extern void LarSubmitFrameEventData(int frameIndex);

        [DllImport("larplugin")]
        public static extern void LarSetupLayerCoords(int typeEyeOrOverlay, int layerIndex, float[] lowerLeft, float[] lowerRight, float[] upperLeft, float[] upperRight);
        [DllImport("larplugin")]
        public static extern void LarSetupLayerData(int typeEyeOrOverlay, int layerIndex, int sideMask, int textureId, int textureType);

        [DllImport("larplugin")]
        public static extern void LarSetTrackingModeEventData(int mode);

        [DllImport("larplugin")]
        public static extern void LarSetPerformanceLevelsEventData(int newCpuPerfLevel,
                                                                    int newGpuPerfLevel);

        [DllImport("larplugin")]
        public static extern void LarSetVSyncCount(int vSyncCount);

        [DllImport("larplugin")]
        public static extern void LarSetFieldOfView(float fieldOfView);

        [DllImport("larplugin")]
        public static extern void LarGetPredictedPose(ref float rx,
                                                       ref float ry,
                                                       ref float rz,
                                                       ref float rw,
                                                       ref float px,
                                                       ref float py,
                                                       ref float pz,
                                                       int frameIndex);

        [DllImport("larplugin")]
        public static extern void LarGetDeviceInfo(ref int displayWidthPixels,
                                                    ref int displayHeightPixels,
                                                    ref float displayRefreshRateHz,
                                                    ref float targetFovXRad,
                                                       ref float targetFovYRad);


    }
}
