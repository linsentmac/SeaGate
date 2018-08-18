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
    ///  An interface to listen <see cref="RenderMode"/> change. 
    /// </summary>
    public interface IRenderListener {
        void OnRenderModeChange(RenderController.RenderMode mode);
    }


}

