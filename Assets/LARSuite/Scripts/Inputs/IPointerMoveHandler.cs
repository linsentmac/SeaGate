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
    ///  A customize IEventSystemHandler for Pointer Move event. Pointer Move event
    ///  is taken as Gaze Move event. 
    /// </summary>
    public interface IPointerMoveHandler : IEventSystemHandler {
        void OnPointerMove(PointerEventData eventData);
    }

}

