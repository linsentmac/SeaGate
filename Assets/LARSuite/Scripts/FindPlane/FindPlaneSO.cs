using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
namespace LARSuiteInternal
{
    public class FindPlaneSO
    {

        [DllImport("tofclient")]
        public static extern void movEnableDisable(bool enable);

        [DllImport("tofclient")]
        public static extern void setCallBack(FindPlanesCallBack cb);

        [DllImport("tofclient")]
        public static extern void startFind();

        [DllImport("tofclient")]
        public static extern void stopFind();

        public delegate void FindPlanesCallBack(IntPtr v, IntPtr n, IntPtr t, int vLen, int nLen, int tLen);
    }
}
