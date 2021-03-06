﻿using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
namespace LARSuite
{
    public class LarManager : MonoBehaviour, IGlassProfileListener
    {
        static public int starkitVersion_major = 4;
        static public int starkitVersion_minor = 3;
        static public int s_eyeLayerMax = 8;
        static public int s_overlayLayerMax = 8;

        [Serializable]
        public class LarSettings
        {
            public enum eAntiAliasing
            {
                K1 = 1,
                K2 = 2,
                K4 = 4,
            };

            public enum eDepth
            {
                K16 = 16,
                K24 = 24
            };

            public enum eVSyncCount
            {
                K1 = 1,
                K2 = 2,
            };

            public enum eMasterTextureLimit
            {
                K0 = 0, // full size
                K1 = 1, // half size
                K2 = 2, // quarter size
                K3 = 3, // ...
                K4 = 4  // ...
            };

            public enum ePerfLevel
            {
                SYSTEM = 0,
                MINIMUM = 1,
                MEDIUM = 2,
                MAXIMUM = 3
            };

            public bool trackPosition = false;
            //public float headHeight = 0.0750f;
            //public float headDepth = 0.0805f;
            //public float interPupilDistance = 0.064f;
            public eDepth eyeDepth = eDepth.K24;
            public eAntiAliasing eyeAntiAliasing = eAntiAliasing.K2;
            public bool eyeHdr = false;
            public eDepth overlayDepth = eDepth.K16;
            public eAntiAliasing overlayAntiAliasing = eAntiAliasing.K1;
            public bool overlayHdr = false;
            public eVSyncCount vSyncCount = eVSyncCount.K1;
            public eMasterTextureLimit masterTextureLimit = eMasterTextureLimit.K0;
            public ePerfLevel cpuPerfLevel = ePerfLevel.MEDIUM;
            public ePerfLevel gpuPerfLevel = ePerfLevel.MEDIUM;
        }

        [SerializeField]
        private LarSettings settings;
        private GameObject headCamera;
        public Transform head;
        public Camera monoCamera;
        public Camera leftCamera;
        public Camera rightCamera;
        public Camera initleftCamera;
        public Camera initrightCamera;
        public Camera leftOverlay;
        public Camera rightOverlay;
        public Camera monoOverlay;
        public bool enableGazeInput;
        public bool HideEnableGazeInput;
        public bool Block;
        public float cursorHideTime;
        public float protectScreenTime;
        public bool protectScreeen;
        public float powerOffTime = 60;
        private bool powerOff;
        private int frameCount = 0;
        private static WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        private LarPlugin plugin = null;
        private float sensorWarmupDuration = 1.0f;
        private bool initialized = false;
        private bool running = false;
	    private bool initializedinstart = false;
        private List<LarEye> eyes = new List<LarEye>(s_eyeLayerMax);
        private List<LarOverlay> overlays = new List<LarOverlay>(s_overlayLayerMax);
        private bool disableInput = false;
        private Coroutine onResume = null;

        private float cursorTimestamps = 1f;
        private float screenTimestamps = 1f;
        private float timer = 0;
        private float screenTimer = 0;
        private Vector3 headPositon;
        private Quaternion headRotation;


        private AndroidJavaClass jc;
        private AndroidJavaObject jo;

        private double preScreenrotX=0;
        private double preScreenrotY=0;
        private double preScreenrotZ=0;
        private double preScreenrotW=0;

        private double screenrotX=0;
        private double screenrotY=0;
        private double screenrotZ=0;
        private double screenrotW=0;
        private double threshold = 0.6; //add threshold to avoid too sensitive

        public bool Initialized
        {
            get { return initialized; }
        }

        public bool IsRunning
        {
            get { return running; }
        }

        public bool DisableInput
        {
            get { return disableInput; }
            set { disableInput = value; }
        }

        void Awake()
        {

            if (!ValidateReferencedComponents())
            {
                enabled = false;
                return;
            }
            Input.backButtonLeavesApp = true;
            Application.targetFrameRate = -1;
            if (Application.platform == RuntimePlatform.Android)
            {
                jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            }
        }

        bool ValidateReferencedComponents()
        {
            plugin = LarPlugin.Instance;
            if (plugin == null)
            {
                Debug.LogError("Lar Plugin failed to load. Disabling...");
                return false;
            }

            if (head == null)
            {
                Debug.LogError("Required head gameobject not found! Disabling...");
                return false;
            }

            if (monoCamera == null && (leftCamera == null || rightCamera == null))
            {
                Debug.LogError("Required eye components are missing! Disabling...");
                return false;
            }

            return true;
        }

