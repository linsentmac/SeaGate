using UnityEngine;
using System.Collections;
namespace LARSuite
{
    public class SimpleObjectRotator : MonoBehaviour
    {

        public float _speed = 10.0f;
        private bool _stop = false;

        public float Speed
        {
            get
            {
                return _speed;
            }
            set
            {
                _speed = value;
            }
        }

        void Update()
        {

            if (_stop==false)
            {
                transform.Rotate(Vector3.right * Time.deltaTime * Speed);
            }

        }

        public void StopRotate()
        {
            _stop = true;
        }
    }
}
