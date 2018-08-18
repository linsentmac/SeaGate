using UnityEngine;
using System;
using System.Collections;
using LARSuiteInternal;

namespace LARSuite
{
    public abstract class LarPlugin
    {
        private static LarPlugin instance;

        public static LarPlugin Instance
        {
            get
            {
                if (instance == null)
                {
                    if(!Application.isEditor && Application.platform == RuntimePlatform.Android)
                    {
                        instance = LarPluginAndroid.Create();
                    }
                    else
                    {
                        instance = LarPluginWin.Create();
                    }
                }
                return instance;
            }
        }

        public LarManager larCamera = null;
        public LarEye[] eyes = null;
        public LarOverlay[] overlays = null;
        public DeviceInfo deviceInfo;

        public enum TrackingMode
        {
            TRACKING_ORIENTATION = 1,
            TRACKING_POSITION = 2
        }

        public enum FrameOption
        {
            DISABLE_DISTORTION_CORRECTION = (1 << 0),   //!< Disables the lens distortion correction (useful for debugging)
            DISABLE_REPROJECTION = (1 << 1),   //!< Disables re-projection
            ENABLE_MOTION_TO_PHOTON = (1 << 2),   //!< Enables motion to photon testing
            DISABLE_CHROMATIC_CORRECTION = (1 << 3)   //!< Disables the lens chromatic aberration correction (performance optimization)
        };

        public struct DeviceInfo
        {
            public int      displayWidthPixels;
            public int      displayHeightPixels;
            public float    displayRefreshRateHz;
            public float    targetFovXRad;
            public float    targetFovYRad;
        }

        public virtual bool IsInitialized() { return false; }
        public virtual bool IsRunning() { return false; }
        public virtual bool IsSlamReady() { return false; }
        public virtual IEnumerator Initialize ()
        {
            var go = GameObject.FindGameObjectWithTag("LarCamera");
            if (go == null)
            {
                Debug.Log("Gameobject with tag LarCamera not found!");
                yield break;
            }
            larCamera = go.GetComponent<LarManager>();
            if (larCamera == null)
            {
                Debug.Log("LarManager not found!");
                yield break;
            }

            yield break;
        }
        public virtual IEnumerator BeginAR(int cpuPerfLevel =0, int gpuPerfLevel =0)
        {
            if (eyes == null)
            {
                eyes = larCamera.gameObject.GetComponentsInChildren<LarEye>();
                if (eyes == null)
                {
                    Debug.Log("Components with LarEye not found!");
                }

                Array.Sort(eyes);
            }

            if (overlays == null)
            {
                overlays = larCamera.gameObject.GetComponentsInChildren<LarOverlay>();
                if (overlays == null)
                {
                    Debug.Log("Components with LarOverlay not found!");
                }

                Array.Sort(overlays);
            }

            yield break;
        }
        public virtual void EndAR() { }
        public virtual void EndEye() { }
        public virtual void SetTrackingMode(TrackingMode mode) { }
        public virtual void SetPerformanceLevels(int newCpuPerfLevel, int newGpuPerfLevel) { }
        public virtual void SetVSyncCount(int vSyncCount) { }
        public virtual void SetFieldOfView(float fieldOfView) { }
        public virtual void RecenterTracking() { }
        public virtual void SubmitFrame(int frameIndex) { }
        public virtual void GetPredictedPose (ref Quaternion orientation, ref Vector3 position, int frameIndex = -1)
        {
            orientation =  Quaternion.identity;
            position = Vector3.zero;
        }
        public abstract DeviceInfo GetDeviceInfo ();
        public virtual void Shutdown()
        {
            LarPlugin.instance = null;
        }
    }

}
