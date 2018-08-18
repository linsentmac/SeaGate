using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smallFishControl : MonoBehaviour {
    public static smallFishControl _instance;
    public List<List<FishSwiming>> fishList = new List<List<FishSwiming>>();
    private int minNum = 12;
    private int maxNum = 20;
    private GameObject FishParent;
    public GameObject gateObject;
    public static int FishNum = 0;
    private VRSDKGZ VRGz;
    //点击间隔
    private float ClickWaittimer = 0f;
    // public Camera TsCamera;

    public GameObject clownTriggerObject;
    public GameObject clownFishObject;
    public GameObject lionFishObject;
    private SkinnedMeshRenderer triggerRendere;
    private SkinnedMeshRenderer clownFishRenderer;
    private SkinnedMeshRenderer lionFishRenderer;
    private Transform mainCameraTran;

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

        mainCameraTran = GameObject.Find("Main Camera").transform;
        triggerRendere = clownTriggerObject.GetComponent<SkinnedMeshRenderer>();
        clownFishRenderer = clownFishObject.GetComponent<SkinnedMeshRenderer>();
        //lionFishRenderer = lionFishObject.GetComponent<SkinnedMeshRenderer>();

    }
    private GameObject clown_triggerfish;
    private GameObject clownfish;
    private GameObject lionfish;
    private float Zdis = 1.5f;
    private float timer = 0f;
    private float radio = 0.3f;
    public AudioSource fishSound;

    public void SpwanFish(Vector3 pos, bool playSound)
    {
        resetSmallFish = false;
        if (playSound) {
            fishSound.Play();
        }
        
        GameObject fishGm = null;
        Transform gzGm = GameObject.Find("Main Camera").transform;
        GameObject go = new GameObject();
        go.transform.parent = gzGm;
        go.transform.localEulerAngles = new Vector3(0, Random.Range(-60, 60), 0);
        List<FishSwiming> fishes = null;
        if (gzGm != null)
        {
            fishes = new List<FishSwiming>();
            int num = Random.Range(minNum, maxNum);
            FishNum += num;
            int n = Random.Range(0, 3);
            if (n == 0) fishGm = clown_triggerfish;
            else if (n == 1) fishGm = clownfish;
            else fishGm = lionfish;

            for (int i = 0; i < num; i++)
            {
                go.transform.localPosition = new Vector3(Random.Range(-radio, radio), Random.Range(-radio, radio), Zdis);
                //GameObject gg = GameObject.Instantiate(fishGm, pos, Quaternion.identity) as GameObject;
                GameObject gg = GameObject.Instantiate(fishGm, go.transform.position, go.transform.rotation) as GameObject;
                gg.transform.parent = FishParent.transform;
                fishes.Add(gg.GetComponent<FishSwiming>());
                
            }

            if (fishes != null && fishes.Count > 0)
            {
                fishList.Add(fishes);
                if (fishList.Count > 5 && FishNum > 100)
                {
                    Debug.Log(fishList[0].Count);
                    for (int i = 0; i < fishList[0].Count; i++)
                    {

                        fishList[0][i].Death = true;
                        fishList[0][i].Destory = true;
                        FishNum--;
                        Debug.Log(fishList[0][i].Death);
                    }
                    //delete MoreThan Nums Fishes
                    fishList.RemoveAt(0);
                }

            }
            Debug.Log(gzGm.position + "Ros:" + gzGm.rotation);
            Destroy(go);
        }

    }

    void GazyClick()
    {
        if (ClickWaittimer >= 0.5f)
        {
            SpwanFish(Vector3.zero, true);
            Debug.Log("Click");
            ClickWaittimer = 0f;
        }

    }
    void Start () {
		
	}

    private bool resetSmallFish;
	// Update is called once per frame
	void Update () {
        ClickWaittimer += Time.deltaTime;
        if ((Input.GetKeyDown (KeyCode.JoystickButton0)||Input.GetMouseButtonDown (0))
                && !EnterGateController.IsFirstEnterGate())
        {
            Debug.Log("JoystickButton0");
            GazyClick();

        }
        
        //Debug.Log("tmac MainCamera z = " + mainCameraTran.position.z);
        float mainCameraZ = mainCameraTran.position.z;
        float gateZ = gateObject.transform.position.z;
        if (mainCameraZ < gateZ && fishList != null && fishList.Count > 0 && FishNum > 0)
        {
            //triggerRendere.enabled = true;
            //clownFishRenderer.enabled = true;
            //lionFishRenderer.enabled = true;
            resetSmallFish = true;
            destoryFish(true);
        }
        else if(mainCameraZ > gateZ && resetSmallFish)
        {
            //triggerRendere.enabled = false;
            //clownFishRenderer.enabled = false;
            //lionFishRenderer.enabled = false;

            // resetSmallFish
            //SpwanFish(Vector3.zero, false);
            destoryFish(false);

        }

        

    }


    private void destoryFish(bool death) {
        for (int i = 0; i < fishList.Count; i++) {
            for (int j = 0; j < fishList[i].Count; j++) {
                fishList[i][j].Death = death;
                //Destroy(fishList[i][j]);
            }
        }
        //fishList = null;
    }
    
}
