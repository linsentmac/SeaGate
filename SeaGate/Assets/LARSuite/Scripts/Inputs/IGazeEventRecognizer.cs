//-----------------------------------------------------------------------
// <copyright>
//
// Copyright 2016 Lenovo Inc. All Rights Reserved.
//
// </copyright>
//-----------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace LARSuite
{
    /// <summary>
    ///  An interface for Gaze event recongnizer, all recongnizer should register
    ///  to and drive by <see cref="GazeInputModule"/>. 
    /// </summary>
    public interface IGazeEventRecognizer {

        void SetPointerEventData(PointerEventData data);
        void Process();
    }
}
