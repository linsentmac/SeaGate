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
    ///  Recongize the gaze click event, drive by the <see cref="GazeInputModule"/>. 
    /// </summary>
    public class GazeClickRecognizer : IGazeEventRecognizer {

        private PointerEventData _pointerData;

        public GazeClickRecognizer() {
        }

        public void SetPointerEventData(PointerEventData data) {
            _pointerData = data;
        }

        public void Process() {

            if (_pointerData == null) return;

#if UNITY_EDITOR
            bool pressed = Input.GetMouseButtonDown(0);
            bool released = Input.GetMouseButtonUp(0);
#else
            bool pressed = TouchInput.TouchStart();
            bool released = TouchInput.TouchEnd();
#endif
            if (pressed) {
                ProcessPress();
            }

            if (released) {
                ProcessRelease();
            }
        }

        public void ProcessPress() {

            if (_pointerData.eligibleForClick) return;

            var gameObject = _pointerData.pointerCurrentRaycast.gameObject;

            _pointerData.pressPosition = _pointerData.position;
            _pointerData.pointerPressRaycast = _pointerData.pointerCurrentRaycast;
            _pointerData.pointerPress =
              ExecuteEvents.ExecuteHierarchy(gameObject, _pointerData, ExecuteEvents.pointerDownHandler)
                ?? ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);

            _pointerData.rawPointerPress = gameObject;
            _pointerData.eligibleForClick = true;
            _pointerData.delta = Vector2.zero;
            _pointerData.dragging = false;
            _pointerData.useDragThreshold = true;
            _pointerData.clickCount = 1;
            _pointerData.clickTime = Time.unscaledTime;

        }

        public void ProcessRelease() {

            ExecuteEvents.Execute(_pointerData.pointerPress, _pointerData, ExecuteEvents.pointerUpHandler);
            if (_pointerData.eligibleForClick) {
                ExecuteEvents.Execute(_pointerData.pointerPress, _pointerData, ExecuteEvents.pointerClickHandler);
            }
            
            _pointerData.pointerPress = null;
            _pointerData.rawPointerPress = null;
            _pointerData.eligibleForClick = false;
            _pointerData.clickCount = 0;
            _pointerData.clickTime = 0;
            _pointerData.pointerDrag = null;
            _pointerData.dragging = false;
        }
    }

}
