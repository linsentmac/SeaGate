//-----------------------------------------------------------------------
// <copyright>
//
// Copyright 2016 Lenovo Inc. All Rights Reserved.
//
// </copyright>
//-----------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;
using System.IO;
namespace LARSuite
{
    /// <summary>
    ///  A persistor to load/save <see cref="GlassProfile"/> to device local
    ///  file system.(WIP)
    /// </summary>
    public class GlassProfilePersistor {

        public GlassProfile LoadProfile(string path) {
            GlassProfile profile = new GlassProfile();

            string propertyString = System.IO.File.ReadAllText(path);

            List<JsonObject> jsons = JsonObjectUtility.BuildJsonList(propertyString);

            JsonObject first = jsons[0];
            object leftProperty;
            first.TryGetValue("LeftEye", out leftProperty);

            GlassProfile.LenseProperty left = new GlassProfile.LenseProperty();
            left.Init((JsonObject)leftProperty);

            profile.LeftProperty = left;

            object rightProperty;
            first.TryGetValue("RightEye", out rightProperty);

            GlassProfile.LenseProperty right = new GlassProfile.LenseProperty();
            right.Init((JsonObject)rightProperty);

            profile.RightProperty = right;

            return profile;
        }

        public GlassProfile LoadProfileConf(string pach) {
            GlassProfile profile = new GlassProfile();
            List<string> dataList = new List<string>();
            List<string> dataListLeft = new List<string>();
            List<string> dataListRight = new List<string>();
            string txt = File.ReadAllText(pach).ToString();
            StreamReader stringData = new StreamReader(pach);
            string line = stringData.ReadLine();
            for (int i = 0; i < txt.Length; i++) {
                if (line!= null) {
                    line = stringData.ReadLine();
                    if (line != null) {                      
                        string[] sArray = line.Split(new char[1] { '=' });
                        foreach (var item in sArray) {
                            if (item.Contains("0")) {
                                dataList.Add(item);
                            }

                        }
                    }
                }
            }
            for (int i = 0; i < dataList.Count; i++) {
                if (i<dataList.Count/2) {
                    dataListLeft.Add(dataList[i]);
                } else {
                    dataListRight.Add(dataListRight[i]);
                }
            }
            GlassProfile.LenseProperty LeftData = new GlassProfile.LenseProperty();
            LeftData.InitConf(dataListLeft);
            GlassProfile.LenseProperty RightData = new GlassProfile.LenseProperty();
            RightData.InitConf(dataListRight);
            return profile;
        }

        public GlassProfile LoadDefault() {

            GlassProfile profile = new GlassProfile();
            GlassProfile.LenseProperty leftDefault = new GlassProfile.LenseProperty();
            leftDefault.PosX = -0.0315f;
            leftDefault.PosY = 0.0f;
            leftDefault.PosZ = 0.0f;
            leftDefault.RotX = 0.0f;
            leftDefault.RotY = 0.45f;
            leftDefault.RotZ = 0.0f;
            leftDefault.FovUp = 10.0f;
            leftDefault.FovDown = 10.0f;
            profile.LeftProperty = leftDefault;

            GlassProfile.LenseProperty rightDefault = new GlassProfile.LenseProperty();
            rightDefault.PosX = 0.0315f;
            rightDefault.PosY = 0.0f;
            rightDefault.PosZ = 0.0f;
            rightDefault.RotX = 0.0f;
            rightDefault.RotY = -0.45f;
            rightDefault.RotZ = 0.0f;
            rightDefault.FovUp = 10.0f;
            rightDefault.FovDown = 10.0f;
            profile.RightProperty = rightDefault;

            return profile;

        }
    }

}

