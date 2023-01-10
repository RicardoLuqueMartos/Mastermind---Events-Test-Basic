using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    BoardManager boardManager;

    #region Events
    //Version C# on relie l'appel du delegate à l'event. 
    // La définition du delegate va déterminer quelles informations seront disponibles. 
    // ATTENTION la signature du delegate doit être respectée par les fonctions qui écoutent.
    public delegate void MessageEvent();
    public static event MessageEvent WinGame;
    public static event MessageEvent LooseGame;

    #endregion Events

   
    // Start is called before the first frame update
    void OnEnable()
    {
        if (boardManager == null)       
            boardManager = FindObjectOfType<BoardManager>();


        //C# : on inscrit la fonction LooseMessage à l'event
        //grace à la propriété static nous devons juste trouver la classe
        ValidateLineButton.ValidateLine += VerifyLine;
    }
    private void VerifyLine()
    {
        if (boardManager == null)
            boardManager = FindObjectOfType<BoardManager>();

        int CorrectSlots = 0;
        int WrongPlace = 0;
        int Repetition = 0;

        List<Color> usedColorsList = new List<Color>();

        for (int i = 0; i < boardManager.board.linesList[boardManager.CurrentLine].slotsList.Count; i++)
        {
            if (boardManager == null) Debug.Log("boardManager");
            else if (boardManager.board == null) Debug.Log("boardManager.board");
            else if (boardManager.board.linesList[boardManager.CurrentLine] == null) Debug.Log("boardManager.board.linesList[boardManager.CurrentLine]");
            else if (boardManager.board.linesList[boardManager.CurrentLine].slotsList[i] == null) Debug.Log("boardManager.board.linesList[boardManager.CurrentLine].slotsList[i]");
            else if (boardManager.board.linesList[boardManager.CurrentLine].slotsList[i].ballGameObject == null) Debug.Log("boardManager.board.linesList[boardManager.CurrentLine].slotsList[i].ballGameObject");
            else if (boardManager.board.linesList[boardManager.CurrentLine].slotsList[i].ballGameObject.GetComponent<MeshRenderer>() == null) Debug.Log("boardManager.board.linesList[boardManager.CurrentLine].slotsList[i].ballGameObject.GetComponent<MeshRenderer>()");
            else if (boardManager.board.linesList[boardManager.CurrentLine].slotsList[i].ballGameObject.GetComponent<MeshRenderer>().material == null) Debug.Log("boardManager.board.linesList[boardManager.CurrentLine].slotsList[i].ballGameObject.GetComponent<MeshRenderer>().material");
            else if (boardManager.iALine == null) Debug.Log(" boardManager.iALine");
            else if (boardManager.iALine.slotsList[i] == null) Debug.Log("boardManager.iALine.slotsList[i]");
            else if (boardManager.iALine.slotsList[i].ballGameObject == null) Debug.Log("boardManager.iALine.slotsList[i].ballGameObject");
            else if (boardManager.iALine.slotsList[i].ballGameObject.GetComponent<MeshRenderer>() == null) Debug.Log("boardManager.iALine.slotsList[i].ballGameObject.GetComponent<MeshRenderer>()");
            else if (boardManager.iALine.slotsList[i].ballGameObject.GetComponent<MeshRenderer>().material == null) Debug.Log("boardManager.iALine.slotsList[i].ballGameObject.GetComponent<MeshRenderer>().material");

            if (boardManager.board.linesList[boardManager.CurrentLine].slotsList[i].ballGameObject.GetComponent<MeshRenderer>().material.color
                == boardManager.iALine.slotsList[i].ballGameObject.GetComponent<MeshRenderer>().material.color)
            {
                CorrectSlots++;

                if (usedColorsList.Contains(boardManager.iALine.slotsList[i].ballGameObject.GetComponent<MeshRenderer>().material.color) == false)
                    usedColorsList.Add(boardManager.iALine.slotsList[i].ballGameObject.GetComponent<MeshRenderer>().material.color);
                else
                {
                    //    Repetition++;
                }
            }
            else
            {
                // color correct but wrong place
                if (boardManager.iALine.AssignedColorsList.Contains
                    (boardManager.board.linesList[boardManager.CurrentLine].slotsList[i].ballGameObject.GetComponent<MeshRenderer>().material.color) == true)
                {
                    WrongPlace++;

                    if (usedColorsList.Contains(boardManager.iALine.slotsList[i].ballGameObject.GetComponent<MeshRenderer>().material.color) == false)
                        usedColorsList.Add(boardManager.iALine.slotsList[i].ballGameObject.GetComponent<MeshRenderer>().material.color);
                    else
                    {
                        Repetition++;
                    }

                    //   usedColorsList.Add(iALine.slotsList[i].ballSlot.ballGameObject.GetComponent<MeshRenderer>().material.color);
                }
            }
            // disable slot
            boardManager.DisableSlot(boardManager.board.linesList[boardManager.CurrentLine].slotsList[i]);
        }
        // set balls
        for (int i = 0; i < boardManager.board.linesList[boardManager.CurrentLine].miniSlotsXManager.MiniBallSlotsList.Count; i++)
        {
            int i1 = (i + CorrectSlots);
            int i2 = (WrongPlace + CorrectSlots + Repetition);

            if (i < CorrectSlots)
            {
                boardManager.board.linesList[boardManager.CurrentLine].miniSlotsXManager.MiniBallSlotsList[i].ballGameObject.GetComponent<MeshRenderer>().material
                        = boardManager.Colors.BlackColor;

                boardManager.board.linesList[boardManager.CurrentLine].miniSlotsXManager.MiniBallSlotsList[i].ballGameObject.SetActive(true);
            }
            else if (i1 - 1 < i2)
            {
                boardManager.board.linesList[boardManager.CurrentLine].miniSlotsXManager.MiniBallSlotsList[i].ballGameObject.GetComponent<MeshRenderer>().material
                        = boardManager.Colors.WhiteColor;

                boardManager.board.linesList[boardManager.CurrentLine].miniSlotsXManager.MiniBallSlotsList[i].ballGameObject.SetActive(true);
            }

            Debug.Log(CorrectSlots + " / " + WrongPlace + " _ i = " + i
               + " _ i + CorrectSlots =" + i1 + " / CorrectSlots + WrongPlace =" + i2);

        }

        // win game
        if (CorrectSlots == boardManager.rules.slotsByLine)
        {
            Debug.Log("win");
            WinGame?.Invoke();
        }
        else if (boardManager.CurrentLine == boardManager.rules.linesByBoard
            && CorrectSlots == boardManager.rules.slotsByLine)
        {
            LooseGame?.Invoke();
        }
        else
        {
            boardManager.SelectNextLine();
        }
    }
}
