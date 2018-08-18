using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using LARSuite;
namespace LARSuiteInternal
{
    public class pointCloud 
    {
        public  bool StartSlam()
        {
            return PointCloudSo.start_slam();
        }
        public bool StopSlam()
        {
            return PointCloudSo.stop_slam();
        }
        public int  SlamInt()
        {
            return PointCloudSo.slam_init();
        }
        public bool ModeSwitch(bool mode)
        {
            return PointCloudSo.mode_switch(mode);
        }

        public Vector3[] GetPointCloud()
        {
            const int MAX_POINT_COUNT = 500;    



            float[] m_data = new float[MAX_POINT_COUNT * 3];
            GCHandle dataObject;
            IntPtr dataInptr;
            dataObject = GCHandle.Alloc(m_data, GCHandleType.Pinned);
            dataInptr = dataObject.AddrOfPinnedObject();
           int  len = 0;
            
            PointCloudSo.get_point_cloud(out len, dataInptr);
            Vector3[] points = new Vector3[len];
            int[] indices = new int[len];

            for (int j = 0; j < len; j++)
            {
                // points
                points[j].x = m_data[3 * j];
                points[j].y = m_data[3 * j + 1];
                points[j].z = m_data[3 * j + 2];
                //index 
                indices[j] = j;

                //  Debug.Log("m_points [" + j + "]" + Vector3ToString(points[j]));
            }
            return points;
        }
        // Use this for initialization

    }
}
