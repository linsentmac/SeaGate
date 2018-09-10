using LARSuiteInternal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
namespace LARSuite
{
    public class FindPlane
    {
      
        public delegate void FindPlanesResultCallBack(int planesCount, Vector3[] vertices, int[] triangles, Vector3[] normals, Vector2[] uvs);

        public static FindPlanesResultCallBack s_findPlanesResultCallBackFunc;

        private static FindPlaneSO.FindPlanesCallBack s_findPlanesCallBack;

        public static void SetMovEnable(bool enable)
        {
            FindPlaneSO.movEnableDisable(enable);
        }
        public static void SetCallBack(FindPlanesResultCallBack cb)
        {
           // cb = FindPlanesCallBackFunc;
            s_findPlanesCallBack = FindPlanesCallBackFunc;
            s_findPlanesResultCallBackFunc = cb;
            FindPlaneSO.setCallBack(s_findPlanesCallBack);
        }

        public static void StartFind()
        {
            FindPlaneSO.startFind();
        }

        public static void StopFind()
        {
            FindPlaneSO.stopFind();
        }

        private static void FindPlanesCallBackFunc(IntPtr f, IntPtr v, IntPtr t, int fLen, int vLen, int tLen)
        {
            List<Vector3> verticesList = new List<Vector3>();
            List<Vector2> uvList = new List<Vector2>();
            List<Vector3> normalsList = new List<Vector3>();

            int offset = 0;
            int planesCount = Marshal.ReadInt32(f);
            offset += 2 * 4;

            if (tLen == 0)
            {
                planesCount = 0;
                return;
            }

            int d = 4;
            float a = (float)(2f / Math.Sqrt(3) * d);
            float b = (float)(2 * (Math.Sqrt(2) - 1) * d);

            for (int i = 0; i < planesCount; i++)
            {
                float[] center = new float[7];
                Marshal.Copy((IntPtr)(f.ToInt32() + offset), center, 0, 7);
                int triaglesCount = Marshal.ReadInt32((IntPtr)(f.ToInt32() + (offset += 28)));
                float[] v2 = new float[triaglesCount * 9];
                Vector3 normal = new Vector3(center[3], center[4], center[5]);
                Marshal.Copy((IntPtr)(f.ToInt32() + (offset += 4)), v2, 0, triaglesCount * 9);

                for (int j = 0; j < triaglesCount * 3; j++)
                {
                    normalsList.Add(normal);
                    Vector3 vertex = new Vector3(v2[j * 3], v2[j * 3 + 1], v2[j * 3 + 2]);
                    verticesList.Add(vertex);
                    uvList.Add(new Vector2((float)((vertex.x + a / 2)), (float)((vertex.y) + b / 2)));

                }
                offset += triaglesCount * 3 * 3 * 4;
            }

            int[] triangles = new int[verticesList.Count];
            for (int m = 0; m < triangles.Length / 3; m++)
            {
                triangles[m * 3 + 0] = m * 3 + 0;
                triangles[m * 3 + 1] = m * 3 + 1;
                triangles[m * 3 + 2] = m * 3 + 2;
            }

            if (s_findPlanesResultCallBackFunc != null)
            {
                s_findPlanesResultCallBackFunc(planesCount, verticesList.ToArray(), triangles, normalsList.ToArray(), uvList.ToArray());
            }
        }

    }
}