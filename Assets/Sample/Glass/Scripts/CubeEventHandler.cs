
using System;
using LARSuite;
using UnityEngine;
using UnityEngine.EventSystems;
namespace LARSuite
{
    public class CubeEventHandler : GazeEventTrigger
    {

        private Color _color;
        private MeshRenderer _render;

        void Start()
        {
            _render = gameObject.GetComponent<MeshRenderer>();
            _color = _render.material.color;
        }

        public override void OnGazeClick(PointerEventData data)
        {
            _render.material.color = Color.cyan;
        }

        public override void OnGazeEnter(PointerEventData data)
        {
            _render.material.color = Color.red;
        }

        public override void OnGazeExist(PointerEventData data)
        {
            _render.material.color = _color;
        }

        public override void OnGazeMove(PointerEventData data)
        {

        }
    }
}