        // Use this for initialization
        IEnumerator Start()
        {
            Debug.Log("Starting starkit! version : " + starkitVersion_major + "." + starkitVersion_minor);
            initleftCamera.gameObject.transform.Find("LCanvas").gameObject.SetActive(false);
            initrightCamera.gameObject.transform.Find("RCanvas").gameObject.SetActive(false);
            initleftCamera.gameObject.SetActive(true);
            initrightCamera.gameObject.SetActive(true);
            yield return StartCoroutine(Initialize());
            initialized = plugin.IsInitialized();

            initleftCamera.gameObject.transform.Find("LCanvas").gameObject.SetActive(false);
            initrightCamera.gameObject.transform.Find("RCanvas").gameObject.SetActive(false);
            initleftCamera.gameObject.SetActive(false);
            initrightCamera.gameObject.SetActive(false);
            leftCamera.nearClipPlane = 0.05f;
            rightCamera.nearClipPlane = 0.05f;
            leftCamera.gameObject.transform.Find("Canvas").gameObject.SetActive(false);
            rightCamera.gameObject.transform.Find("Canvas").gameObject.SetActive(false);
            if (settings.trackPosition)
            {
                leftCamera.gameObject.transform.Find("Canvas").gameObject.SetActive(true);
                rightCamera.gameObject.transform.Find("Canvas").gameObject.SetActive(true);
            }
            yield return StartCoroutine(plugin.BeginAR((int)settings.cpuPerfLevel, (int)settings.gpuPerfLevel));
            StartCoroutine(SubmitFrame());
            yield return new WaitUntil(() => plugin.IsRunning() == true);
            running = true;
            initializedinstart = true;
            yield return new WaitUntil(() => plugin.IsSlamReady() == true);

            if (settings.trackPosition)
            {
                leftCamera.gameObject.transform.Find("Canvas").gameObject.SetActive(false);
                rightCamera.gameObject.transform.Find("Canvas").gameObject.SetActive(false);
            }

            plugin.RecenterTracking();
            blockFun();
            Debug.Log("Lar initialized in start!");
        }

        private void blockFun()
        {
          
            if (Block == true)
            {
                initleftCamera.gameObject.SetActive(true);
                initrightCamera.gameObject.SetActive(true);

                initleftCamera.gameObject.transform.Find("LCanvas").gameObject.SetActive(true);
                initrightCamera.gameObject.transform.Find("RCanvas").gameObject.SetActive(true);
            }
            else
            {
                initleftCamera.gameObject.SetActive(false);
                initrightCamera.gameObject.SetActive(false);
                initleftCamera.gameObject.transform.Find("LCanvas").gameObject.SetActive(false);
                initrightCamera.gameObject.transform.Find("RCanvas").gameObject.SetActive(false);
            }
        }

        private IEnumerator Initialize()
        {
            // Plugin must be initialized OnStart in order to properly
            // get a valid surface
            GameObject mainCameraGo = GameObject.FindWithTag("MainCamera");
            if (mainCameraGo)
            {
                Debug.Log("Camera with MainCamera tag found.");
                if (!disableInput)
                {
                    Debug.Log("Will use translation and orientation from the MainCamera.");
                    transform.position = mainCameraGo.transform.position;
                    transform.rotation = mainCameraGo.transform.rotation;
                }
            }

            // Attach gaze to left eye
            headCamera = GameObject.Find("Main Camera");
            AttachGazeInputToEventSystem();
            AttachGazeCursorToHead();
            AttachPhysicsRayCasterToHead();
            InitializeGazeInput();

            headPositon = head.transform.position;
            headRotation = head.transform.rotation;


            // adjust the left/right camera position rotation
            GlassProfileManager.Instance.Register(this);

            GlassProfile.LenseProperty property = GlassProfileManager.Instance.CustomProfile().LeftProperty;
            leftCamera.transform.localPosition = new Vector3(property.PosX, property.PosY, property.PosZ);
            leftCamera.transform.localEulerAngles = new Vector3(property.RotX, property.RotY, property.RotZ);
            UpdateCameraFov(leftCamera, property.FovUp, property.FovDown);

            property = GlassProfileManager.Instance.CustomProfile().RightProperty;
            rightCamera.transform.localPosition = new Vector3(property.PosX, property.PosY, property.PosZ);
            rightCamera.transform.localEulerAngles = new Vector3(property.RotX, property.RotY, property.RotZ);
            UpdateCameraFov(rightCamera, property.FovUp, property.FovDown);

            // start ar/gl
            GL.Clear(false, true, Color.black);

            yield return StartCoroutine(plugin.Initialize());
            InitializeEyes();
            InitializeOverlays();

            if (settings.trackPosition)
            {
                plugin.SetTrackingMode(LarPlugin.TrackingMode.TRACKING_POSITION);
            }
            else
            {
                plugin.SetTrackingMode(LarPlugin.TrackingMode.TRACKING_ORIENTATION);
            }

            plugin.SetVSyncCount((int)settings.vSyncCount);
            QualitySettings.vSyncCount = (int)settings.vSyncCount;

            plugin.SetFieldOfView((float)leftCamera.fieldOfView * Mathf.Deg2Rad);

        }

