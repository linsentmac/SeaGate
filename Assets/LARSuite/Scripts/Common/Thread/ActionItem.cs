//-----------------------------------------------------------------------
// <copyright>
//
// Copyright 2016 Lenovo Inc. All Rights Reserved.
//
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace LARSuite
{
    /// <summary>
    /// Action Item to be execute in action queues.
    /// </summary>
    public struct ActionItem {
        public Action<object> action;
        public object userData;
    }
}

