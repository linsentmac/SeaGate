using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LARSuite;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Keyboard : GazeEventTrigger
{
    public Text mInputText;

    public override void OnGazeClick(PointerEventData data)
    {
        mInputText = GameObject.Find("KeyBoard").GetComponent<KeyBoardManager>().mInput;
        var oldText = mInputText.text.Trim();
    
        var myText = transform.Find("Text").GetComponent<Text>().text;
        if ("<-".Equals(myText))
        {
            if (oldText.Length > 0) {
                mInputText.text = oldText.Substring(0,oldText.Length-1);
            }
        }else if("-".Equals(myText)){
            mInputText.text = oldText.Insert(0,"-");
        }
        else {
            mInputText.text = oldText + myText;
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
