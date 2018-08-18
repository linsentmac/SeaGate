using AOT;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
namespace LARSuiteInternal
{
    public class CameraClient : MonoBehaviour
    {
        static int CAMERA_OFFSET = 59;

        static IntPtr map;

        static byte[] data = new byte[480 * 640];

        static string filePath = "/mnt/sdcard/Ldti/";

        public delegate void data_cb();

        [DllImport("camera_ldticlient")]
        public static extern void registorDataCallback(data_cb cb);

        [DllImport("camera_ldticlient")]
        public static extern void start_camera();

        [DllImport("camera_ldticlient")]
        public static extern void stop_camera();

        [DllImport("camera_ldticlient")]
        public static extern IntPtr ldtiInit();

        [MonoPInvokeCallback(typeof(data_cb))]
        void CameraDataNotify()
        {
            Int32 cameraWidth = Marshal.ReadInt32(map, CAMERA_OFFSET);
            Int32 cameraHeight = Marshal.ReadInt32(map, CAMERA_OFFSET + sizeof(Int32));
            Int32 frameIdx = Marshal.ReadInt32(map, CAMERA_OFFSET + 2 * sizeof(Int32));
            Int64 frameTime = Marshal.ReadInt64(map, CAMERA_OFFSET + 3 * sizeof(Int32));
            Marshal.Copy(new IntPtr(map.ToInt32() + CAMERA_OFFSET + 3 * sizeof(Int32) + sizeof(Int64)), data, 0, data.Length);

            try
            {
                string fileName = filePath + frameIdx + ".raw";
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                FileStream fs = File.Create(fileName);
                fs.Write(data, 0, data.Length);
                fs.Close();
            }
            catch (Exception e)
            {
                Debug.Log("err :" + e.ToString());
            }
        }

        void Start()
        {
            Debug.Log("start :");
            map = ldtiInit();
            registorDataCallback(CameraDataNotify);
            start_camera();
        }

        void Update()
        {

        }

        private void OnDestroy()
        {
            stop_camera();
        }

    }
}
