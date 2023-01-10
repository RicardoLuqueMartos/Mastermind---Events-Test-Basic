using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidateLineButton : MonoBehaviour, UsableObject
{
    //Version C# on relie l'appel du delegate à l'event. 
    // La définition du delegate va déterminer quelles informations seront disponibles. 
    // ATTENTION la signature du delegate doit être respectée par les fonctions qui écoutent.
    public delegate void MessageEvent();
    public static event MessageEvent ValidateLine;

    // Start is called before the first frame update
    public void UseObject()
    {
        ValidateLine?.Invoke();
    }

}
