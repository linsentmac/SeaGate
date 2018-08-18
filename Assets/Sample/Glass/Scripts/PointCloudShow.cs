using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class PointCloudShow : MonoBehaviour
{
   // public Text m_inputSize;
    private const int MAX_POINT_COUNT = 500;
    private Mesh m_mesh;

    //private Vector3[] m_points = new Vector3[MAX_POINT_COUNT];
    int[] m_indices = new int[MAX_POINT_COUNT];
    private float[] m_data = new float[MAX_POINT_COUNT * 3];
    private int totalPointCount = 0;

    private bool m_isInit = false;

    [DllImport("larclient")]
    public static extern bool start_slam();
    [DllImport("larclient")]
    public static extern bool stop_slam();
    [DllImport("larclient")]
    public static extern int slam_init();

    [DllImport("larclient")]
    public static extern bool mode_switch(bool mode);
    [DllImport("larclient")]
    public static extern void get_point_cloud(out int len, IntPtr data);


    GCHandle dataObject;
    IntPtr dataInptr;


    void Start()
    {
        dataObject = GCHandle.Alloc(m_data, GCHandleType.Pinned);
        dataInptr = dataObject.AddrOfPinnedObject();

        m_mesh = GetComponent<MeshFilter>().mesh;
        m_mesh.Clear();
     //   Debug.Log("start_slamqqqq ");


        bool start = start_slam();
    //    Debug.Log("start_slam....... " + start);
        bool mode = mode_switch(true);
        Debug.Log("mode_switch " + mode);
    }

    private void Update()
    {
      
        if (!m_isInit)
        {
         //   Debug.Log("Update111 ");
            if (slam_init() == 1)
            {
             //   Debug.Log("Update222 ");
                m_isInit = true;
            }
       
            return;
        }

        
        int len = 0;
        get_point_cloud(out len, dataInptr);
         Debug.Log("update len" + len);
      

        if (len == 0)
        {
            return;
        }

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

        m_mesh.Clear();
        m_mesh.vertices = points;
        m_mesh.SetIndices(indices, MeshTopology.Points, 0);
    }

    private string Vector3ToString(Vector3 v)
    {
        return "(" + v.x + "," + v.y + "," + v.z + ")";
    }

    //private void Update()
    //{
    //    int len = 0;
    //    get_point_cloud(out len, dataInptr);
    //    //  Debug.Log("update len" + len);
    //    int pointCount = len;
    //    int i = totalPointCount;
    //    totalPointCount += pointCount;

    //    for (int j = 0; j < pointCount; j++)
    //    {
    //        m_points[(i + j) % MAX_POINT_COUNT].x = m_data[3 * j];
    //        m_points[(i + j) % MAX_POINT_COUNT].y = m_data[3 * j + 1];
    //        m_points[(i + j) % MAX_POINT_COUNT].z = m_data[3 * j + 2];
    //        // m_points[(i + j) % MAX_POINT_COUNT] = new Vector3(m_data[3 * j], m_data[3 * j + 1], m_data[3 * j + 2]);
    //        //Debug.Log("(i + j) "+((i + j) % MAX_POINT_COUNT));
    //        //Debug.Log("m_points " + m_points[(i + j) % MAX_POINT_COUNT].ToString());
    //    }

    //    if (totalPointCount >= MAX_POINT_COUNT)
    //    {
    //        totalPointCount = totalPointCount % MAX_POINT_COUNT;
    //    }

    //    m_mesh.Clear();
    //    m_mesh.vertices = m_points;
    //    m_mesh.SetIndices(indices, MeshTopology.Points, 0);
    //}
}
