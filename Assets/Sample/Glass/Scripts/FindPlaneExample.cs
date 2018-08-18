using LARSuite;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;



    public class FindPlaneExample : MonoBehaviour
    {
    private static FindPlane.FindPlanesResultCallBack s_CallBack;
    bool isStart = false;
    public Text planeInfo;
    public GameObject dynamicButton;
    public GameObject cube;
   
    private int PlanesCount = 0;

    static System.Object listLocker = new System.Object();
    static List<PlaneData> pool = new List<PlaneData>();
    static List<PlaneData> active = new List<PlaneData>();
    List<PlaneData> trackedPlane = new List<PlaneData>();
    List<GameObject> trackedGameObj = new List<GameObject>();

    double keyDownTime = 0;

    void FindPlanesCallBackFunc(int planesCount, Vector3[] vertices, int[] triangles, Vector3[] normals, Vector2[] uvs)
    {
        lock (listLocker)
        {
            foreach (var meshData in active)
            {
                meshData.setMeshState(false);
            }
            pool.AddRange(active);
            active.Clear();
        }
        Loom2.RemoveAllTask();
        PlanesCount = planesCount;

        Loom2.RunOnMainThread(new MeshRunable(vertices, normals, uvs, triangles));
    }

    void Start()
    {
        FindPlane.SetMovEnable(true);
        s_CallBack = FindPlanesCallBackFunc;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.JoystickButton0))
        {

            TimeSpan ts = new TimeSpan(DateTime.Now.Ticks);
            if (ts.TotalMilliseconds - keyDownTime < 100)
            {
                keyDownTime = ts.TotalMilliseconds;
                return;
            }

            keyDownTime = ts.TotalMilliseconds;
            if (isStart)
            {
                StopFindPlanes();
                return;
            }

            GameObject CameraLeft = GameObject.Find("Main Camera");
            Ray ray = new Ray(CameraLeft.transform.position, CameraLeft.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (!hit.transform.name.Equals("Dynamic Find"))
                {
                    Debug.Log("hit.transform.name Dynamic Find: " + hit.transform.name);
                    for (int i = active.Count - 1; i >= 0; i--)
                    {
                        trackedPlane.Add(active[i]);
                        active.RemoveAt(i);
                    }
                    GameObject cubeObject = Instantiate(cube);
                    cubeObject.transform.position = hit.point;
                    cubeObject.transform.up = hit.normal;
                    trackedGameObj.Add(cubeObject);



                }
            }
        }
    }

    private void OnGUI()
    {
        planeInfo.text = "Find " + PlanesCount + " Planes";
    }

    public void StartFindPlanes()
    {
        isStart = true;

        foreach (var plane in trackedPlane)
        {
            plane.destroySelf();
        }
        trackedPlane.Clear();

        foreach (var obj in trackedGameObj)
        {
            GameObject.Destroy(obj);
        }
        trackedGameObj.Clear();

        FindPlane.SetCallBack(s_CallBack);
        FindPlane.StartFind();
        Debug.Log("Start Find");

        dynamicButton.GetComponentInChildren<Text>().text = "STOP";
    }

    public void StopFindPlanes()
    {
        isStart = false;
        FindPlane.StopFind();
        Debug.Log("Stop Find");
        Material material = Resources.Load<Material>("Trigrid_green");
        dynamicButton.GetComponentInChildren<Text>().text = "START";

    }

    public void OnClick()
    {
        if (isStart)
        {
            StopFindPlanes();
        }
        else
        {
            StartFindPlanes();
        }
    }

    public class MeshRunable : Runable
    {
        Vector3[] vertices;
        int[] triangles;
        Vector3[] normals;
        Vector2[] uvs;

        public MeshRunable(Vector3[] vertices, Vector3[] normals, Vector2[] uvs, int[] triangles)
        {
            this.vertices = vertices;
            this.triangles = triangles;
            this.normals = normals;
            this.uvs = uvs;
        }

        public void run()
        {
            lock (listLocker)
            {
                if (pool.Count > 0)
                {
                    PlaneData plane = pool[pool.Count - 1];
                    plane.mesh.vertices = vertices;
                    plane.mesh.triangles = triangles;
                    plane.mesh.normals = normals;
                    plane.mesh.uv = uvs;
                    plane.setMeshState(true);
                    GameObject camera = GameObject.Find("Main Camera");

                    plane.setPosition(camera.transform.position + camera.transform.forward * 0.01f);
                    plane.go.transform.rotation = camera.transform.rotation;
                    pool.RemoveAt(pool.Count - 1);
                    active.Add(plane);
                }
                else
                {
                    PlaneData plane = new PlaneData();
                    plane.mesh.vertices = vertices;
                    plane.mesh.triangles = triangles;
                    plane.mesh.normals = normals;
                    plane.mesh.uv = uvs;
                    plane.setMeshState(true);
                    GameObject camera = GameObject.Find("Main Camera");

                    plane.setPosition(camera.transform.position + camera.transform.forward * 0.01f);
                    plane.go.transform.rotation = camera.transform.rotation;
                    active.Add(plane);
                }
            }
        }
    }
}

