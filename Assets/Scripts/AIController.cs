using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.iOS;

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
    public static event MessageEvent SelectNextLine;

    #endregion Events


    // Start is called before the first frame update
    private void OnEnable()
    {
        if (boardManager == null)       
            boardManager = FindObjectOfType<BoardManager>();

        //C# : on inscrit la fonction LooseMessage à l'event
        ValidateLineButton.ValidateLine += VerifyLine;
        BoardManager.BoardGenerated += Init;

    }

    void Init()
    {
        Debug.Log("Init");
        if (boardManager.TestMode == false)
            HideIABoard();

        GenerateCode();
    }

    void HideIABoard()
    {
        boardManager.IALine.iALineGameObject.SetActive(false);
    }
    public void GenerateCode()
    {
        for (int i = 0; i < boardManager.Rules.slotsByLine; i++)
        {
            if (boardManager.Colors.ColorsList.Count > 0)
            {
                // ran material
                int ran = UnityEngine.Random.Range(1, boardManager.Colors.ColorsList.Count);
                boardManager.IALine.slotsList[i].AssignMaterialToBall(boardManager.Colors.ColorsList[ran]);
                boardManager.IALine.slotsList[i].indexAssignedColor = ran;
                boardManager.IALine.slotsList[i].ActivateBallGameObject();

                boardManager.IALine.AssignedColorsList.Add(boardManager.Colors.ColorsList[ran].color);
            }
        }
    }
    private void VerifyLine()
    {
        if (boardManager == null)
            boardManager = FindObjectOfType<BoardManager>();

        int CorrectSlots = 0;
        int WrongPlace = 0;
        int Repetition = 0;

        List<Color> usedColorsList = new List<Color>();

        for (int i = 0; i < boardManager.Board.linesList[boardManager.CurrentLine].slotsList.Count; i++)
        {
            Color iABallColor = boardManager.IALine.slotsList[i].GetBallColor();
            Color BallColor = boardManager.Board.linesList[boardManager.CurrentLine].slotsList[i].GetBallColor();

            if (BallColor == iABallColor)
            {
                CorrectSlots++;

                if (usedColorsList.Contains(iABallColor) == false)
                    usedColorsList.Add(iABallColor);
                
            }
            else
            {
                // color correct but wrong place
                if (boardManager.IALine.AssignedColorsList.Contains(BallColor) == true)
                {
                    WrongPlace++;

                    if (usedColorsList.Contains(iABallColor) == false)
                        usedColorsList.Add(iABallColor);
                    else
                    {
                        Repetition++;
                    }
                }
            }
            // disable slot
            boardManager.Board.linesList[boardManager.CurrentLine].slotsList[i].DisableSlot();
        }
        
        // set mini balls X
        for (int i = 0; i < boardManager.Board.linesList[boardManager.CurrentLine].miniSlotsXManager.MiniBallSlotsList.Count; i++)
        {
            int i1 = (i + CorrectSlots);
            int i2 = (WrongPlace + CorrectSlots + Repetition);

            if (i2 > 0)
            {
                if (i < CorrectSlots)
                {
                    boardManager.Board.linesList[boardManager.CurrentLine].miniSlotsXManager.MiniBallSlotsList[i].ballGameObject.GetComponent<MeshRenderer>().material
                            = boardManager.Colors.BlackColor;

                    boardManager.Board.linesList[boardManager.CurrentLine].miniSlotsXManager.MiniBallSlotsList[i].ballGameObject.SetActive(true);
                }
                else if (i1 - 1 < i2)
                {
                    boardManager.Board.linesList[boardManager.CurrentLine].miniSlotsXManager.MiniBallSlotsList[i].ballGameObject.GetComponent<MeshRenderer>().material
                            = boardManager.Colors.WhiteColor;

                    boardManager.Board.linesList[boardManager.CurrentLine].miniSlotsXManager.MiniBallSlotsList[i].ballGameObject.SetActive(true);
                }
            }
       //     Debug.Log(CorrectSlots + " / " + WrongPlace + " / " + Repetition + " _ i = " + i
        //       + " _ i + CorrectSlots =" + i1 + " / CorrectSlots + WrongPlace + Repetition =" + i2);
        }

        // win game ?
        if (CorrectSlots == boardManager.Rules.slotsByLine)
        {
            Win();
        }
        else if (boardManager.CurrentLine == boardManager.Rules.linesByBoard
            && CorrectSlots == boardManager.Rules.slotsByLine)
        {
            Loose();
        }
        else
        {
            GoToNextLine();
        }
    }
    private void GoToNextLine()
    {
        SelectNextLine?.Invoke();
    }

    private void ShowIABoard()
    {
        boardManager.IALine.iALineGameObject.SetActive(true);
    }

    private void Win()
    {
        ShowIABoard();
        WinGame?.Invoke();
    }

    private void Loose()
    {
        LooseGame?.Invoke();
    }

    private void OnDisable()
    {
        ValidateLineButton.ValidateLine -= VerifyLine;
        BoardManager.BoardGenerated -= Init;

    }
}
