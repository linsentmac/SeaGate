//-----------------------------------------------------------------------
// <copyright>
//
// Copyright 2016 Lenovo Inc. All Rights Reserved.
//
// </copyright>
//-----------------------------------------------------------------------

using UnityEngine;

namespace LARSuite
{
    /// <summary>
    ///  Gaze target encapsulate gaze ray interaction target objet and the 
    ///  source camera. 
    /// </summary>
    public class GazeTarget {
        public Camera Source { get; set; }
        public GameObject Target { get; set; }
        public Vector3 Position { get; set; }
        public bool Reactable { get; set; }
    }

}
