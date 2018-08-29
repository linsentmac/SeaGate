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
    ///  An interface to listen the <see cref="GlassProfile"/> update.
    /// </summary>
    public interface IGlassProfileListener {

        void OnLenseSeperationChange(float seperation);
        void OnLenseFrustumChange(GlassProfile.LenseFrustum frustum);

    }

}
