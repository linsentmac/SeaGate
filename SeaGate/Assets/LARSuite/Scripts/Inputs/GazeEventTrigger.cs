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
using System;

namespace LARSuite
{
    /// <summary>
    ///  A customize EventTrigger for gaze event. Pointer events are mapped to Gaze events now, 
    ///  we supply four Gaze event: GazeEnter, GazeClick, GazeMove, and GazeExist. 
    /// </summary>
    public abstract class GazeEventTrigger : EventTrigger, IPointerMoveHandler {

        public abstract void OnGazeEnter(PointerEventData data);

        public abstract void OnGazeClick(PointerEventData data);

        public abstract void OnGazeMove(PointerEventData data);

        public abstract void OnGazeExist(PointerEventData data);

        public override void OnPointerClick(PointerEventData eventData) {
            OnGazeClick(eventData);
        }

        public override void OnPointerEnter(PointerEventData eventData) {
            OnGazeEnter(eventData);
        }

        public override void OnPointerExit(PointerEventData eventData) {
            OnGazeExist(eventData);
        }

        public void OnPointerMove(PointerEventData eventData) {
            OnGazeMove(eventData);
        }
    }
}

