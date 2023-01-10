using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidateLineButton : MonoBehaviour, UsableObject
{
    //Version C# on relie l'appel du delegate � l'event. 
    // La d�finition du delegate va d�terminer quelles informations seront disponibles. 
    // ATTENTION la signature du delegate doit �tre respect�e par les fonctions qui �coutent.
    public delegate void MessageEvent();
    public static event MessageEvent ValidateLine;

    // Start is called before the first frame update
    public void UseObject()
    {
        ValidateLine?.Invoke();
    }

}
