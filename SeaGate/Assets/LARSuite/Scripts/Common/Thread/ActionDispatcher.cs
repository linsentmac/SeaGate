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
    /// Dispatch actions to a given IActionQueue in Async.
    /// </summary>
    public class ActionDispatcher : Singleton<ActionDispatcher> {
        private ActionDispatcher() { }

        public void DispatchAsync(IActionQueue queue, Action<object> action, object userData = null) {
            ActionItem actionItem = new ActionItem {
                action = action,
                userData = userData
            };

            DispatchAsync(queue, actionItem);
        }

        public void DispatchAsync(IActionQueue queue, ActionItem actionItem) {
            queue.Enqueue(actionItem);
        }
    }
}