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

        ValidateLineButton.ValidateLine += CheckCode;

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
    public void CheckCode()
    {
        Debug.Log("Verification de ligne");
        bool[] goodColors = new bool[boardManager.Rules.slotsByLine];
        bool[] WrongPlacement = new bool[boardManager.Rules.slotsByLine];
        
        int nbGoodColors = 0;
        int nbBadPos = 0;
        int currentResultBallsIndex = 0;

        for (int i = 0; i < boardManager.Rules.slotsByLine; i++) // Verification of good colors & positions
        {
            Color iABallColor = boardManager.IALine.slotsList[i].GetBallColor();
            Color BallColor = boardManager.Board.linesList[boardManager.CurrentLine].slotsList[i].GetBallColor();

            if (BallColor == iABallColor)
            {
                nbGoodColors++;
                goodColors[i] = true;
            }
        }
        if (nbGoodColors >= boardManager.Rules.slotsByLine)
        {
            Win();
        }
        else
        {
            for (int i = 0; i < boardManager.Rules.slotsByLine; i++)
            {
                if (!goodColors[i])
                {
                    Color _BallColor = boardManager.Board.linesList[boardManager.CurrentLine].slotsList[i].GetBallColor(); // la couleur de chaque qui n'est pas ok

                    for (int j = 0; j < boardManager.Rules.slotsByLine; j++)
                    {
                        Color _iABallColor = boardManager.IALine.slotsList[j].GetBallColor(); // la couleur de chaque de l'ia qui n'est pas ok

                        if (_BallColor == _iABallColor && !WrongPlacement[j] && !goodColors[i])
                        {
                            nbBadPos++;
                            WrongPlacement[j] = true;
                            break;
                        }                    
                    }                
                }
            }
            Debug.Log("nbGoodColors "+ nbGoodColors+ " / nbBadPos " + nbBadPos);
            
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
        ValidateLineButton.ValidateLine -= CheckCode;
        BoardManager.BoardGenerated -= Init;

    }
}
