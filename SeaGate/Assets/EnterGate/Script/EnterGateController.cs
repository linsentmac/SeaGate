using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterGateController : MonoBehaviour {

    private Transform mainTran;
    private Transform gateTran;
    public AudioSource defaultSound;
    public AudioSource enterSound;
    public GameObject whale;
    public GameObject clichGif;

    public GameObject sphere;
    private Renderer sphereRenderer;

    public GameObject humanWhale;
    public GameObject arrowObject;
    public GameObject arrowMeterialObject;
    public GameObject coralObject;
    private GameObject seaGateObject;
    private GameObject gateObject;
    private GameObject qipaoObject;
    private SkinnedMeshRenderer whaleRenderer;
    private Camera camera;
    private static bool isFirstEnter = true;
    private float gifCancelTime = 7;
    private float gifHintTime;
    private bool gifHintVisiable = true;

    private Material _arrowMat;
    private float arrow_red;
    private float arrow_green;
    private float arrow_blue;

    public GameObject QiPao;

    public static bool IsFirstEnterGate() {
        return isFirstEnter;
    }

    // Use this for initialization
    void Start () {
        whale.SetActive(false);
        //defaultSound.Play();
        //arrowObject = GameObject.Find("Arrow");
        seaGateObject = GameObject.Find("seaGate");
        gateObject = GameObject.Find("Gate");
        //qipaoObject = GameObject.Find("qiPao");
        mainTran = GameObject.Find("Main Camera").transform;
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        gateTran = gateObject.transform;

        whaleRenderer = humanWhale.GetComponent<SkinnedMeshRenderer>();

        _arrowMat = arrowMeterialObject.GetComponent<Renderer>().material;
        arrow_red = _arrowMat.color.r;
        arrow_green = _arrowMat.color.g;
        arrow_blue = _arrowMat.color.b;
        //_arrowMat.color = new Color(_arrowMat.color.r, _arrowMat.color.g, _arrowMat.color.b, 0.2f);
        //sphere.GetComponent<Renderer>().material.shader = Shader.Find("Standard");
        sphereRenderer = sphere.GetComponent<Renderer>();
    }


    private float preDistance;
    // Update is called once per frame
    void Update () {

        //Debug.Log("tmac Math rotation = " + Math.Atan(-1) / Math.PI * 180);

        //Debug.Log("tmac arrow rotation enagel = " + arrowObject.transform.rotation.eulerAngles);
        float eulerAngleGateY = gateTran.rotation.eulerAngles.y;
        float eulerAngleArrowY = arrowObject.transform.rotation.eulerAngles.y;

        float distance = gateTran.position.z - mainTran.position.z;
        float arrowX = arrowObject.transform.position.x;
        float arrowY = arrowObject.transform.position.y;
        float gateX = gateTran.position.x;
        float gateY = gateTran.position.y;
        //Debug.Log("tmac arrow x = " + arrowX + " arrow y = " + arrowY + " gateX = " + gateX + " gateY = " + gateY);

        // Condition 1: Arrow obj must enter along the range of gate (Gate Range contains position <x, y> and rotation( eulerAngle y))
        //arrowX > gateX - 0.3 && arrowX < gateX + 0.4 && arrowY < gateY + 0.4 && arrowY > gateY - 0.3
                                   // && eulerAngleArrowY < eulerAngleGateY + 45 && eulerAngleArrowY > eulerAngleGateY - 45
        if (arrowX > gateX - 0.35 && arrowX < gateX + 0.35 
                                    && eulerAngleArrowY < eulerAngleGateY + 18 && eulerAngleArrowY > eulerAngleGateY - 18) {

            //Debug.Log("tmac Enter gate in the range of ............... ");
            //Debug.Log("tmac Alpha = " + _arrowMat.color.a);


            // Condition 2: The distance between the Main Camera(people site) and the Gate is within 0.5 meter. 
            if (isFirstEnter && distance < 0)
            {
                isFirstEnter = false;

                QiPao.SetActive(true);

                coralObject.SetActive(true);
                
                //sphere.GetComponent<Renderer>().material.shader = Shader.Find("InsideVisible");

                // whale appear, gate disappear, clickHintGif appear when enter gate within 0.5 meter
                if (!whale.activeSelf) {
                    whale.SetActive(true);
                }
                whaleRenderer.enabled = true;

                //sphereRenderer.enabled = true;

                //seaGateObject.SetActive(false);
                //qipaoObject.SetActive(false);
                //arrowObject.SetActive(false);
                //clichGif.SetActive(true);
                //gifHintVisiable = true;
            }
            else if (distance < 1 && arrowObject != null)
            {
                // update the alpha of arrow
                float currentDistance = distance;
                Debug.Log("tmac currentDistance = " + currentDistance + " / preDistance = " + preDistance);
                if (currentDistance <= 0.5)
                {
                    if (!defaultSound.isPlaying) {
                        defaultSound.Play();
                    }
                    //arrowObject.SetActive(false);
                    _arrowMat.color = new Color(arrow_red, arrow_green, arrow_blue, 0);
                }
                else {
                    if (defaultSound.isPlaying)
                    {
                        defaultSound.Pause();
                    }
                    if (currentDistance - preDistance < -0.05)
                    {
                        if (_arrowMat.color.a >= 0.15)
                        {
                            //Debug.Log("tmac currentAlpha = " + _arrowMat.color.a);
                            _arrowMat.color = new Color(arrow_red, arrow_green, arrow_blue, (_arrowMat.color.a - 0.15f));
                        }
                        preDistance = currentDistance;
                    }
                    else if (currentDistance - preDistance > 0.05)
                    {
                        if (_arrowMat.color.a <= 0.85)
                        {
                            _arrowMat.color = new Color(arrow_red, arrow_green, arrow_blue, (_arrowMat.color.a + 0.15f));
                        }
                        preDistance = currentDistance;
                    }
                }
                
            }
            else if(distance > 0){
                QiPao.SetActive(false);

                //sphere.GetComponent<Renderer>().material.shader = Shader.Find("Standard");
                if (sphereRenderer.enabled)
                {
                    //sphereRenderer.enabled = false;
                }

                //whale.SetActive(false);
                isFirstEnter = true;
                if (_arrowMat.color.a < 1) {
                    _arrowMat.color = new Color(arrow_red, arrow_green, arrow_blue, 1);
                }
                if (whale.activeSelf) {
                    whaleRenderer.enabled = false;
                    //whaleRenderer.material.color = Color.Lerp(whaleRenderer.material.color, new Color(whaleRenderer.material.color.r, whaleRenderer.material.color.g, whaleRenderer.material.color.b, 0), 1.5f * Time.deltaTime);

                }
                //arrowObject.SetActive(true);
                //_arrowMat.color = new Color(arrow_red, arrow_green, arrow_blue, 1);
                if (clichGif.activeSelf) {
                    clichGif.SetActive(false);
                }

                if (coralObject.activeSelf) {
                    coralObject.SetActive(false);
                }
                

            }

        }

        

        if (gifHintVisiable && distance < 0) {
            if (!clichGif.activeSelf) {
                clichGif.SetActive(true);
            }
            gifHintTime += Time.deltaTime;
            if (gifHintTime >= gifCancelTime) {
                releaseGifAnimation();
            }
        }

        if (Input.GetKeyUp(KeyCode.JoystickButton0) && gifHintVisiable)
        {
            releaseGifAnimation();
        }

        if (Input.GetKeyUp(KeyCode.JoystickButton0))
        {
            if (coralObject.activeSelf)
            {
                //coralObject.SetActive(false);
            }
            else {
                //coralObject.SetActive(true);
            }

            if (whaleRenderer.enabled)
            {
                //whaleRenderer.enabled = false;
            }
            else {
                //whaleRenderer.enabled = true;
            }
        }
    }

    private void releaseGifAnimation() {
        gifHintVisiable = false;
        clichGif.SetActive(false);
        gifHintTime = 0;
    }

}
