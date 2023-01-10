using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBallSlot : MonoBehaviour , UsableObject
{

    public GameObject ballGameObject;
    public GameObject selectedCircle;
    public GameObject bar;
    public int ballIndex;

    public void UseObject(BoardManager boardManager)
    {
    //    boardManager.SelectColor(colorIndex);
    }
}
