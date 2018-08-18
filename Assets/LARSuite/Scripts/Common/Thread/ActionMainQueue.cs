//-----------------------------------------------------------------------
// <copyright>
//
// Copyright 2016 Lenovo Inc. All Rights Reserved.
//
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections;

namespace LARSuite
{
    /// <summary>
    /// The Main action queue in which actions will be executed on main thread.
    /// </summary>
    public class ActionMainQueue : Singleton<ActionMainQueue>, IActionQueue {
        private List<ActionItem> _actions = new List<ActionItem>();
        private object _syncActions = new object();

        private ActionMainQueue() { }

        public void Enqueue(ActionItem actionItem) {
            lock (_syncActions) {
                _actions.Add(actionItem);
            }
        }

        public bool IsEmpty() {
            return _actions.Count == 0;
        }

        public void ExecuteActions() {
            List<ActionItem> readyActions = DequeueReadyActions();
            foreach (var actionItem in readyActions) {
                actionItem.action(actionItem.userData);
            }
        }

        private List<ActionItem> DequeueReadyActions() {
            List<ActionItem> readyActions = new List<ActionItem>();
            lock (_syncActions) {
                readyActions.AddRange(_actions);
                _actions.Clear();
            }

            return readyActions;
        }
    }
}

