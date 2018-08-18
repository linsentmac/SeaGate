//-----------------------------------------------------------------------
// <copyright>
//
// Copyright 2016 Lenovo Inc. All Rights Reserved.
//
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;

using System.Collections.Generic;

namespace LARSuite
{
    /// <summary>
    ///  A central place to register all <see cref="IRenderListener"/> and 
    ///  notify them if <see cref="RenderMode"/> changes. 
    /// </summary>
    public class RenderController : Singleton<RenderController> {

        public enum RenderMode {
            Mono,
            Stereo
        }

        private RenderMode _mode;
        
        private List<IRenderListener> _listeners = new List<IRenderListener>();

        private RenderController() {
            

#if UNITY_EDITOR
        _mode = RenderMode.Mono;
#else
        _mode = RenderMode.Stereo;
#endif

    }

    public RenderMode Mode {
            get {
                return _mode;
            }

            set {
                if(_mode != value) {
                    _mode = value;
                    NotifyModeChange(_mode);
                }
            }
        }

        public void Register(IRenderListener listener) {
            _listeners.Add(listener);
        }

        public void UnRegister(IRenderListener listener) {
            _listeners.Remove(listener);
        }

        public void NotifyModeChange(RenderMode mode) {
            foreach(IRenderListener listener in _listeners) {
                listener.OnRenderModeChange(mode);
            }
        }

    }
}

