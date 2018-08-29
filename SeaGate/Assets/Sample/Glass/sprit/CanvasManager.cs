using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour {
    public GameObject mUIGroup;
    public GameObject mKeyboard;
    public GameObject mKeyboardGroup;

    public bool mUIIsShow = true;
    // Use this for initialization
    void Start () {
        mKeyboard.GetComponent<KeyBoardManager>().keyBoardShow(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void showUI(bool isShow) {
        mUIIsShow = isShow;
        mUIGroup.SetActive(isShow);
        if (!isShow) {
            mKeyboardGroup.SetActive(false);
        }
    }
}
