using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WhaleAm : MonoBehaviour {

    private float WaitAmtimer =1f;
    public float SwimTimer = 33f;
    public Material WhaleMat;
    private float timer=0f;
    public GameObject Whale;
    private Animator Am;

    private Color alpha = new Color(1,1,1,0);
    private float WaitTimer = 1f;
    public RawImage rawImage;

	// Use this for initialization
	void Start () {
     
        Am = Whale.GetComponent<Animator>();
        WaitTimer = Random.Range(1,3.5f);
   
        WhaleMat.SetColor("_Color", new Color(1, 1, 1, 0));
        Whale.SetActive(false);
        StartCoroutine(ShowTV());

    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if (timer>=WaitTimer)
        {
            
                Whale.SetActive(true);
                alpha = Color.Lerp(alpha, new Color(1, 1, 1, 1), 2 * Time.deltaTime);       
                 WhaleMat.SetColor("_Color", alpha);
           
        }

	}

    IEnumerator ShowTV()
    {
        yield return new WaitForSeconds(0.1f);

        if (rawImage.texture != null)
        {
            rawImage.texture = null;
        }

    }
}