        private void AttachGazeInputToEventSystem()
        {
            GameObject eventSystem = GameObject.Find("EventSystem");
            if (eventSystem == null)
            {
                eventSystem = CreateEventSystem();
                eventSystem.transform.parent = transform.parent;
            }

            if (eventSystem.GetComponent<GazeInputModule>() == null)
            {
                var gazeInput = eventSystem.AddComponent<GazeInputModule>();
            }
        }

        private void AttachGazeCursorToHead()
        {
            if (GameObject.Find("GazeCursor")) return;

            if (headCamera != null)
            {
                GameObject gazeCursor = CreateGazeCursor();
                gazeCursor.transform.SetParent(headCamera.transform);
                gazeCursor.transform.localPosition = Vector3.zero;
                gazeCursor.transform.localRotation = Quaternion.identity;
                gazeCursor.transform.localScale = Vector3.one;

            }
        }

        private void AttachPhysicsRayCasterToHead()
        {
            if (headCamera != null)
            {
                if (headCamera.GetComponent<PhysicsRaycaster>() == null)
                {
                    headCamera.AddComponent<PhysicsRaycaster>();
                    headCamera.GetComponent<Camera>().enabled = false;
                }
            }
        }

        private GameObject CreateEventSystem()
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
            return eventSystem;
        }

        private GameObject CreateGazeCursor()
        {
            GameObject cursorObject = new GameObject("GazeCursor");
            cursorObject.AddComponent<MeshRenderer>();
            cursorObject.AddComponent<GazeCursor>();
            return cursorObject;
        }

        private void InitializeGazeInput()
        {
            if (enableGazeInput)
            {
                EnableGazeInput();
            }
            else
            {
                DisableGazeInput();
            }
        }

        private void EnableGazeInput()
        {
            GameObject eventSystem = GameObject.Find("EventSystem");
            if (eventSystem != null)
            {

                BaseInputModule[] inputs = eventSystem.GetComponents<BaseInputModule>();
                foreach (var input in inputs)
                {
                    input.enabled = false;
                }

                var gazeInput = eventSystem.GetComponent<GazeInputModule>();
                if (gazeInput)

                    gazeInput.enabled = true;
            }
        }

        private void DisableGazeInput()
        {
            GameObject eventSystem = GameObject.Find("EventSystem");
            if (eventSystem != null)
            {
                BaseInputModule[] inputs = eventSystem.GetComponents<BaseInputModule>();
                foreach (var input in inputs)
                {
                    input.enabled = true;
                }

                var gazeInput = eventSystem.GetComponent<GazeInputModule>();
                if (gazeInput) gazeInput.enabled = false;
            }
        }

        private void UpdateCameraFov(Camera _camera, float up, float down)
        {

            float upRadi = up * Mathf.PI / 180;
            float downRadi = down * Mathf.PI / 180;

            float near = _camera.nearClipPlane;
            float far = _camera.farClipPlane;
            float top = (float)(near * Math.Tan(upRadi));
            float bottom = (float)(-1.0f * near * Math.Tan(downRadi));
            float h = top - bottom;
            float w = _camera.aspect * h / 2; //the L/R camera not devided into half yet.

            float left = -1.0f * w * 0.5f;
            float right = w * 0.5f;

            Matrix4x4 perspectiveMatrix = MatrixUtility.Perspective(left, right, bottom, top, near, far);

            _camera.projectionMatrix = perspectiveMatrix;
        }

