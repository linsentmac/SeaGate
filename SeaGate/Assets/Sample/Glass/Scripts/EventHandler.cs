
using LARSuite;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

namespace LARSuite
{
    public class EventHandler : GazeEventTrigger
    {
        public override void OnGazeClick(PointerEventData data)
        {
            Debug.Log("OnGazeClick -------");
        }

        public override void OnGazeEnter(PointerEventData data)
        {
            Debug.Log("OnGazeEnter --------");
        }

        public override void OnGazeExist(PointerEventData data)
        {
            Debug.Log("OnGazeExist ---------");
        }

        public override void OnGazeMove(PointerEventData data)
        {
            Debug.Log("OnGazeMove -----------");
        }
    }
}
