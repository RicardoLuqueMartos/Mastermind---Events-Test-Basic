using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotBase : MonoBehaviour
{
    [SerializeField]
    protected BoardManager boardManager;

    [SerializeField]
    protected GameObject ballGameObject;
    [SerializeField]
    protected GameObject selectedCircle;
    [SerializeField]
    protected GameObject bar;
    [SerializeField]
    public int ballIndex;


    public void ActivateBallGameObject()
    {
        ballGameObject.SetActive(true);
    }

    public void DisactivateBallGameObject()
    {
        ballGameObject.SetActive(false);
    }

    public void AssignMaterialToBall(Material material)
    {
        ballGameObject.GetComponent<MeshRenderer>().material = material;                
    }

    public bool IsBallActiveInHierarchy()
    {
       return ballGameObject.activeInHierarchy;
    }

    public Color GetBallColor()
    {
        return ballGameObject.GetComponent<MeshRenderer>().material.color;
    }
}