        private void InitializeEyes()
        {
            eyes.Clear();
            if (monoCamera != null && monoCamera.gameObject.activeSelf)
            {
                Debug.Log("add eyes mono");
                AddEyes(monoCamera, LarEye.eSide.BOTH);
            }
            if (leftCamera != null && leftCamera.gameObject.activeSelf)
            {
                Debug.Log("add eyes left");
                AddEyes(leftCamera, LarEye.eSide.LEFT);
            }
            if (rightCamera != null && rightCamera.gameObject.activeSelf)
            {
                Debug.Log("add eyes right");
                AddEyes(rightCamera, LarEye.eSide.RIGHT);
            }

            if (settings.eyeHdr && settings.eyeAntiAliasing != LarSettings.eAntiAliasing.K1)
            {
                Debug.LogWarning("Antialiasing not supported when HDR is enabled. Disabling antiAliasing...");
                settings.eyeAntiAliasing = LarSettings.eAntiAliasing.K1;
            }

            LarPlugin.DeviceInfo info = plugin.deviceInfo;

            foreach (LarEye eye in eyes)
            {
                if (eye == null) continue;

                Vector3 eyePos;
               

                eye.Format = settings.eyeHdr ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
                eye.Resolution = new Vector2(info.displayWidthPixels / 2, info.displayHeightPixels);
                eye.Depth = (int)settings.eyeDepth;
                eye.AntiAliasing = (int)settings.eyeAntiAliasing;    // hdr not supported with antialiasing
                eye.OnPostRenderListener = OnPostRenderListener;

                eye.Initialize();
            }
        }

        private void AddEyes(Camera cam, LarEye.eSide side)
        {
            bool enableCamera = false;
            var eyesFound = cam.gameObject.GetComponents<LarEye>();
            for (int i = 0; i < eyesFound.Length; i++)
            {
                eyesFound[i].Side = side;
                if (eyesFound[i].imageType == LarEye.eType.RENDER_TEXTURE) enableCamera = true;
            }
            eyes.AddRange(eyesFound);
            if (eyesFound.Length == 0)
            {
                var eye = cam.gameObject.AddComponent<LarEye>();
                eye.Side = side;
                eyes.Add(eye);
                enableCamera = true;
            }
            cam.allowHDR = settings.eyeHdr;
            cam.enabled = enableCamera;
        }

        private void AddOverlays(Camera cam, LarOverlay.eSide side)
        {
            bool enableCamera = false;
            var overlaysFound = cam.gameObject.GetComponents<LarOverlay>();
            for (int i = 0; i < overlaysFound.Length; i++)
            {
                overlaysFound[i].Side = side;
                if (overlaysFound[i].imageType == LarOverlay.eType.RENDER_TEXTURE) enableCamera = true;
            }
            overlays.AddRange(overlaysFound);
            if (overlaysFound.Length == 0)
            {
                var overlay = cam.gameObject.AddComponent<LarOverlay>();
                overlay.Side = side;
                overlays.Add(overlay);
                enableCamera = true;
            }
            cam.allowHDR = settings.overlayHdr;
            cam.enabled = enableCamera;
        }

        void InitializeOverlays()
        {
            overlays.Clear();
            if (leftOverlay != null && leftOverlay.gameObject.activeSelf)
            {
                AddOverlays(leftOverlay, LarOverlay.eSide.LEFT);
            }
            if (rightOverlay != null && rightOverlay.gameObject.activeSelf)
            {
                AddOverlays(rightOverlay, LarOverlay.eSide.RIGHT);
            }
            if (monoOverlay != null && monoOverlay.gameObject.activeSelf)
            {
                AddOverlays(monoOverlay, LarOverlay.eSide.BOTH);
            }
            if (settings.overlayHdr && settings.overlayAntiAliasing != LarSettings.eAntiAliasing.K1)
            {
                Debug.LogWarning("Antialiasing not supported when HDR is enabled. Disabling antiAliasing...");
                settings.overlayAntiAliasing = LarSettings.eAntiAliasing.K1;
            }

            LarPlugin.DeviceInfo info = plugin.deviceInfo;

            foreach (LarOverlay overlay in overlays)
            {
                if (overlay == null) continue;

                Vector3 eyePos;
              
                overlay.Format = settings.overlayHdr ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
                overlay.Resolution = new Vector2(rightOverlay.pixelWidth / 2, rightOverlay.pixelHeight);
                overlay.Depth = (int)settings.overlayDepth;
                overlay.AntiAliasing = (int)settings.overlayAntiAliasing;  // hdr not supported with antialiasing
                overlay.OnPostRenderListener = OnPostRenderListener;

                overlay.Initialize();
            }
        }

