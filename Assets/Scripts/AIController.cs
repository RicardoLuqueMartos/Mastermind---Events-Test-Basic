using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.iOS;

public class AIController : MonoBehaviour
{
    public enum VerifiedStateEnum { CorrectPlace, WrongPlace, WrongPlaceColorReused, WrongColor }
    public List<VerifiedStateEnum> VerifiedStatesList = new List<VerifiedStateEnum>();

    [SerializeField] bool UseNewVerification = false;

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

        //C# : on inscrit la fonction à l'event
   /*     if (!UseNewVerification)
        {
            ValidateLineButton.ValidateLine += VerifyLine;
            ValidateLineButton.ValidateLine -= checkCode;

        }
        else
        {
            ValidateLineButton.ValidateLine += checkCode;
            ValidateLineButton.ValidateLine -= VerifyLine;
        }*/

        ValidateLineButton.ValidateLine += checkCode;


        BoardManager.BoardGenerated += Init;

    }

    void Init()
    {
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
        VerifiedStatesList.Clear();

        if (boardManager == null)
            boardManager = FindObjectOfType<BoardManager>();

        int CorrectSlots = 0;
        int WrongPlace = 0;
        int currentResultBallsIndex = 0;

        List<Color> usedColorsList = new List<Color>();
        List<bool> DoublonsList = new List<bool>();

        // prepare doublons list
        for (int i = 0; i < boardManager.IALine.slotsList.Count; i++)
        {
            bool doublon = false;
            if (usedColorsList.Contains(boardManager.IALine.slotsList[i].GetBallColor())) doublon = true;
            else usedColorsList.Add(boardManager.IALine.slotsList[i].GetBallColor());
            DoublonsList.Add(doublon);
        }
          
        for (int i = 0; i < boardManager.Board.linesList[boardManager.CurrentLine].slotsList.Count; i++)
        {
            Color iABallColor = boardManager.IALine.slotsList[i].GetBallColor();
            Color BallColor = boardManager.Board.linesList[boardManager.CurrentLine].slotsList[i].GetBallColor();

            if (BallColor == iABallColor)
            {
                VerifiedStateEnum state = VerifiedStateEnum.CorrectPlace;
                VerifiedStatesList.Add(state);
                Debug.Log("CorrectPlace " + boardManager.Board.linesList[boardManager.CurrentLine].slotsList[i].name);
            }
            else
            {
                if (usedColorsList.Contains(BallColor)
                    && DoublonsList[i] == true)
                {
                    VerifiedStateEnum state = VerifiedStateEnum.WrongPlaceColorReused;
                    VerifiedStatesList.Add(state);

                    Debug.Log("WrongPlaceColorReused "+ boardManager.Board.linesList[boardManager.CurrentLine].slotsList[i].name);
                }
                else if (!usedColorsList.Contains(BallColor)
                    && DoublonsList[i] == false)
                {
                    VerifiedStateEnum state = VerifiedStateEnum.WrongPlace;
                    VerifiedStatesList.Add(state);
            
                    Debug.Log("WrongPlace1 "+ boardManager.Board.linesList[boardManager.CurrentLine].slotsList[i].name);
                }
                else if (usedColorsList.Contains(BallColor)
                    && DoublonsList[i] == false)
                {
                    VerifiedStateEnum state = VerifiedStateEnum.WrongPlace;
                    VerifiedStatesList.Add(state);

                    Debug.Log("WrongPlace2 "+ boardManager.Board.linesList[boardManager.CurrentLine].slotsList[i].name);
                }
            }
         /*   if (usedColorsList.Contains(BallColor) == false)
                usedColorsList.Add(BallColor);
         */
            // disable slot
            boardManager.Board.linesList[boardManager.CurrentLine].slotsList[i].DisableSlot();
        }

        // prepare result balls
        for (int i = 0; i < VerifiedStatesList.Count; i++)
        {
            if (VerifiedStatesList[i] == VerifiedStateEnum.CorrectPlace) CorrectSlots++;
            else if (VerifiedStatesList[i] == VerifiedStateEnum.WrongPlaceColorReused
                || VerifiedStatesList[i] == VerifiedStateEnum.WrongPlace) WrongPlace++;
        }

        // set mini balls X
        for (int i = 0; i < CorrectSlots; i++)
        {
            boardManager.Board.linesList[boardManager.CurrentLine].miniSlotsXManager.MiniBallSlotsList[i].ballGameObject.GetComponent<MeshRenderer>().material                            
                = boardManager.Colors.BlackColor;

            boardManager.Board.linesList[boardManager.CurrentLine].miniSlotsXManager.MiniBallSlotsList[i].ballGameObject.SetActive(true);

            currentResultBallsIndex++;
        }

        for (int i = currentResultBallsIndex; i < currentResultBallsIndex+WrongPlace; i++)
        {
            boardManager.Board.linesList[boardManager.CurrentLine].miniSlotsXManager.MiniBallSlotsList[i].ballGameObject.GetComponent<MeshRenderer>().material
                = boardManager.Colors.WhiteColor;

            boardManager.Board.linesList[boardManager.CurrentLine].miniSlotsXManager.MiniBallSlotsList[i].ballGameObject.SetActive(true);       
        }
        /*
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
        */

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

    public void checkCode()
    {
        Debug.Log("Verification de ligne");
        bool[] boolTab = new bool[boardManager.IALine.AssignedColorsList.Count];
        int nbGoodColors = 0;
        int nbBadPos = 0;
        int currentResultBallsIndex = 0;


        for (int i = 0; i < boardManager.IALine.slotsList.Count; i++)                               //Verification of good colors & positions
        {
            Color iABallColor = boardManager.IALine.slotsList[i].GetBallColor();
            Color BallColor = boardManager.Board.linesList[boardManager.CurrentLine].slotsList[i].GetBallColor();

            if (BallColor == iABallColor)
            {
                nbGoodColors++;
                boolTab[i] = true;
            }
        }
        if (nbGoodColors >= boardManager.IALine.slotsList.Count)
        {
            Win();
        }
        else
        {
            for (int i = 0; i < boardManager.IALine.slotsList.Count; i++)
            {
                if (!boolTab[i])
                {
                    Color BallColor = boardManager.Board.linesList[boardManager.CurrentLine].slotsList[i].GetBallColor();

                    for (int j = 0; j < boardManager.IALine.slotsList.Count; j++)
                    {
                        if (!boolTab[j])
                        {
                            Color iABallColor = boardManager.IALine.slotsList[j].GetBallColor();

                            if (BallColor == iABallColor)
                            {
                                nbBadPos++;
                                boolTab[j] = true;
                                break;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < nbGoodColors; i++)
            {
                boardManager.Board.linesList[boardManager.CurrentLine].miniSlotsXManager.MiniBallSlotsList[i].ballGameObject.GetComponent<MeshRenderer>().material
                    = boardManager.Colors.BlackColor;

                boardManager.Board.linesList[boardManager.CurrentLine].miniSlotsXManager.MiniBallSlotsList[i].ballGameObject.SetActive(true);

                currentResultBallsIndex++;
            }

            for (int i = currentResultBallsIndex; i < currentResultBallsIndex + nbBadPos; i++)
            {
                boardManager.Board.linesList[boardManager.CurrentLine].miniSlotsXManager.MiniBallSlotsList[i].ballGameObject.GetComponent<MeshRenderer>().material
                    = boardManager.Colors.WhiteColor;

                boardManager.Board.linesList[boardManager.CurrentLine].miniSlotsXManager.MiniBallSlotsList[i].ballGameObject.SetActive(true);
            }

            // win game ?
            if (nbGoodColors == boardManager.Rules.slotsByLine)
            {
                Win();
            }
            else if (boardManager.CurrentLine == boardManager.Rules.linesByBoard
                && nbGoodColors == boardManager.Rules.slotsByLine)
            {
                Loose();
            }
            else
            {
                GoToNextLine();
            }
        }
    }

 //   Envoyer un message à @Pronymor

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
     //   ValidateLineButton.ValidateLine -= VerifyLine;
        ValidateLineButton.ValidateLine -= checkCode;
        BoardManager.BoardGenerated -= Init;

    }
}
