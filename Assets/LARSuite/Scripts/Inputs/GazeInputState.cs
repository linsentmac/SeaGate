//-----------------------------------------------------------------------
// <copyright>
//
// Copyright 2016 Lenovo Inc. All Rights Reserved.
//
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LARSuite
{
    /// <summary>
    ///  GazeInputState watchs the Gaze input module state and notify registered 
    ///  <see cref="IGazeInputListener"/>. 
    /// </summary>
    public class GazeInputState : Singleton<GazeInputState> {

        private GazeInputState() { }

        private List<IGazeInputListener> _stateListeners = new List<IGazeInputListener>();

        private PointerEventData _pointerData;


        private GazeTarget _currentTarget = new GazeTarget();
        private GazeTarget _previousTarget = new GazeTarget();

        public void SetPointerEventData(PointerEventData data) {
            _pointerData = data;
        }

        public void Register(IGazeInputListener listener) {
            _stateListeners.Add(listener);
        }

        public void UnRegister(IGazeInputListener listener) {
            _stateListeners.Remove(listener);
        }

        public void PreGazeCaster() {
            if (_pointerData == null) return;

            UpdateCurrentDataToGazeTarget(_previousTarget);
        }

        public void PostGazeCaster() {
            if (_pointerData == null) return;

            UpdateCurrentDataToGazeTarget(_currentTarget);

            if (_currentTarget.Target == _previousTarget.Target) {
                if (_currentTarget.Target != null) {
                    MoveOverObject(_currentTarget);
                }
            } else {
                if (_previousTarget.Target != null) {
                    LeaveObject(_previousTarget);
                }

                if (_currentTarget.Target != null) {
                    EnterObejct(_currentTarget);
                }
            }
        }

        public void EnableGaze() {

            foreach (var listener in _stateListeners) {
                listener.OnGazeEnabled();
            }
        }

        public void DisableGaze() {

            UpdateCurrentDataToGazeTarget(_currentTarget);

            GameObject currentGameObject = GetCurrentGameObject();
            if (_currentTarget.Target) {
                LeaveObject(_currentTarget);
            }

            foreach(var listener in _stateListeners) {
                listener.OnGazeDisabled();
            }

        }

        public void EnterObejct(GazeTarget target) {

            foreach(var listener in _stateListeners) {
                listener.OnGazeEnter(target);
            }
        }

        public void MoveOverObject(GazeTarget target) {
            foreach(var listener in _stateListeners) {
                listener.OnGazeMoveOver(target);
            }

            ExecuteEvents.Execute<IPointerMoveHandler>(_currentTarget.Target, _pointerData, (x, y) => x.OnPointerMove(_pointerData));
        }

        public void LeaveObject(GazeTarget target) {
            foreach(var listener in _stateListeners) {
                listener.OnGazeExit(target);
            }
        }

        GameObject GetCurrentGameObject() {
            if (_pointerData != null && _pointerData.enterEventCamera != null) {
                return _pointerData.pointerCurrentRaycast.gameObject;
            }

            return null;
        }

        void UpdateCurrentDataToGazeTarget(GazeTarget target) {

            Camera camera = _pointerData.enterEventCamera;
            GameObject gazeObject = GetCurrentGameObject();
            Vector3 intersectionPosition = _pointerData.pointerCurrentRaycast.worldPosition;

            bool clickable = _pointerData.pointerPress != null ||
                ExecuteEvents.GetEventHandler<IPointerClickHandler>(gazeObject) != null;

            target.Source = camera;
            target.Target = gazeObject;
            target.Position = intersectionPosition;
            target.Reactable = clickable;

        }

    }

}

