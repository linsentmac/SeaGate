//-----------------------------------------------------------------------
// <copyright>
//
// Copyright 2016 Lenovo Inc. All Rights Reserved.
//
// </copyright>
//-----------------------------------------------------------------------

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace LARSuite
{
    /// <summary>
    ///  A customize Unity Input Module for Gaze Event. It cast a ray from screen center point to 
    ///  virtual scene, get the cast result and send them to <see cref="IGazeEventRecognizer"/> to
    ///  recognize and trigger gaze event, pump them into Unity event system. 
    ///  
    /// </summary>
    public class GazeInputModule : BaseInputModule {

        private PointerEventData pointerData;

        private List<IGazeEventRecognizer> _gazeEventRecongizers = new List<IGazeEventRecognizer>();

        public override bool ShouldActivateModule() {
            return base.ShouldActivateModule();
        }

        public override void ActivateModule() {
            base.ActivateModule();
            PreparePointerEventData();
            GazeInputState.Instance.SetPointerEventData(pointerData);
            GazeInputState.Instance.EnableGaze();
        }

        public override void DeactivateModule() {
            base.DeactivateModule();
            if (pointerData != null) {
                HandlePointerExitAndEnter(pointerData, null);
                pointerData = null;
            }
            eventSystem.SetSelectedGameObject(null, GetBaseEventData());
            _gazeEventRecongizers.Clear();

            GazeInputState.Instance.DisableGaze();
        }

        public void RegisterGazeRecognizer(IGazeEventRecognizer recognizer) {
            recognizer.SetPointerEventData(pointerData);
            _gazeEventRecongizers.Add(recognizer);
        }

        public override void Process() {
            GazeInputState.Instance.PreGazeCaster();
            CastingGazeRay();
            GazeInputState.Instance.PostGazeCaster();

            foreach (var recognizer in _gazeEventRecongizers) {
                recognizer.Process();
            }
        }
        
        private void CastingGazeRay() {

            pointerData.Reset();
            pointerData.position = GetGazeScreenPosition();
            eventSystem.RaycastAll(pointerData, m_RaycastResultCache);
            pointerData.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
            m_RaycastResultCache.Clear();

            var gazeObject = pointerData.pointerCurrentRaycast.gameObject;
            HandlePointerExitAndEnter(pointerData, gazeObject);
            var selected = ExecuteEvents.GetEventHandler<ISelectHandler>(gazeObject);
            if (selected == eventSystem.currentSelectedGameObject) {
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, GetBaseEventData(),
                                      ExecuteEvents.updateSelectedHandler);
            } else {
                eventSystem.SetSelectedGameObject(null, pointerData);
            }
        }

        public override bool IsPointerOverGameObject(int pointerId) {
            return pointerData != null && pointerData.pointerEnter != null;
        }

        
        void PreparePointerEventData() {
            if (pointerData == null) {
                pointerData = new PointerEventData(eventSystem);

                GazeClickRecognizer clickRecongizer = new GazeClickRecognizer();
                RegisterGazeRecognizer(clickRecongizer);
            }
        }
        
        private Vector2 GetGazeScreenPosition() {
            int viewportWidth = Screen.width;
            int viewportHeight = Screen.height;
            return new Vector2(0.5f * viewportWidth, 0.5f * viewportHeight);
        }
    }

}

