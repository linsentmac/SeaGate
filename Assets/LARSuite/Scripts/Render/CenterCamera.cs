//-----------------------------------------------------------------------
// <copyright>
//
// Copyright 2016 Lenovo Inc. All Rights Reserved.
//
// </copyright>
//-----------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System;

namespace LARSuite
{

    /// <summary>
    /// Center camera works as the Main camera in the Mono <see cref="RenderMode"/>; in the 
    /// Stereo mode, it work as fake camera, it does do nothing rendering, it create two 
    /// <see cref="StereoCamera"/> to do actual rendering. it also listen to <see cref="IRenderListener"/>
    /// for render mode change and <see cref="IGlassProfileListener"/> for <see cref="GlassProfile"/> change.
    /// 
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CenterCamera : MonoBehaviour, IRenderListener, IGlassProfileListener {

        private Camera _monoCamera;

        void Awake() {
            RenderController.Instance.Register(this);
            GlassProfileManager.Instance.Register(this);
            _monoCamera = GetComponent<Camera>();
            _monoCamera.fieldOfView = GlassProfileManager.Instance.LenseFov;
            CreateStereCamera(StereoCamera.Eye.Left, _monoCamera);
            CreateStereCamera(StereoCamera.Eye.Right, _monoCamera);
        }

        void Start() {
            UpdateRenderMode(RenderController.Instance.Mode);
        }

        void OnDestroy() {
            RenderController.Instance.UnRegister(this);
            GlassProfileManager.Instance.UnRegister(this);
        }

        private StereoCamera CreateStereCamera(StereoCamera.Eye eye, Camera master) {
            string cameraName = "StereoCamera" + ( eye == StereoCamera.Eye.Left ? "Left" : "Right");
            GameObject cameraObject = new GameObject(cameraName);
            cameraObject.transform.SetParent(transform);
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.CopyFrom(master);
            camera.depth = master.depth - 1;

            Rect viewPort = eye == StereoCamera.Eye.Left ? new Rect(0.0f, 0.0f, 0.5f, 1.0f) : new Rect(0.5f, 0.0f, 0.5f, 1.0f);
            camera.transform.localRotation = Quaternion.identity;
            camera.transform.localScale = Vector3.one;
            camera.rect = viewPort;

            StereoCamera stereoCamera = cameraObject.AddComponent<StereoCamera>();
            stereoCamera.Master = master;
            stereoCamera.EyeID = eye;

            return stereoCamera;
        }

        public void OnRenderModeChange(RenderController.RenderMode mode) {
            UpdateRenderMode(mode);
        }

        public void OnLenseSeperationChange(float seperation) {
        }

        public void OnLenseFrustumChange(GlassProfile.LenseFrustum frustum) {
            _monoCamera.fieldOfView = frustum.Fov;
        }

        private void UpdateRenderMode(RenderController.RenderMode mode) {
            if (mode == RenderController.RenderMode.Mono) {
                _monoCamera.enabled = true;
            } else {
                _monoCamera.enabled = false;
            }
        }
    }
}


