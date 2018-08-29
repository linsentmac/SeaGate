using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
namespace LARSuiteInternal
{

    public class PointCloudSo 
    {


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

    }
}
