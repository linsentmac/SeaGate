//-----------------------------------------------------------------------
// <copyright>
//
// Copyright 2016 Lenovo Inc. All Rights Reserved.
//
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace LARSuite
{
    /// <summary>
    ///  A central place to manage all <see cref="GlassProfile"/> . (WIP)
    /// </summary>
    public class GlassProfileManager :Singleton<GlassProfileManager> {

        private const string GLASS_PROFILE = "glass_profile.json";

        private GlassProfile _defaultProfile;
        private GlassProfile _customProfile;
        private List<IGlassProfileListener> _listeners = new List<IGlassProfileListener>();


        private GlassProfileManager() {

            GlassProfilePersistor persister = new GlassProfilePersistor();
            _defaultProfile = persister.LoadDefault();


#if UNITY_EDITOR
            string path = FileManager.Instance.PersistentDataPath + GLASS_PROFILE;
#else
            string path = "/data/misc/lar/" + GLASS_PROFILE;
#endif

            if (System.IO.File.Exists(path)) {
                _customProfile = persister.LoadProfile(path);
            } else {
                _customProfile = _defaultProfile;
            }
        }

        public GlassProfile DefaultProfile() {
            return _defaultProfile;
        }

        public GlassProfile CustomProfile() {
            return _customProfile;
        }

        public void Register(IGlassProfileListener listener) {
            _listeners.Add(listener);
        }

        public void UnRegister(IGlassProfileListener listener) {
            _listeners.Remove(listener);
        }

        public float LenseSeperationLeft {
            get {
                return _customProfile.LenseSeperationLeft;
            }

            set {
                if (_customProfile.LenseSeperationLeft != value) {
                    _customProfile.LenseSeperationLeft = value;
                    NotifyLenseSeperationChange(_customProfile.LenseSeperationLeft);
                }
            }
        }
        public float LenseSeperationRight {
            get {
                return _customProfile.LenseSeperationRight;
            }

            set {
                if (_customProfile.LenseSeperationRight != value) {
                    _customProfile.LenseSeperationRight = value;
                    NotifyLenseSeperationChange(_customProfile.LenseSeperationRight);
                }
            }
        }

        public float LenseFov {
            get {
                return _customProfile.LenseFrustumData.Fov;
            }

            set {
                if (_customProfile.LenseFrustumData.Fov != value) {
                    _customProfile.LenseFrustumData.Fov = value;
                    NotifyLenseFrustumChange(_customProfile.LenseFrustumData);
                }
            }
        }

        public float LenseFovLeft {
            get {
                return _customProfile.LenseFrustumData.Left;
            }

            set {
                if (_customProfile.LenseFrustumData.Left != value) {
                    _customProfile.LenseFrustumData.Left = value;
                    NotifyLenseFrustumChange(_customProfile.LenseFrustumData);
                }
            }
        }

        public float LenseFovRight {
            get {
                return _customProfile.LenseFrustumData.Right;
            }

            set {
                if (_customProfile.LenseFrustumData.Right != value) {
                    _customProfile.LenseFrustumData.Right = value;
                    NotifyLenseFrustumChange(_customProfile.LenseFrustumData);
                }
            }
        }

        public void NotifyLenseSeperationChange(float seperation) {
            foreach (IGlassProfileListener listener in _listeners) {
                listener.OnLenseSeperationChange(seperation);
            }
        }

        public void NotifyLenseFrustumChange(GlassProfile.LenseFrustum frustum) {
            foreach (IGlassProfileListener listener in _listeners) {
                listener.OnLenseFrustumChange(frustum);
            }
        }

    }

}