        IEnumerator SubmitFrame()
        {
            while (true)
            {
                yield return waitForEndOfFrame;
                plugin.SubmitFrame(frameCount);

                frameCount++;
            }
        }

        public void RecenterTracking()
        {
            plugin.RecenterTracking();
        }

        void OnPostRenderListener()
        {
            plugin.EndEye();
        }

        public void SetPause(bool pause)
        {
            //Debug.Log("LLH SetPause : pause = " + pause + "running = " + running);
            if (!initialized || running != pause)
            {
                //Debug.Log("LLH SetPause return");
                return;
            }

            if (pause)
            {
                OnPause();
            }
            else if (onResume == null)
            {
                onResume = StartCoroutine(OnResume());
            }
        }

        void OnPause()
        {
            //Debug.Log("LLH  OnPause");
            StopAllCoroutines();
            plugin.EndAR();
            running = false;
            onResume = null;
            timer = 0;
            screenTimer = 0;
            powerOff = false;
        }

        IEnumerator OnResume()
        {
            if (!initializedinstart )
            {
                Debug.Log("LLH OnResume return, do nothing!");
                yield break;
            }
            Debug.Log("LLH OnResume going...");
            initleftCamera.gameObject.transform.Find("LCanvas").gameObject.SetActive(false);
            initrightCamera.gameObject.transform.Find("RCanvas").gameObject.SetActive(false);
            initleftCamera.gameObject.SetActive(false);
            initrightCamera.gameObject.SetActive(false);
            leftCamera.nearClipPlane = 0.05f;
            rightCamera.nearClipPlane = 0.05f;
            leftCamera.gameObject.transform.Find("Canvas").gameObject.SetActive(false);
            rightCamera.gameObject.transform.Find("Canvas").gameObject.SetActive(false);
            if (settings.trackPosition)
            {
                leftCamera.gameObject.transform.Find("Canvas").gameObject.SetActive(true);
                rightCamera.gameObject.transform.Find("Canvas").gameObject.SetActive(true);
            }
            yield return StartCoroutine(plugin.BeginAR((int)settings.cpuPerfLevel, (int)settings.gpuPerfLevel));
            StartCoroutine(SubmitFrame());
            yield return new WaitUntil(() => plugin.IsRunning() == true);
            running = true;
            yield return new WaitUntil(() => plugin.IsSlamReady() == true);

            if (settings.trackPosition)
            {
                leftCamera.gameObject.transform.Find("Canvas").gameObject.SetActive(false);
                rightCamera.gameObject.transform.Find("Canvas").gameObject.SetActive(false);
            }

            plugin.RecenterTracking();
            blockFun();
            yield break;

        }
        private void HideCursor()
        {


        }
        void LateUpdate()
        {
            //  protectOledScreen();
            DestoryCursor();
            if (protectScreeen == true)
            {
                protectScreen();
            }


           

            //if (!initialized || !running)
            //onResume : need show the screen even slam is not ready yet!
            if (!initialized)
            {
                return;
            }

            Vector3 position = new Vector3();
            Quaternion orientation = new Quaternion();
            plugin.GetPredictedPose(ref orientation, ref position, frameCount);

            if (!disableInput)
            {
                head.transform.localRotation = orientation;
                head.transform.localPosition = position;
            }
          
         
        }
        private void CursorShowOrHide(bool cursorBool)
        {
            GameObject cursor = head.transform.Find("GazeCursor").gameObject;
            if (cursorBool == true)
            {
                cursor.SetActive(true );
            }
            else
            {
                cursor.SetActive(false);
            }

        }

