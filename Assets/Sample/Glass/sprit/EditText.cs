using LARSuite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditText : GazeEventTrigger
{
    static GameObject mKeyBoard;

    public override void OnGazeClick(PointerEventData data)
    {
        Debug.Log("input click");
        if (mKeyBoard == null)
        {
            mKeyBoard = GameObject.Find("KeyBoard");
            Debug.Log("input click mKeyBoard" + (mKeyBoard == null));
        }

        KeyBoardManager kbm = mKeyBoard.GetComponent<KeyBoardManager>();
        kbm.keyBoardShow(!kbm.mIsShowKeyboard);
        if (mKeyBoard.activeSelf)
        {
            mKeyBoard.GetComponent<KeyBoardManager>().mInput = transform.Find("Text").GetComponent<Text>();
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
}
