using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvailableColorSlot : MonoBehaviour, UsableObject
{
    public GameObject slot;
    public GameObject ballGameObject;
    public GameObject selectedCircle;
    public GameObject bar1;
    public GameObject bar2;
    public int colorIndex;

    public void UseObject(BoardManager boardManager) {

        boardManager.SelectColor(colorIndex);
    }
}
