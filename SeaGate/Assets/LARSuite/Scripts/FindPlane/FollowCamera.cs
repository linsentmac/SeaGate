using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LARSuite
{
    public class FollowCamera : MonoBehaviour
    {

        public float MoveSpeed = 1.0f;
        public float MaxOffset = 1.0f;

        GameObject LarCamera;
        float distance;
        Vector3 targetPosition;

        void Start()
        {
            LarCamera = GameObject.Find("Main Camera");
            distance = Vector3.Distance(transform.position, LarCamera.transform.position);

            Debug.Log(" distance " + distance);
        }

        void Update()
        {
            targetPosition = LarCamera.transform.position + LarCamera.transform.forward * distance;
            Vector3 offset = transform.position - targetPosition;
            if (offset.magnitude > MaxOffset)
            {
                transform.rotation = LarCamera.transform.rotation;
                targetPosition += offset.normalized * MaxOffset;
                transform.position = Vector3.Lerp(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
            }
        }
    }
}