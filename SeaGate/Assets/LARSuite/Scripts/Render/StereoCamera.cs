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
    ///  There are two stereo cameras enabled for rendering in Stereo <see cref="RenderMode"/>, 
    ///  one for Left <see cref="Eye"/> and one for Right. Stereo camera also listen <see cref="IRenderListener"/>
    ///  and <see cref="IGlassProfileListener"/> for RenderMode change or <see cref="GlassProfile"/> change.
    ///  
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class StereoCamera : MonoBehaviour, IRenderListener, IGlassProfileListener {

        public enum Eye {
            Left = 0,
            Right = 1
        }

        private Camera _master;
        private Camera _camera;
        private Eye _eyeID;

        public Camera Master {
            get {
                return _master;
            }

            set {
                _master = value;
            }
        }

        public Eye EyeID {
            get {
                return _eyeID;
            }
            set {
                _eyeID = value;
                UpdateLenseSeperation(GlassProfileManager.Instance.CustomProfile().LenseSeperationLeft);
                UpdateLenseSeperation(GlassProfileManager.Instance.CustomProfile().LenseSeperationRight);

                GlassProfile.LenseProperty property = _eyeID == Eye.Left ? GlassProfileManager.Instance.CustomProfile().LeftProperty :
                GlassProfileManager.Instance.CustomProfile().RightProperty;

                UpdateLenseProperty(property);

            }
        }

        void Awake() {
            RenderController.Instance.Register(this);
            GlassProfileManager.Instance.Register(this);
            _camera = GetComponent<Camera>();

            UpdateLenseSeperation(GlassProfileManager.Instance.CustomProfile().LenseSeperationLeft);
            UpdateLenseSeperation(GlassProfileManager.Instance.CustomProfile().LenseSeperationRight);

            GlassProfile.LenseProperty property = _eyeID == Eye.Left ? GlassProfileManager.Instance.CustomProfile().LeftProperty :
                GlassProfileManager.Instance.CustomProfile().RightProperty;

            UpdateLenseProperty(property);

            UpdateRenderMode(RenderController.Instance.Mode);
        }

        void Update() {
        }

        void OnDestroy() {
            RenderController.Instance.UnRegister(this);
            GlassProfileManager.Instance.UnRegister(this);
        }

        public void OnRenderModeChange(RenderController.RenderMode mode) {
            UpdateRenderMode(mode);
        }

        public void OnLenseSeperationChange(float seperation) {
            //UpdateLenseSeperation(seperation);
        }

        public void OnLenseFrustumChange(GlassProfile.LenseFrustum frustum) {
        }

        private void UpdateRenderMode(RenderController.RenderMode mode) {
            if (RenderController.RenderMode.Mono == mode) {
                _camera.enabled = false;
            } else {
                _camera.enabled = true;
            }
        }

        private void UpdateLenseSeperation(float lenseSeperation) {
            float cameraOffsetMultipler = EyeID == StereoCamera.Eye.Left ? -1 : 1;
            _camera.transform.localPosition = cameraOffsetMultipler * lenseSeperation * 0.5f * Vector3.right;
        }

        public void UpdateLenseProperty(GlassProfile.LenseProperty property) {
            Vector3 localPos = new Vector3(property.PosX, property.PosY, property.PosZ);
            _camera.transform.localPosition = localPos;

            Vector3 localRot = new Vector3(property.RotX, property.RotY, property.RotZ);
            _camera.transform.localEulerAngles = localRot;
        }
    }
}

