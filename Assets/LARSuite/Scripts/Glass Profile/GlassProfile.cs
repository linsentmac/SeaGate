//-----------------------------------------------------------------------
// <copyright>
//
// Copyright 2016 Lenovo Inc. All Rights Reserved.
//
// </copyright>
//-----------------------------------------------------------------------

using UnityEngine;
using System;
using System.Collections;
using SimpleJson;
using System.Collections.Generic;
namespace LARSuite
{
    /// <summary>
    ///  Glass Profile could save the various glass properties, like <see cref="LenseFrustum"/>,
    ///  eye seperation and render canvas size. It will be loaded from glass profile
    ///  file on device start up in the future. 
    /// </summary>
    public class GlassProfile {

        public class LenseFrustum {
            public float Fov { get; set; }
            public float Left { get; set; }
            public float Right { get; set; }
        }


        private float _lenseSeperationLeft;
        private float _lenseSeperationRight;

        public class LenseProperty {
            public float PosX { get; set; }
            public float PosY { get; set; }
            public float PosZ { get; set; }

            public float RotX { get; set; }
            public float RotY { get; set; }
            public float RotZ { get; set; }
            public float FovUp { get; set; }
            public float FovDown { get; set; }

            public void Init(JsonObject jsonObject) {
                object value;
                jsonObject.TryGetValue("position.x", out value);
                PosX = (float)Convert.ToDouble(value);

                jsonObject.TryGetValue("position.y", out value);
                PosY = (float)Convert.ToDouble(value);

                jsonObject.TryGetValue("position.z", out value);
                PosZ = (float)Convert.ToDouble(value);

                jsonObject.TryGetValue("rotation.x", out value);
                RotX = (float)Convert.ToDouble(value);

                jsonObject.TryGetValue("rotation.y", out value);
                RotY = (float)Convert.ToDouble(value);

                jsonObject.TryGetValue("rotation.z", out value);
                RotZ = (float)Convert.ToDouble(value);
                jsonObject.TryGetValue("fov.up", out value);
                FovUp = (float)Convert.ToDouble(value);

                jsonObject.TryGetValue("fov.down", out value);
                FovDown = (float)Convert.ToDouble(value);
            }
            public void InitConf(List<string> data) {
                PosX = float.Parse(data[0]);
                PosY = float.Parse(data[1]);
                PosZ = float.Parse(data[2]);
                RotX = float.Parse(data[3]);
                RotY = float.Parse(data[4]);
                RotZ = float.Parse(data[5]);
            }
        }

        private float _lenseSeperation;

        private LenseFrustum _lenseFrustum;
        public LenseProperty LeftProperty {get; set;}
        public LenseProperty RightProperty { get; set; }
        public float _rgbcamera2headOffsetX;
        public float _rgbcamera2headOffsetY;

        public GlassProfile() {
            _lenseSeperationLeft = 0.064f;
            _lenseSeperationRight = 0.064f;
            _lenseFrustum = new LenseFrustum();
            _lenseFrustum.Fov = 20.0f;
            _lenseFrustum.Left = 14.0f;
            _lenseFrustum.Right = 18.5f;
            _rgbcamera2headOffsetX = 0.0f;
            _rgbcamera2headOffsetY = 0.0f;
        }

        public float LenseSeperationLeft {
            get {
                return _lenseSeperationLeft;
            }
            
            set {
                _lenseSeperationLeft = value;
            }
        }
        public float LenseSeperationRight {
            get {
                return _lenseSeperationRight;
            }

            set {
                _lenseSeperationRight = value;
            }
        }
        public LenseFrustum LenseFrustumData {
            get {
                return _lenseFrustum;
            }
        }

        public Vector2 ScreenSize {
            get {
                return new Vector2(Screen.width, Screen.height);
            }
        }
    }

}

