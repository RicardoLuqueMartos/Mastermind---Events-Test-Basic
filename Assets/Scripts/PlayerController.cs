using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    RaycastHit hit;

    // Update is called once per frame
    void Update()
    {
        CastRay();
    }
       
    void CastRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100))
        {
            if (Input.GetMouseButtonDown(0)
                && hit.transform.GetComponent<UsableObject>() != null)
            {
                hit.transform.GetComponent<UsableObject>().UseObject(GlobalVariables.boardManager);
                    
            }
        }
    }
}
