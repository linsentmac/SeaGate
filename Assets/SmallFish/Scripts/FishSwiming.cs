using UnityEngine;
using System.Collections;

public class FishSwiming : MonoBehaviour {

    public float Swimingsmooth = 10f;

    private float timer = 0f;
    public int countTimer = 2;

    public float xsmooth = 0;
    public float ysmooth = 0;

    private Transform Head;
    private Animator Am;
    private Material material;
    private Color color = new Color(1,1,1,0);
    public bool Death = false;
    public bool Destory = false;

    float Dis = 1f;
    private bool IsClose = false;



    private void Start()
    {
        Head = GameObject.Find("Main Camera").transform;
           Am = this.GetComponentInChildren<Animator>();
        Swimingsmooth = Random.Range(0.2f,0.8f);
        if (Swimingsmooth>0.4f)
        {
            Am.SetBool("Fast",true);
        }

        RandomDir();

        material = this.GetComponentInChildren<Renderer>().material;
        material.SetColor("_Color", color);
    }
    // Update is called once per frame
    void Update () {
        //FishLiveOrDeath();
        //if (Death)
        //{
        //    color = Color.Lerp(color, new Color(1, 1, 1, 0), 0.3f*Time.deltaTime);

        //    if (color.a<=0.1f)
        //    {
        //        Destroy(this.gameObject);
        //    }
        //}
        //else
        //{
        //    color = Color.Lerp(color, new Color(1, 1, 1, 1), Time.deltaTime);
        //}          

        //material.SetColor("_Color", color);


        //transform.Translate(Vector3.forward*Swimingsmooth*Time.deltaTime,Space.Self);
        //timer += Time.deltaTime;

        //transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,0);
        //float xangle = transform.eulerAngles.x;
        //float yangle = transform.eulerAngles.y;
        //if (xangle > 180) xangle -= 360;
        //if (yangle > 180) yangle -= 360;

        //if (xangle > 45f || xangle < -45f || timer > countTimer)
        //{
        //    if (timer>countTimer)
        //    {
        //        timer = 0;
        //        RandomDir();
        //    }
        //    if (xangle> 45f)
        //    {
        //        if (xsmooth > 0)
        //        {
        //            return;
        //        } 
        //    }
        //    else if (xangle < -45f)
        //    {
        //        if (xsmooth<0)
        //        {
        //            return;
        //        }
        //    }
        //    else
        //    {
        //        return;
        //    }

        //}

        //transform.Rotate(Vector3.up,ysmooth*Time.deltaTime, Space.Self);
        //transform.Rotate(xsmooth*Time.deltaTime,0,0);

        FishLiveOrDeath();

        material.SetColor("_Color", color);
        transform.Translate(Vector3.forward * Swimingsmooth * Time.deltaTime, Space.Self);
        timer += Time.deltaTime;

        FishMove();

    }

    void RandomDir()
    {
        countTimer = Random.Range(0, 5);
        int i = Random.Range(0, 2);
        if (i == 0)
        {
            xsmooth = Random.Range(-50, 50);
            ysmooth = 0;
        }
        else
        {
            ysmooth = Random.Range(-50, 50);
            xsmooth = 0;
        }
    }



    void FishLiveOrDeath()
    {
        if (!Death && color.a < 0.99f)
        {
            color = Color.Lerp(color, new Color(1, 1, 1, 1), Time.deltaTime);
        }
        else if (Death)
        {
            color = Color.Lerp(color, new Color(1, 1, 1, 0), 1.5f * Time.deltaTime);
            if (color.a < 0.1f)
            {
                //SmallFishManager.FishNum--;
                if (Destory) {
                    Destroy(this.gameObject);
                }
                
            }
        }
    }



    void FishMove()
    {
        if (Head != null)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
            float xangle = transform.eulerAngles.x;

            float dis = Vector3.Distance(transform.position, Head.position);
            if (dis > 15) Death = true;
            //float dis = Mathf.Abs(transform.position.z-Sunflower.position.z);
            if (xangle > 180) xangle -= 360;
            // Debug.Log(Quaternion.Angle(transform.rotation, Sunflower.rotation));
            if (dis <= Dis && Quaternion.Angle(transform.rotation, Head.rotation) > 100)
            {
                if (!IsClose)
                {

                    float y = Vector3.Cross(transform.forward, transform.position).y;
                    // Debug.Log(y);
                    if (y > 0)
                    {
                        ysmooth = 80;

                    }
                    else
                    {
                        ysmooth = -80;
                    }

                    countTimer = 0;
                    xsmooth = 0;
                    IsClose = true;
                }

            }
            else
            {
                if (IsClose) IsClose = false;
                if (xangle > 45 || xangle < -45f || timer > countTimer)
                {
                    if (timer > countTimer)
                    {
                        timer = 0;
                        RandomDir();
                    }
                    if (xangle > 45f)
                    {
                        if (xsmooth > 0)
                        {
                            return;
                        }
                    }
                    else if (xangle < -45f)
                    {
                        if (xsmooth < 0)
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }


                }
            }


            transform.Rotate(Vector3.up, ysmooth * Time.deltaTime, Space.Self);
            transform.Rotate(xsmooth * Time.deltaTime, 0, 0);
        }
    }
}
