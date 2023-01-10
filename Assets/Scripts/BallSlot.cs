using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSlot : MonoBehaviour, UsableObject
{
    #region Variables
    BoardManager boardManager;

    public GameObject ballGameObject;
    public GameObject selectedCircle;
    public GameObject bar;
    public int ballIndex;

    #endregion Variables

    public void UseObject(BoardManager _boardManager)
    {  
        boardManager = _boardManager;
        SelectSlot(ballIndex);
    }

    void SelectSlot(int SlotIndex)
    {
        if (boardManager.CurrentSlot != -1)
        {
            boardManager.AssignDefaultBoardColor(boardManager.board.linesList[boardManager.CurrentLine].slotsList
                [boardManager.CurrentSlot].ballSlot.selectedCircle.GetComponent<MeshRenderer>());
        }
        if (boardManager.CurrentSlot != SlotIndex)
        {
            boardManager.SetCurrentSlot(SlotIndex);

            boardManager.AssignSelectionColor(boardManager.board.linesList
                [boardManager.CurrentLine].slotsList[boardManager.CurrentSlot].ballSlot.selectedCircle.GetComponent<MeshRenderer>());
        }
        else boardManager.SetCurrentSlot(-1);

        boardManager.ResetColorsSelection();
    }
}
