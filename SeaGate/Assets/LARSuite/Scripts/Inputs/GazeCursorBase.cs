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
    ///  A base class for a gaze cursor render, it register itself to <see cref="GazeInputState"/>
    ///  as a <see cref="IGazeInputListener"/>, it's derived class could implement IGazeInputListener
    ///  interface to draw a customize gaze cursor.
    ///  
    /// </summary>
    public abstract class GazeCursorBase : MonoBehaviour, IGazeInputListener {

        protected float _distance = 10.0f;
        protected const float _minDistance = 0.3f;
        protected const float _maxDistance = 10.0f;

        public abstract void OnGazeDisabled();
        public abstract void OnGazeEnabled();
        public abstract void OnGazeEnter(GazeTarget target);
        public abstract void OnGazeExit(GazeTarget target);
        public abstract void OnGazeMoveOver(GazeTarget target);

        static GazeCursorBase _instance = null;

        static public GazeCursorBase Instance() {

            if(_instance == null) {
                GameObject go = GameObject.Find("GazeCursor");
                if (go) {
                    _instance = go.GetComponent<GazeCursorBase>();
                }
            }
            
            return _instance;
        }

        public void Awake() {
            GazeInputState.Instance.Register(this);
            gameObject.SetActive(false);
        }

        public void Update() {
            _distance = Mathf.Clamp(_distance, _minDistance, _maxDistance);
        }

        public void OnDestroy() {
            GazeInputState.Instance.UnRegister(this);
        }

        public Vector3 WorldPosition() {
            Vector3 localPosition = new Vector3(0.0f, 0.0f, _distance);
            return transform.TransformPoint(localPosition);
        }

        protected void UpdateLocalDistanceFromTarget(GazeTarget target) {
            Vector3 targetLocalPosition = new Vector3(target.Position.x, target.Position.y, target.Position.z);
            if (Vector3.zero.Equals(targetLocalPosition))
            {
                //Debug.Log("target Position is zero : ");
                if(target.Target != null)
                {
                    targetLocalPosition = new Vector3(target.Target.transform.position.x,
                        target.Target.transform.position.y,
                        target.Target.transform.position.z);
				}
            }

            float tmpDistance = Vector3.Distance(GazeCursorBase.Instance().transform.position, targetLocalPosition);
            _distance = Mathf.Clamp(tmpDistance, _minDistance, _maxDistance);
        }
    }

}
