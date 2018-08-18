using UnityEngine;
using System.Collections;
namespace LARSuite
{
    public class CylinderEventHandler : MonoBehaviour
    {

        private Color _color;
        private MeshRenderer _render;
      
        void Start()
        {
           
            _render = gameObject.GetComponent<MeshRenderer>();
            _color = _render.material.color;
        }

        public void OnPointerClick()
        {
            _render.material.color = Color.cyan;
        }

        public void OnPointerEnter()
        {
            _render.material.color = Color.red;
        }

        public void OnPointerExist()
        {
            _render.material.color = _color;
        }
    }
}
