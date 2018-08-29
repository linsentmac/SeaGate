using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddChecker : MonoBehaviour
{
    public GameObject key;
    public GameObject mPerfabChecker;

    public GameObject mPanel;

    public Text mLeftDistance;
    public Text mRightDistance;

    public Text mInputDistance;
    public Text mInputRotationX;
    public Text mInputRotationY;
    public Text mInputRotationZ;

    public Text mInputTranslateX;
    public Text mInputTranslateY;
    public Text mInputTranslateZ;


    private float mCheckerDistance = 1.0f;

    private GameObject mChecker;
    private CheckerManager mCheckerManager;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var currentPosition = transform.position;
        Ray ray = new Ray(currentPosition, transform.forward);
        RaycastHit hit;
        bool isHit = Physics.Raycast(ray, out hit, Mathf.Infinity);
        if (isHit)
        {
            var point = hit.point;
            var distance = Vector3.Distance(currentPosition, point);
            mLeftDistance.text = distance.ToString("F4");
            mRightDistance.text = distance.ToString("F4");
        }
        else
        {
            mLeftDistance.text = "∞";
            mRightDistance.text = "∞";
        }

        if (Input.GetKeyDown(KeyCode.JoystickButton0))
        {
           GameObject canvas = GameObject.Find("Canvas");
            CanvasManager cm = canvas.GetComponent<CanvasManager>();
            if (isHit && hit.collider.gameObject.tag.Equals("UI") || cm.mUIIsShow )
            {
                return;
            }
            else
            {
                if (mChecker == null)
                {
                    mChecker = Instantiate(mPerfabChecker, transform.position + transform.forward * mCheckerDistance, Quaternion.identity);
                    mCheckerManager = mChecker.GetComponent<CheckerManager>();
                }
                else
                {
                    mChecker.transform.position = transform.position + transform.forward * mCheckerDistance;
                }

                mCheckerManager.resetLocalRotation();
                mCheckerManager.resetLocalPosition();
                mChecker.transform.LookAt(transform);
            }

        }

    }

    public void setCheckerDistance()
    {
        var text = mInputDistance.text.Trim();
        float distance = float.Parse(text);
        if (distance < 0.2) {
            distance = 1.0f;
        }

        Debug.Log("mCheckerDistance"+ mCheckerDistance);
        mCheckerDistance = distance;
    }

    public void isShowPanel(bool isShow)
    {
        mPanel.SetActive(isShow);
    }

    private string Vector3toString(Vector3 v)
    {
        return " x:" + v.x + " y:" + v.y + " z:" + v.z;
    }


    public void adjustLocalRotationX()
    {
        var text = mInputRotationX.text.Trim();
        mCheckerManager.adjustLocalRotationX(parseFloat(text));
    }

    public void adjustLocalRotationY()
    {
        var text = mInputRotationY.text.Trim();
     
        mCheckerManager.adjustLocalRotationY(parseFloat(text));
    }

    public void adjustLocalRotationZ()
    {
        var text = mInputRotationZ.text.Trim();
    
        mCheckerManager.adjustLocalRotationZ(parseFloat(text));
    }

    public void adjustLocalPositionX()
    {
        var text = mInputTranslateX.text.Trim();
     
        mCheckerManager.adjustLocalPositionX(parseFloat(text));
    }

    public void adjustLocalPositionY()
    {
        var text = mInputTranslateY.text.Trim();
      
        mCheckerManager.adjustLocalPositionY(parseFloat(text));
    }

    public void adjustLocalPositionZ()
    {
        var text = mInputTranslateZ.text.Trim();
     
        mCheckerManager.adjustLocalPositionZ(parseFloat(text));
    }

    private float parseFloat(string text) {
        float result = 0;
        if (string.IsNullOrEmpty(text))
        {
            result = 0;
        }
        else
        {
           
            result = float.Parse(text);
        }

        return result;
    }


}