        private void protectScreen()
        {

            screenTimestamps -= Time.deltaTime;

            if (screenTimestamps <= 0)
            {
                screenTimestamps = 1f;

                headPositon = head.transform.position;

                headRotation = head.transform.rotation;

             

                preScreenrotX = headRotation.eulerAngles.x;
                preScreenrotY = headRotation.eulerAngles.y;
                preScreenrotZ = headRotation.eulerAngles.z;
            }

            bool posBO = false;
            bool rotBO = false;

            posBO = (headPositon == head.transform.position) ? true : false;
            rotBO = (headRotation == head.transform.rotation) ? true : false;
            //  Debug.Log("posBO = " + posBO + " / rotBO = " + rotBO);
            //if (!rotBO)
            //{
               

                screenrotX = head.transform.rotation.eulerAngles.x;
                screenrotY = head.transform.rotation.eulerAngles.y;
                screenrotZ = head.transform.rotation.eulerAngles.z;

            //}


            double rotXdiff = Math.Abs(screenrotX - preScreenrotX);
            double rotYdiff = Math.Abs(screenrotY - preScreenrotY);
            double rotZdiff = Math.Abs(screenrotZ - preScreenrotZ);
          


            if ((posBO == true && rotBO == true) || (rotXdiff < threshold
                       && rotYdiff < threshold
                       && rotZdiff < threshold))
            {

                screenTimer += Time.deltaTime;

                if (screenTimer >= protectScreenTime)
                {

                    leftCamera.nearClipPlane = 100;
                    rightCamera.nearClipPlane = 100;

                    if (screenTimer >= powerOffTime && !powerOff)
                    {
                        powerOff = true;
                        powerToScreenOff();
                    }


                   
                }


                else
                {
                    leftCamera.nearClipPlane = 0.05f;
                    rightCamera.nearClipPlane = 0.05f;
                    
                }

            }
            else
            {
                leftCamera.nearClipPlane = 0.05f;
                rightCamera.nearClipPlane = 0.05f;
               
                screenTimer = 0;//只要移动就立马计时从零开始

                powerOff = false;
            }

        }

        public void powerToScreenOff()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                Debug.Log("powerScreen off ...... ");
                jo.Call<int>("screenOff");
            }
        }
        private void DestoryCursor()
        {
            GameObject cursor = head.transform.Find("GazeCursor").gameObject;



            cursorTimestamps -= Time.deltaTime;


            if (cursorTimestamps <= 0)
            {
                cursorTimestamps = 1f;

                headPositon = head.transform.position;

                headRotation = head.transform.rotation;

             

                preScreenrotX = head.eulerAngles.x;
                preScreenrotY = head.eulerAngles.y;
                preScreenrotZ = head.eulerAngles.z;
                //Debug.Log("Tmac update preScreen preScreenrotY  ...... " + preScreenrotY);
            }

            bool posBO = false;
            bool rotBO = false;

            posBO = (headPositon == head.transform.position) ? true : false;
            rotBO = (headRotation == head.transform.rotation) ? true : false;
            //  Debug.Log("posBO = " + posBO + " / rotBO = " + rotBO);

            //if (!rotBO)
            //{
             

                screenrotX = head.eulerAngles.x;
                screenrotY = head.eulerAngles.y;
                screenrotZ = head.eulerAngles.z;
                //Debug.Log("Tmac screenrotY " + screenrotY);
            //}


            double rotXdiff = Math.Abs(screenrotX - preScreenrotX);
            double rotYdiff = Math.Abs(screenrotY - preScreenrotY);
            double rotZdiff = Math.Abs(screenrotZ - preScreenrotZ);
            //double rotWdiff = Math.Abs(screenrotW - preScreenrotW);
            //Debug.Log("posBO = " + posBO + "/ rotBO =" + rotBO + " /rotXdiff = " + rotXdiff + " /rotYdiff = " + rotYdiff + " /rotZdiff = " + rotZdiff);
            if (enableGazeInput == true)
            {
                if ((posBO == true && rotBO == true) || (rotXdiff < threshold
                       && rotYdiff < threshold
                       && rotZdiff < threshold))
                {
                    timer += Time.deltaTime;

                    if (timer >= cursorHideTime)
                    {
                        //  Debug.Log(" cursor.SetActive(false) ");
                        cursor.SetActive(false);


                    }


                    else
                    {
                        cursor.SetActive(true);

                    }

                }
                else
                {

                    timer = 0;//只要移动就立马计时从零开始
                    cursor.SetActive(true);


                }

            }
            else
            {
                cursor.SetActive(false);


            }





        }

        public void OnLenseSeperationChange(float seperation)
        {
        }

        public void OnLenseFrustumChange(GlassProfile.LenseFrustum frustum)
        {
        }
        private void OnDestroy()
        {
            Debug.Log("LLH LarManager.OnDestroy()");
            StopAllCoroutines();

            if (plugin.IsRunning()) plugin.EndAR();
            running = false;
            if (plugin.IsInitialized()) plugin.Shutdown();
        }

        private void OnApplicationPause(bool pause)
        {
            Debug.Log("LLH LarManager.OnApplicationPause() pause : " + pause);

            SetPause(pause);
        }

        void OnApplicationQuit()
        {
            Debug.Log("LLH LarManager.OnApplicationQuit()");

            SetPause(true);
        }
    }
}
