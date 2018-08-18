//-----------------------------------------------------------------------
// <copyright>
//
// Copyright 2016 Lenovo Inc. All Rights Reserved.
//
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Threading;

namespace LARSuite
{
    /// <summary>
    /// The salve action queue in which actions will be executed on background threads.
    /// </summary>
    public class ActionSlaveQueue : Singleton<ActionSlaveQueue>, IActionQueue {
        private ActionSlaveQueue() { }

        public void Enqueue(ActionItem actionItem) {
            ThreadPool.QueueUserWorkItem(ExecuteAction, actionItem);
        }

        public void ExecuteActions() {
        }

        private void ExecuteAction(object actionItem) {
            ActionItem item = (ActionItem)actionItem;
            item.action(item.userData);
        }
    }
}

