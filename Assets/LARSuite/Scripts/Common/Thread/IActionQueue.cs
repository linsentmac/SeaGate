//-----------------------------------------------------------------------
// <copyright>
//
// Copyright 2016 Lenovo Inc. All Rights Reserved.
//
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;

namespace LARSuite
{
    /// <summary>
    /// Interface for action queue.
    /// </summary>
    public interface IActionQueue {
        void Enqueue(ActionItem actionItem);
        void ExecuteActions();
    }
}