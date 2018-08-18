using UnityEngine;
using System.Collections;
//using Lotus;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using LARSuite;

public class SmallFishManager : GazeEventTrigger
{

    public static SmallFishManager _instance;
    public List<List<FishSwiming>> fishList = new List<List<FishSwiming>>();
    private int minNum = 12;
    private int maxNum = 20;
    private GameObject FishParent;
    public static int FishNum = 0;
    private VRSDKGZ VRGz;
    //点击间隔
    private float ClickWaittimer = 0f;
   // public Camera TsCamera;
    private void Awake()
    {
        _instance = this;
        VRGz = this.GetComponent<VRSDKGZ>();
        FishParent = new GameObject();
        FishParent.name = "FishParent";
        FishParent.transform.parent = null;
        //TsCamera = GameObject.Find("TsCm").GetComponent<Camera>();
        clown_triggerfish = VRGz.clown_triggerfish;
        clownfish = VRGz.clownfish;
        lionfish = VRGz.lionfish;
        
    }


    private GameObject clown_triggerfish;
    private GameObject clownfish;
    private GameObject lionfish;
    private float Zdis = 1.5f;
    private float timer = 0f;
    private float radio = 0.3f;
    public AudioSource fishSound;

    public void SpwanFish(Vector3 pos)
    {
        fishSound.Play();
        GameObject fishGm = null;
        Transform gzGm = GameObject.Find("Main Camera").transform;
        GameObject go = new GameObject();
        go.transform.parent = gzGm;
        go.transform.localEulerAngles = new Vector3(0,Random.Range(-60,60),0);
        List<FishSwiming> fishes =null;
        if (gzGm != null)
        {
            fishes = new List<FishSwiming>();
            int num = Random.Range(minNum, maxNum);
            FishNum+=num;
            int n = Random.Range(0, 3);
            if (n == 0) fishGm = clown_triggerfish;
            else if (n == 1) fishGm = clownfish;
            else fishGm = lionfish;
            
            for (int i = 0; i < num; i++)
            {
                go.transform.localPosition = new Vector3(Random.Range(-radio, radio),Random.Range(-radio, radio),Zdis);             
                //GameObject gg = GameObject.Instantiate(fishGm, pos, Quaternion.identity) as GameObject;
                GameObject gg = GameObject.Instantiate(fishGm, go.transform.position, go.transform.rotation) as GameObject;
                gg.transform.parent = FishParent.transform;
                fishes.Add(gg.GetComponent<FishSwiming>());

            }
           
            if (fishes!=null&&fishes.Count>0)
            {
                fishList.Add(fishes);
                Debug.Log(fishList[0].Count);
                if (fishList.Count>5&&FishNum>100)
                {
                    Debug.Log(fishList[0].Count);
                    for (int i = 0; i < fishList[0].Count; i++)
                    {
                      
                        fishList[0][i].Death = true;
                        Debug.Log(fishList[0][i].Death);
                    }
                    //delete MoreThan Nums Fishes
                    fishList.RemoveAt(0);
                }
               
            }
            Debug.Log(gzGm.position+"Ros:"+gzGm.rotation);
            Destroy(go);
        }
        
    }

    void GazyClick()
    {
        if (ClickWaittimer>=0.5f)
        {
            SpwanFish(Vector3.zero);
            Debug.Log("Click");
            ClickWaittimer = 0f;
        }
       
    }


    public override void OnGazeEnter(PointerEventData data)
    {

    }
    public override void OnGazeExist(PointerEventData data)
    {

    }
    public override void OnGazeMove(PointerEventData data)
    {

    }
    public override void OnGazeClick(PointerEventData data)
    {
        Debug.Log("OnGazeClick");
        GazyClick();
    }


    private void Update()
    {
        ClickWaittimer += Time.deltaTime;
        //if (Input.GetKeyUp(KeyCode.JoystickButton0)
        //        || Input.GetKeyUp(KeyCode.Return)
        //        || Input.GetKeyUp(KeyCode.KeypadEnter)
        //        || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        //        || Input.GetMouseButtonDown(0))
        //{
        //    GazyClick();
        //}
    }

}
