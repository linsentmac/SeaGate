using UnityEngine;

namespace Lotus.MRWidgets.Common {
    public enum PivotAxis
    {
        // Rotate about all axes.
        Free,
        // Rotate about an individual axis.
        Y
    }

    public class FollowScreen : MonoBehaviour
    {

        [Header("Billboard")]

        /// <summary>
        /// The axis about which the object will rotate.
        /// </summary>
        [Tooltip("Specifies the axis about which the object will rotate.")]
        public PivotAxis PivotAxis = PivotAxis.Free;

        [Tooltip("Specifies the target we will orient to. If no Target is specified the main camera will be used.")]
        public Transform TargetTransform;

        public Camera FollowCamera;
        //public GameObject FollowCamera;

        [Header("Tagalong")]

        [Tooltip("Sphere radius.")]
        public float SphereRadius = 1.0f;

        [Tooltip("How fast the object will move to the target position.")]
        public float MoveSpeed = 2.0f;

        public float DistanceToCamera = 1.5f;

        /// <summary>
        /// When moving, use unscaled time. This is useful for games that have a pause mechanism or otherwise adjust the game timescale.
        /// </summary>
        [SerializeField]
        [Tooltip("When moving, use unscaled time. This is useful for games that have a pause mechanism or otherwise adjust the game timescale.")]
        private bool useUnscaledTime = true;

        /// <summary>
        /// Used to initialize the initial position of the SphereBasedTagalong before being hidden on LateUpdate.
        /// </summary>
        [SerializeField]
        [Tooltip("Used to initialize the initial position of the SphereBasedTagalong before being hidden on LateUpdate.")]
        private bool hideOnStart;

        [SerializeField]
        [Tooltip("Display the sphere in red wireframe for debugging purposes.")]
        private bool debugDisplaySphere;

        [SerializeField]
        [Tooltip("Display a small green cube where the target position is.")]
        private bool debugDisplayTargetPosition;

        private Vector3 targetPosition;
        private Vector3 optimalPosition;


        private void OnEnable()
        {
            if (FollowCamera == null)
            {
                FollowCamera = Camera.main;
                //FollowCamera = CameraCache.MainGo;
            }

            if (TargetTransform == null)
            {
                TargetTransform = FollowCamera.transform;
            }

            //DistanceToCamera = Vector3.Distance(transform.position, FollowCamera.transform.position);

            Update();
        }

        private void Update()
        {
            UpdateBillboard();

            UpdateTagalong();
        }

        /// <summary>
        /// Keeps the object facing the camera.
        /// </summary>
        private void UpdateBillboard()
        {
            if (TargetTransform == null)
            {
                return;
            }

            // Get a Vector that points from the target to the main camera.
            Vector3 directionToTarget = TargetTransform.position - transform.position;

            // Adjust for the pivot axis.
            switch (PivotAxis)
            {
                case PivotAxis.Y:
                    directionToTarget.y = 0.0f;
                    break;

                case PivotAxis.Free:
                default:
                    // No changes needed.
                    break;
            }

            // If we are right next to the camera the rotation is undefined. 
            if (directionToTarget.sqrMagnitude < 0.001f)
            {
                return;
            }
            
            // Calculate and apply the rotation required to reorient the object
            transform.rotation = Quaternion.LookRotation(-directionToTarget);
        }


        private void UpdateTagalong()
        {
            optimalPosition = FollowCamera.transform.position + FollowCamera.transform.forward * DistanceToCamera;
            Vector3 offsetDir = transform.position - optimalPosition;
            if (offsetDir.magnitude > SphereRadius)
            {
                targetPosition = optimalPosition + offsetDir.normalized * SphereRadius;

                float deltaTime = useUnscaledTime
                    ? Time.unscaledDeltaTime
                    : Time.deltaTime;

                transform.position = Vector3.Lerp(transform.position, targetPosition, MoveSpeed * deltaTime);
            }
        }

        private void LateUpdate()
        {
            if (hideOnStart)
            {
                hideOnStart = !hideOnStart;
                gameObject.SetActive(false);
            }
        }

        public void OnDrawGizmos()
        {
            if (Application.isPlaying == false) { return; }

            Color oldColor = Gizmos.color;

            if (debugDisplaySphere)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(optimalPosition, SphereRadius);
            }

            if (debugDisplayTargetPosition)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawCube(targetPosition, new Vector3(0.1f, 0.1f, 0.1f));
            }

            Gizmos.color = oldColor;
        }

        void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                transform.position = new Vector3(0, 0, 1.5f);
                FollowCamera.transform.forward = new Vector3(0, 0, 1.0f);
                Debug.Log("tmac FollowScreen cs OnApplicationPause position = " + transform.position);
            }
        }
    }
}
