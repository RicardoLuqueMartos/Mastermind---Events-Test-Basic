using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSlot : MonoBehaviour , UsableObject
{

    public GameObject ballGameObject;
    public GameObject selectedCircle;
    public GameObject bar;
    public int ballIndex;

    void Start ()
    {

    }

    public void UseObject(BoardManager boardManager)
    {        
        boardManager.SelectSlot(ballIndex);
    }
}
