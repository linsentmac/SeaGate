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
    ///  An interface to listen <see cref="GazeInputModule"/> state change, like 
    ///  gaze enable/disable, gaze enter or exist <see cref="GazeTarget"/>. 
    /// </summary>
    public interface IGazeInputListener {

        void OnGazeEnabled();

        void OnGazeDisabled();

        void OnGazeEnter(GazeTarget target);

        void OnGazeMoveOver(GazeTarget target);

        void OnGazeExit(GazeTarget target);
    }

}

