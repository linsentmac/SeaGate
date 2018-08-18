//-----------------------------------------------------------------------
// <copyright>
//
// Copyright 2016 Lenovo Inc. All Rights Reserved.
//
// </copyright>
//-----------------------------------------------------------------------

using UnityEngine;
using System.Collections;

namespace LARSuite
{
    /// <summary>
    ///  Map specific Key code and touch to TouchStart and TouchEnd.
    /// </summary>
    public class TouchInput {
        public static bool TouchStart() {
            return Input.GetKeyUp(KeyCode.JoystickButton0)
                || Input.GetKeyUp(KeyCode.Return)
                || Input.GetKeyUp(KeyCode.KeypadEnter)
                || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);

        }

        public static bool TouchEnd() {
            return Input.GetKeyUp(KeyCode.JoystickButton0)
                || Input.GetKeyUp(KeyCode.Return)
                || Input.GetKeyUp(KeyCode.KeypadEnter)
                || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended);
        }
    }
}

