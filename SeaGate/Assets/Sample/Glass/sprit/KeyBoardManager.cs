using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyBoardManager : MonoBehaviour {
    public Text mInput;
    public GameObject mGroup;

    public bool mIsShowKeyboard = true;

    public void keyBoardShow(bool isShow) {
        Debug.Log("group null" + (mGroup== null));
        mIsShowKeyboard = isShow;
        mGroup.SetActive(isShow);
    }
}
