//-----------------------------------------------------------------------
// <copyright>
//
// Copyright 2016 Lenovo Inc. All Rights Reserved.
//
// </copyright>
//-----------------------------------------------------------------------

using UnityEngine;

namespace LARSuite
{
    /// <summary>
    ///  An implementation of <see cref="GazeCursorBase"/>, our default gaze cursor renderer.
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class GazeCursor : GazeCursorBase {
        public float _explosionSpeed = 2.0f;

        private int _longitudeSegment = 30;
        private int _latitudeSegment = 30;
        private float _minRadius = 0.00001f;
        private float _innerFadeout = 0.0333f;
        private float _outerFadeout = 0.0666f;
        private float _maxRadius = 0.1f;

        private Material _material;
        
        private bool _inExplosion = false;
        private float _explosion = 0.0f;
        private float _minExplosion = 0.0f;
        private float _maxExplosion = 0.06f;
       // private GameObject head;

        //private float Timestamps = 1f;
        //private float timer = 0;
        //private Vector3 headPositon;
        //private Quaternion headRotation;

        void Start() {
            var meshRender = gameObject.GetComponent<MeshRenderer>();
            meshRender.material = Resources.Load("GazeCursorMaterial") as Material;
            meshRender.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            meshRender.receiveShadows = false;
            meshRender.useLightProbes = false;

            Mesh cursor = CreateOpenCircleVertices(_minRadius, _maxRadius);
            gameObject.AddComponent<MeshFilter>();
            GetComponent<MeshFilter>().mesh = cursor;
            _material = gameObject.GetComponent<Renderer>().material;
            _inExplosion = false;
           

        }

        void Update() {
            base.Update();
            UpdateCursor();
         
        }
     
        private void UpdateCursor() {
            float scale = _distance / _maxDistance;

            _explosion += (_inExplosion ? 1 : -1) * _explosionSpeed * Time.deltaTime;
            _explosion = Mathf.Clamp(_explosion, _minExplosion, _maxExplosion);

            _material.SetFloat("_scale", scale);
            _material.SetFloat("_explosion", _explosion);
            _material.SetFloat("_distance", _distance);
            _material.SetFloat("_minRadius", _minRadius);
            _material.SetFloat("_innerFadeout", _innerFadeout);
            _material.SetFloat("_maxRadius", _maxRadius);
            _material.SetFloat("_outerFadeout", _outerFadeout);
        }

        public override void OnGazeEnabled() {
            gameObject.SetActive(true);
        }

        public override void OnGazeDisabled() {
            gameObject.SetActive(false);
        }

        public override void OnGazeEnter(GazeTarget target) {
           
            //gameObject.SetActive(true);
            UpdateLocalDistanceFromTarget(target);
            if (target.Reactable) _inExplosion = true;
        }

        public override void OnGazeMoveOver(GazeTarget target) {
            UpdateLocalDistanceFromTarget(target);
        }

        public override void OnGazeExit(GazeTarget target) {
            if (GameObject.Find("LarCamera").GetComponent<LarManager>().HideEnableGazeInput == true)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
            }
        
           
            _distance = _maxDistance;
            if (target.Reactable) _inExplosion = false;

        }

        private Mesh CreateOpenCircleVertices(float innerRadius, float outterRadius) {
            Mesh mesh = new Mesh();
            int longitudes = _longitudeSegment;
            int latitudes = _latitudeSegment;
            int vertex_count = (longitudes + 1) * 2 * latitudes;
            int indices_count = (longitudes + 1) * 3 * 2 * latitudes;

            Vector3[] vertices = new Vector3[vertex_count];
            int[] indices = new int[indices_count];
            int vi = 0;
            int vert = 0;
            int idx = 0;
            const float kTwoPi = Mathf.PI * 2.0f;
            float latitudeStep = (outterRadius - innerRadius) / latitudes;
            float inner = innerRadius;
            float outter = innerRadius + latitudeStep;

            for(int latitude = 0; latitude < latitudes; latitude++) {
                for (int longitude = 0; longitude <= longitudes; ++longitude) {
                    float angle = (float)longitude / (float)(longitudes) * kTwoPi;
                    float x = Mathf.Sin(angle);
                    float y = Mathf.Cos(angle);
                    vertices[vi++] = new Vector3(x * outter, y * outter, 1.0f);
                    vertices[vi++] = new Vector3(x * inner, y * inner, 1.0f);
                }

                for (int longitude = 0; longitude < longitudes; ++longitude) {
                    indices[idx++] = vert + 1;
                    indices[idx++] = vert;
                    indices[idx++] = vert + 2;

                    indices[idx++] = vert + 1;
                    indices[idx++] = vert + 2;
                    indices[idx++] = vert + 3;

                    vert += 2;
                }

                vert += 2;
                inner = outter;
                outter += latitudeStep;
            }

            mesh.vertices = vertices;
            mesh.triangles = indices;
            mesh.RecalculateBounds();
            ;
            mesh.UploadMeshData(true);

            return mesh;
        }
    }
}


