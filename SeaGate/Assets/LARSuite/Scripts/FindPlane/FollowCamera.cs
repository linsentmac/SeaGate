using System;
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

        private float x_Distance;
        private float y_Distance;
        private float z_Distance;

        private float x_rotation_Init;
        private float y_rotation_Init;

        private float gateY;

        private GameObject gateObject;

        void Start()
        {
            LarCamera = GameObject.Find("Main Camera");
            distance = Vector3.Distance(transform.position, LarCamera.transform.position);

            //Debug.Log("tmac distance " + distance);


            x_Distance = transform.position.x;
            y_Distance = transform.position.y;
            z_Distance = transform.position.z;
            //Debug.Log("Tmac initial distance " + x_Distance + "/" + y_Distance + "/" + z_Distance);
            //Debug.Log("Tmac Camera Rotation x = " + LarCamera.transform.rotation.eulerAngles.x + "y = " + LarCamera.transform.rotation.eulerAngles.y + "z= " + LarCamera.transform.rotation.eulerAngles.z);

            gateObject = GameObject.Find("Gate");
            x_rotation_Init = this.transform.rotation.eulerAngles.x;
            y_rotation_Init = this.transform.rotation.eulerAngles.y;

            gateY = this.transform.position.y;
        }

        void Update()
        {
            //targetPosition = LarCamera.transform.position + LarCamera.transform.forward * distance;
            //Debug.Log("tmac position " + transform.position);
            //Debug.Log("tmac targetPosition " + targetPosition);
            //Vector3 offset = transform.position - targetPosition;
            //if (offset.magnitude > MaxOffset)
            //{
            //    transform.rotation = LarCamera.transform.rotation;
            //    targetPosition += offset.normalized * MaxOffset;
            //    transform.position = Vector3.Lerp(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
            //}
            //Debug.Log("tmac Camera Position  = " + LarCamera.transform.position);
            
            targetPosition = LarCamera.transform.position + LarCamera.transform.right * x_Distance 
                                                          + LarCamera.transform.up * y_Distance 
                                                          + LarCamera.transform.forward * z_Distance;


            //Debug.Log("tmac rotation x = " + transform.rotation.eulerAngles.x + "\n" + "y = " + transform.rotation.eulerAngles.y + "\n" + "z = " + transform.rotation.eulerAngles.z);
            //Debug.Log("tmac Camera Rotation x = " + LarCamera.transform.rotation.eulerAngles.x + "\n" + "y = " + LarCamera.transform.rotation.eulerAngles.y + "\n" + "z= " + LarCamera.transform.rotation.eulerAngles.z);



            //Debug.Log("tmac position " + transform.position);
            //Debug.Log("tmac targetPosition " + targetPosition);



            Quaternion rotation = Quaternion.Euler((LarCamera.transform.rotation.eulerAngles.x + 7), (LarCamera.transform.rotation.eulerAngles.y + 170), (LarCamera.transform.rotation.eulerAngles.z));
            //transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime);

            Vector3 offset = transform.position - targetPosition;
            if (offset.magnitude > MaxOffset)
            {
                //transform.rotation = LarCamera.transform.rotation;
                targetPosition += offset.normalized * MaxOffset;
                transform.position = Vector3.Lerp(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
            }

            followCameraRotation();
        }


        private void followCameraRotation() {

            float arrowX = this.transform.position.x;
            float gateX = gateObject.transform.position.x;

            float arrowY = this.transform.position.y;
            //float gateY = gateObject.transform.position.y;

            float arrowZ = this.transform.position.z;
            float gateZ = gateObject.transform.position.z;

            float x_dis = gateX - arrowX;
            float y_dis = gateY - arrowY;
            float z_dis = gateZ - arrowZ;

            // arctan 通过反三角函数计算旋转角度 (弧度值/π=角度值/180)（Math.PI = π）
            double targetRotationX = x_rotation_Init;
            double targetRotationY = y_rotation_Init;
            if (Math.Abs(y_dis) > 0.5 && Math.Abs(z_dis) > 0.1) {
                targetRotationX = Math.Atan(y_dis / z_dis) / Math.PI * 180 + x_rotation_Init;
            }
            if (Math.Abs(x_dis) > 0.3 && Math.Abs(z_dis) > 0.1) {
                targetRotationY = Math.Atan(x_dis / z_dis) / Math.PI * 180 + y_rotation_Init;
            }
            
            //double targetRotationZ = Math.Atan(y_dis / x_dis) / Math.PI * 180;
            //Debug.Log("tmac arrow rotation x = " + targetRotationX + " y = " + targetRotationY);

            double targetRotationZ = 0;
            if (arrowX < -0.3) {
                targetRotationZ = 15;
            } else if (arrowX > 0.3) {
                targetRotationZ = -15;
            }

            this.transform.eulerAngles = new Vector3((float)targetRotationX, (float)targetRotationY, (float)targetRotationZ);

        }
    }
}